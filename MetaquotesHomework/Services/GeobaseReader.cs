using MetaquotesHomework.Contracts;

namespace MetaquotesHomework.Services;

public static class GeobaseReader
{
    /// <summary>
    /// Reads geobase data from the specifed stream
    /// </summary>
    public static Geobase Read(Stream source)
    {
        using var stream = new BufferedStream(source, 4096);
        using var reader = new BinaryReader(stream);
        var position = 0;

        Seek(stream, ref position, Constants.HeaderSize);
        var recordCount = reader.ReadInt32();
        var ipsOffset = reader.ReadInt32();
        var idxOffset = reader.ReadInt32();
        var locationsOffset = reader.ReadInt32();
        position += 4 * Constants.IntSize;

        IpLocation[] ips = null!;
        byte[] data = null!;
        int[] cityIndex = null!;
        var arr = new (int offset, Action action)[]
        {
            (ipsOffset, () => ips = ReadIps(stream, reader, ref position, ipsOffset, recordCount)),
            (locationsOffset, () => data = ReadLocations(stream, ref position, locationsOffset, recordCount)),
            (idxOffset, () => cityIndex = ReadCityIndex(stream, reader, ref position, idxOffset, recordCount))
        };

        Array.Sort(arr, (a, b) => a.offset.CompareTo(b.offset));
        foreach (var item in arr)
            item.action();
        return new Geobase(ips, data, cityIndex);
    }

    private static IpLocation[] ReadIps(BufferedStream stream, BinaryReader reader, ref int position, int offset, int records)
    {
        Seek(stream, ref position, offset);
        var ips = new IpLocation[records];
        for (int i = 0; i < records; i++)
        {
            uint from = reader.ReadUInt32();
            uint to = reader.ReadUInt32();
            uint locationIndex = reader.ReadUInt32();
            ips[i] = new IpLocation(from, to, locationIndex);
        }

        position += records * Constants.IpLocationSize;
        return ips;
    }

    private static byte[] ReadLocations(BufferedStream stream, ref int position, int offset, int records)
    {
        Seek(stream, ref position, offset);
        var data = new byte[Constants.LocationSize * records];
        stream.Read(data, 0, data.Length);
        position += data.Length;
        return data;
    }

    private static int[] ReadCityIndex(BufferedStream stream, BinaryReader reader, ref int position, int offset, int records)
    {
        Seek(stream, ref position, offset);
        var data = new int[records];
        for (int i = 0; i < records; i++)
        {
            data[i] = reader.ReadInt32();
        }
        position += data.Length * Constants.IntSize;
        return data;
    }

    private static void Seek(Stream stream, ref int position, int offset)
    {
        if (position == offset)
            return;

        if (stream.CanSeek)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            position = offset;
            return;
        }

        var buffer = new byte[1024];
        var len = offset - position;
        while (len > 0)
        {
            var size = len > buffer.Length ? buffer.Length : len;
            stream.Read(buffer, 0, size);
            len -= size;
        }
        position = offset;
    }
}
