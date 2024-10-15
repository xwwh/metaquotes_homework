namespace MetaquotesHomework.Tests;

internal class UtilsTests
{
    [TestCase("FF.FF.FF.FF")]
    [TestCase("1.1.1.1.")]
    [TestCase("1.1.1.")]
    [TestCase("1.1.1")]
    [TestCase("1.1..")]
    [TestCase(".1.1.1")]
    [TestCase("256.1.1.1")]
    [TestCase("1.1.1.256")]
    [TestCase("1111.1.1.1")]
    [TestCase("1.1.1.1111")]
    public void IpToUint_WhenInvalidValue_ShouldBeNull(string value)
    {
        var actual = Utils.IpToUint(value);
        Assert.That(actual, Is.Null);
    }

    [TestCase("0.0.0.0", 0)]
    [TestCase("0.0.0.1", 1)]
    [TestCase("0.0.1.1", 0x101)]
    [TestCase("0.1.1.1", 0x10101)]
    [TestCase("1.1.1.1", 0x1010101)]
    [TestCase("255.255.255.255", 0xFFFFFFFF)]
    public void IpToUint_Smoke(string value, long expected)
    {
        var actual = Utils.IpToUint(value);
        Assert.That(actual, Is.EqualTo((uint)expected));
    }
}
