namespace MTA.Application.Services;

/// <summary>
/// Interface for mapping service
/// </summary>
public interface IMappingService
{
    /// <summary>
    /// Maps source object to destination type
    /// </summary>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source object</param>
    /// <returns>Mapped destination object</returns>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Maps source object to destination object
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source object</param>
    /// <param name="destination">Destination object</param>
    /// <returns>Mapped destination object</returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

    /// <summary>
    /// Maps collection of source objects to collection of destination objects
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source collection</param>
    /// <returns>Mapped destination collection</returns>
    IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source);

    /// <summary>
    /// Maps collection of source objects to list of destination objects
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source collection</param>
    /// <returns>Mapped destination list</returns>
    List<TDestination> MapToList<TSource, TDestination>(IEnumerable<TSource> source);
}
