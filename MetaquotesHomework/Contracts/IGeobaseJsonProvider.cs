namespace MetaquotesHomework.Contracts;

public interface IGeobaseJsonProvider: IDisposable
{
    /// <summary>
    ///     Initialize geobase cache
    /// </summary>
    void Init();

    /// <summary>
    ///     Returns geobase cache
    /// </summary>
    IGeobaseJson GetGeobaseJson();
}
