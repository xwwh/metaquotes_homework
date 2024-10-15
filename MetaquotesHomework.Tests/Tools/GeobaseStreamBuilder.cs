using MetaquotesHomework.Contracts;
using System.Text;

namespace MetaquotesHomework.Tests.Tools;

internal class GeobaseStreamBuilder
{
    public List<IpLocation> _ips = new List<IpLocation>();
    public List<Location> _locations = new List<Location>();

    public GeobaseStreamBuilder Append(Location location, uint from, uint to)
    {
        _ips.Add(new IpLocation(from, to, (uint)_locations.Count));
        _locations.Add(location);
        return this;
    }

    public Stream Build()
    {
        var ms = new MemoryStream();
        var writer = new BinaryWriter(ms);
        writer.Write(0);
        ms.Write(new byte[32]);
        writer.Write(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        writer.Write(_locations.Count);
        var pos = ms.Position;
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);

        var offset1 = (int)ms.Position;
        foreach (var ip in _ips)
        {
            writer.Write(ip.From);
            writer.Write(ip.To);
            writer.Write(ip.LocationIndex);
        }

        var offset3 = (int)ms.Position;
        var list = new List<(string, int)>();
        foreach (var item in _locations)
        {
            var val = (int)(ms.Position - offset3);
            list.Add((item.City, val));
            ms.Write(item.AsBytes());
        }

        var offset2 = (int)ms.Position;
        foreach (var item in list.OrderBy(x => x.Item1))
        {
            writer.Write(item.Item2);
        }

        ms.Seek(pos, SeekOrigin.Begin);
        writer.Write(offset1);
        writer.Write(offset2);
        writer.Write(offset3);

        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}
