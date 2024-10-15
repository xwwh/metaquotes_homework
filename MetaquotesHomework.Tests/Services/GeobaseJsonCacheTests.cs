using MetaquotesHomework.Services;
using MetaquotesHomework.Tests.Tools;
using System.Text;
using System.Text.Json;

namespace MetaquotesHomework.Tests.Services;

internal class GeobaseJsonCacheTests
{
    [Test]
    public void SerializeJson_Smoke()
    {
        var expected = new string(@"{
                ""country"":""val1"",
                ""region"":""val2"",
                ""postal"":""val3"",
                ""city"":""val4"",
                ""org"":""val5"",
                ""lat"":1.234,
                ""long"":5.678
            }".Where(c => !char.IsWhiteSpace(c)).ToArray());
        var location = new Location
        {
            Country = "val1",
            Region = "val2",
            Postal = "val3",
            City = "val4",
            Org = "val5",
            Lat = 1.234f,
            Long = 5.678f
        };

        var actual = GeobaseJsonCache.SerializeJson(location.AsBytes());
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetJson_Smoke()
    {
        var expected = Enumerable.Range(0, 10).Select(i => Location.Random()).ToArray();
        var bulder = new GeobaseStreamBuilder();
        foreach (var x in expected)
        {
            uint from = 100;
            bulder.Append(x, from, from + 100);
            from += 101;
        }
        var geobase = GeobaseReader.Read(bulder.Build());
        var cache = new GeobaseJsonCache(geobase);
        for(var i = 0; i < expected.Length; i++)
        {
            var json = cache.GetJson(i);
            var str = Encoding.UTF8.GetString(json);
            var actual = JsonSerializer.Deserialize<Location>(json, new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true
            });
            Assert.That(actual, Is.EqualTo(expected[i]));
        }
    }
}
