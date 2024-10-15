using MetaquotesHomework.Services;
using MetaquotesHomework.Tests.Tools;

namespace MetaquotesHomework.Tests.Services;

internal class GeobaseReaderTests
{
    [Test]
    public void ReadTest()
    {
        var expected = Enumerable.Range(0,10).Select(i => Location.Random()).ToArray();
        var bulder = new GeobaseStreamBuilder();
        foreach(var x in expected)
        {
            uint from = 100;
            bulder.Append(x, from, from + 100);
            from += 101;
        }

        var geobase = GeobaseReader.Read(bulder.Build());
        Assert.That(geobase.Size, Is.EqualTo(expected.Length));
        for (var i = 0; i < expected.Length; i++)
        {
            var actual = new Location(geobase.GetLocation(i));
            Assert.That(actual, Is.EqualTo(expected[i]));
        }
    }
}
