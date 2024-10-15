using MetaquotesHomework.Contracts;

namespace MetaquotesHomework.Services;

public class Geobase
{
    private readonly IpLocation[] _ips;
    private readonly byte[] _data;
    private readonly int[] _cityIndex;

    public Geobase(IpLocation[] ips, byte[] data, int[] cityIndex)
    {
        _ips = ips;
        _data = data;
        _cityIndex = cityIndex;
    }

    public int Size => _ips.Length;

    public Span<byte> GetLocation(int index)
    {
        return _data.AsSpan(index * Constants.LocationSize, Constants.LocationSize);
    }

    public int FindFirstCity(string city)
    {
        var left = 0;
        var right = _cityIndex.Length - 1;
        while (left < right)
        {
            var mid = (left + right) / 2;
            var offset = _cityIndex[mid];
            var res = CompareCity(city, offset);
            if (res == 0)
            {
                right = mid;
            }
            else if (res < 0)
                right = mid;
            else
                left = mid + 1;
        }

        var comp = CompareCity(city, _cityIndex[left]);
        return comp == 0 ? _cityIndex[left] / Constants.LocationSize : -1;
    }

    public bool CheckCity(string expected, int index)
    {
        return index >= 0 && index < _ips.Length
            ? CompareCity(expected, index * Constants.LocationSize) == 0
            : false;
    }

    public int FindLocationByIp(uint ip)
    {
        var left = 0;
        var right = _ips.Length - 1;
        IpLocation location;
        while (left < right)
        {
            var mid = (left + right) / 2;
            location = _ips[mid];
            if (ip >= location.From && ip <= location.To)
                return mid;
            if (ip < location.From)
                right = mid;
            else
                left = mid + 1;
        }
        location = _ips[right];
        return ip >= location.From && ip <= location.To
            ? right : -1;
    }

    private int CompareCity(string expected, int offset)
    {
        var actual = _data.AsSpan(offset + Constants.CityOffset, Constants.CitySize);
        var i = 0;
        while (i < actual.Length && i < expected.Length)
        {
            if (expected[i] > 255)
                return -1;
            var c = (byte)expected[i];
            if (c > actual[i])
                return 1;
            if (c < actual[i])
                return -1;
            i++;
        }
        return i == actual.Length || actual[i] == 0 ? 0 : -1;
    }
}
