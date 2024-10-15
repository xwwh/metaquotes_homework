using MetaquotesHomework.Services;
using MetaquotesHomework.Tests.Tools;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MetaquotesHomework.Tests.Services;

internal class GeobaseProviderTests
{
    [TestCase("../assets/geobase.test", false)]
    [TestCase("../assets/geobase.zip", true)]
    public void CreateAsync_WhenFile_Test(string path, bool zipped)
    {
        path = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, path));
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Geobase:Provider", "file" },
                { "Geobase:Location", path },
                { "Geobase:Zipped", zipped.ToString() },
            })
            .Build();
        using var builder = new GeobaseProvider(config);
        var geobase = builder.GetGeobase();

        Assert.That(geobase.Size, Is.EqualTo(1));
        var location = new Location(geobase.GetLocation(0));
        Assert.That(location.Country, Is.EqualTo("cou_CY"));
        Assert.That(location.City, Is.EqualTo("cit_Lima"));
    }

    private class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly byte[] _data;

        public HttpMessageHandlerStub(byte[] data)
        {
            _data = data;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var msg = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            msg.Content = new ByteArrayContent(_data);
            return Task.FromResult(msg);
        }
    }
}
