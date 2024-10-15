using MetaquotesHomework;
using MetaquotesHomework.Contracts;
using MetaquotesHomework.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IGeobaseJsonProvider, GeobaseProvider>();

// Add services to the container.

var app = builder.Build();

var provider = app.Services.GetRequiredService<IGeobaseJsonProvider>();
provider.Init();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/app/{**path}", FallbackToSpaAsync);
app.MapGet("/ip/location", GetLocationByIpAsync);
app.MapGet("/city/locations", GetCityLocationsAsync);
app.Run();

async Task FallbackToSpaAsync(HttpContext context, CancellationToken token)
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/html";
    var file = Path.Combine(app.Environment.WebRootPath, "index.html");
    await context.Response.SendFileAsync(file);
}

static async Task GetCityLocationsAsync(
    HttpContext context,
    [FromServices] IGeobaseJsonProvider factory,
    [FromQuery][Required][MaxLength(Constants.CitySize)] string city, 
    [FromQuery][Range(0, 10000)] int offset = 0,
    [FromQuery][Range(1, 100)] int limit = 10,
    CancellationToken token = default)
{
    if (city == null)
    {
        context.Response.StatusCode = 400;
        return;
    }

    var dataProvider = factory.GetGeobaseJson();
    var index = dataProvider.FindFirstCity(city);
    if (index == -1)
    {
        context.Response.StatusCode = 404;
        return;
    }
    index += offset;
    CalculateJsonLength(dataProvider, index, limit, city, out var cnt, out var len);

    context.Response.StatusCode = 200;
    context.Response.ContentType = Constants.ApplicationJsonContent;
    context.Response.ContentLength = len;
    await context.Response.Body.WriteAsync(Constants.OpeningBrace);
    for (var i = 0; i < cnt; i++)
    {
        if (i > 0)
            await context.Response.Body.WriteAsync(Constants.Comma);
        var location = dataProvider.GetJson(index);
        await context.Response.Body.WriteAsync(location);
    }
    await context.Response.Body.WriteAsync(Constants.ClosingBrace);
}

static async Task GetLocationByIpAsync(
    HttpContext context, 
    [FromQuery][Required] string ip,
    [FromServices]IGeobaseJsonProvider factory,
    CancellationToken token)
{
    var value = Utils.IpToUint(ip);
    if (value == null || !value.HasValue)
    {
        context.Response.StatusCode = 400;
        return;
    }

    var dataProvider = factory.GetGeobaseJson();
    var index = dataProvider.FindLocationByIp(value!.Value);
    if (index == -1)
    {
        context.Response.StatusCode = 404;
        return;
    }

    var location = dataProvider.GetJson(index);
    context.Response.StatusCode = 200;
    context.Response.ContentType = Constants.ApplicationJsonContent;
    context.Response.ContentLength = location.Length;
    await context.Response.Body.WriteAsync(location);
}

static void CalculateJsonLength(
    IGeobaseJson dataProvider,
    int startingIndex,
    int limit,
    string city,
    out int count,
    out int totalLength)
{
    count = 0;
    totalLength = 2; // length of empty array []
    var index = startingIndex;
    while (count < limit)
    {
        if (!dataProvider.CheckCity(city, index))
            break;

        var location = dataProvider.GetJson(index);
        totalLength += location.Length;
        index++;
        count++;
    }
    if (count > 0)
        totalLength += count - 1; //number of commas between json array items
}