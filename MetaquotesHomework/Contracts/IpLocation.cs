namespace MetaquotesHomework.Contracts;

public readonly struct IpLocation
{
    public readonly uint From;
    public readonly uint To;
    public readonly uint LocationIndex;

    public IpLocation(uint from, uint to, uint locationIndex)
    {
        From = from;
        To = to;
        LocationIndex = locationIndex;
    }
}
