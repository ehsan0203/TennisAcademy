using AutoMapper;

namespace MTA.Application.Services;

/// <summary>
/// Implementation of mapping service using AutoMapper
/// </summary>
public class MappingService : IMappingService
{
    private readonly IMapper _mapper;

    public MappingService(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Maps source object to destination type
    /// </summary>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source object</param>
    /// <returns>Mapped destination object</returns>
    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }

    /// <summary>
    /// Maps source object to destination object
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source object</param>
    /// <param name="destination">Destination object</param>
    /// <returns>Mapped destination object</returns>
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return _mapper.Map(source, destination);
    }

    /// <summary>
    /// Maps collection of source objects to collection of destination objects
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source collection</param>
    /// <returns>Mapped destination collection</returns>
    public IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source)
    {
        return _mapper.Map<IEnumerable<TDestination>>(source);
    }

    /// <summary>
    /// Maps collection of source objects to list of destination objects
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">Source collection</param>
    /// <returns>Mapped destination list</returns>
    public List<TDestination> MapToList<TSource, TDestination>(IEnumerable<TSource> source)
    {
        return _mapper.Map<List<TDestination>>(source);
    }
}
