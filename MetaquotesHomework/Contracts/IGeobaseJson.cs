namespace MetaquotesHomework.Contracts;

public interface IGeobaseJson
{
    /// <summary>
    ///     Returns index of the first location with specified city
    ///     or <c>-1</c> if there is no location matching criteria
    /// </summary>
    public int FindFirstCity(string city);

    /// <summary>
    ///     Returns <c>true</c> if location at index <paramref name="index"/> has city <paramref name="city"/>,
    ///     otherwise - <c>false</c>
    /// </summary>
    public bool CheckCity(string city, int index);

    /// <summary>
    ///     Returns index of the first location with specified IP address
    ///     or <c>-1</c> if there is no location matching criteria
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int FindLocationByIp(uint value);

    /// <summary>
    ///     Returns UTF-8 JSON representation of the location with specified index
    /// </summary>
    byte[] GetJson(int index);
}
