using System.Buffers.Binary;
using System.Text;

namespace MetaquotesHomework.Tests.Tools;

internal struct Location
{
    public string Country = "cou_";
    public string Region = "reg_";
    public string Postal = "pos_";
    public string City = "cit_";
    public string Org = "org_";
    public float Lat = 0;
    public float Long = 0;

    public static Location Random()
    {
        return new Location
        {
            Country = $"cou_{Guid.NewGuid().GetHashCode() & 0xFF}",
            Region = $"reg_{Guid.NewGuid().GetHashCode() & 0xFFFF}",
            Postal = $"pos_{Guid.NewGuid().GetHashCode() & 0xFFFF}",
            City = $"cit_{Guid.NewGuid().GetHashCode() & 0xFFFF}",
            Org = $"org_{Guid.NewGuid().GetHashCode() & 0xFFFF}",
            Lat = Guid.NewGuid().GetHashCode() / 10000.0f,
            Long = Guid.NewGuid().GetHashCode() / 10000.0f
        };
    }

    public Location(Span<byte> data)
    {
        Country = Encoding.UTF8.GetString(data.Slice(0, 8)).Trim('\0');
        Region = Encoding.UTF8.GetString(data.Slice(8, 12)).Trim('\0');
        Postal = Encoding.UTF8.GetString(data.Slice(20, 12)).Trim('\0');
        City = Encoding.UTF8.GetString(data.Slice(32, 24)).Trim('\0');
        Org = Encoding.UTF8.GetString(data.Slice(56, 32)).Trim('\0');
        Lat = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(88, 4));
        Long = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(92, 4));
    }

    public byte[] AsBytes()
    {
        var res = new byte[Constants.LocationSize];
        Encoding.UTF8.GetBytes(Country).CopyTo(res, 0);
        Encoding.UTF8.GetBytes(Region).CopyTo(res, 8);
        Encoding.UTF8.GetBytes(Postal).CopyTo(res, 20);
        Encoding.UTF8.GetBytes(City).CopyTo(res, 32);
        Encoding.UTF8.GetBytes(Org).CopyTo(res, 56);
        BitConverter.GetBytes(Lat).CopyTo(res, 88);
        BitConverter.GetBytes(Long).CopyTo(res, 92);
        return res;
    }

    public override string ToString()
    {
        return $"{Country},{Region},{Postal},{City},{Org},{Lat},{Long}";
    }
}
