using MetaquotesHomework.Services;
using MetaquotesHomework.Tests.Tools;

namespace MetaquotesHomework.Tests.Services;

internal class GeobaseTests
{
    [Test]
    public void GetLocationTest()
    {
        var data = Enumerable.Range(0, 10).Select(i => Location.Random()).ToArray();
        var db = Generate(data);

        var actual = db.GetLocation(1);
        Assert.IsTrue(actual.SequenceEqual(data[1].AsBytes()));
    }

    [Test]
    public void FindFirstCityTest()
    {
        const int len = 10; 
        var expected = 0;
        while (expected < len)
        {
            var data = Enumerable.Range(0, len).Select(i => Location.Random()).ToArray();
            for (var i = expected; i < data.Length; i++)
            {
                data[i].City = data[^1].City;
            }
            var db = Generate(data);

            var actual = db.FindFirstCity(data[^1].City);
            Assert.That(actual, Is.EqualTo(expected));
            expected++;
        }
    }

    [TestCase(0, -1)]
    [TestCase(100, 0)]
    [TestCase(500, 4)]
    [TestCase(560, 4)]
    [TestCase(599, 4)]
    [TestCase(1099, 9)]
    [TestCase(1100, -1)]
    public void FindLocationByIpTest(int ip, int expected)
    {
        var data = Enumerable.Range(0, 10).Select(i => Location.Random()).ToArray();
        var db = Generate(data);
        var actual = db.FindLocationByIp((uint)ip);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CheckCityTest()
    {
        var data = Enumerable.Range(0, 10).Select(i => Location.Random()).ToArray();
        var db = Generate(data);

        var actual = db.CheckCity(data[1].City, 1);
        Assert.IsTrue(actual);

        actual = db.CheckCity(data[1].City, 0);
        Assert.IsFalse(actual);

        actual = db.CheckCity(data[1].City, 2);
        Assert.IsFalse(actual);
    }

    private static Geobase Generate(IEnumerable<Location> locations)
    {
        var bulder = new GeobaseStreamBuilder();
        uint from = 100;
        foreach (var x in locations)
        {
            var to = from + 99;
            bulder.Append(x, from, from + 99);
            from += 100;
        }
        return GeobaseReader.Read(bulder.Build());
    }
}
