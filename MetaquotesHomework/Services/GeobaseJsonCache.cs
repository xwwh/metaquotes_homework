using MetaquotesHomework.Contracts;
using System.Buffers.Binary;
using System.Globalization;
using System.Text;

namespace MetaquotesHomework.Services;

public class GeobaseJsonCache: IGeobaseJson
{
    private readonly Geobase _geobase;
    private readonly byte[][] _cache;

    public GeobaseJsonCache(Geobase geobase)
    {
        _geobase = geobase;
        _cache = new byte[geobase.Size][];
    }
    public int FindFirstCity(string city) =>
        _geobase.FindFirstCity(city);

    public bool CheckCity(string expected, int index) =>
        _geobase.CheckCity(expected, index);

    public int FindLocationByIp(uint value) =>
        _geobase.FindLocationByIp(value);

    public byte[] GetJson(int index)
    {
        if (_cache[index] == null)
        {
            var json = SerializeJson(_geobase.GetLocation(index));
            _cache[index] = Encoding.UTF8.GetBytes(json);
        }
            
        return _cache[index];
    }

    public static string SerializeJson(Span<byte> data)
    {
        var sb = new StringBuilder(256).Append("{");
        sb.Append("\"country\":\"");

        Append(sb, data.Slice(Constants.CountryOffset, Constants.CountrySize));
        sb.Append("\",\"region\":\"");
        Append(sb, data.Slice(Constants.RegionOffset, Constants.RegionSize));
        sb.Append("\",\"postal\":\"");
        Append(sb, data.Slice(Constants.PostalOffset, Constants.PostalSize));
        sb.Append("\",\"city\":\"");
        Append(sb, data.Slice(Constants.CityOffset, Constants.CitySize));
        sb.Append("\",\"org\":\"");
        Append(sb, data.Slice(Constants.OrgOffset, Constants.OrgSize));

        sb.Append("\",\"lat\":");
        var lat = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(Constants.LatOffset, 4));
        sb.Append(lat.ToString(CultureInfo.InvariantCulture));

        sb.Append(",\"long\":");
        var lon = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(Constants.LongOffset, 4));
        sb.Append(lon.ToString(CultureInfo.InvariantCulture));

        return sb.Append("}").ToString();
    }

    private static void Append(StringBuilder sb, Span<byte> data)
    {
        for (int i = 0; i < data.Length && data[i] != 0; i++)
            sb.Append((char)data[i]);
    }
}
