using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.EfCore.DataAccessLayer.Specifications;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.ApplicationLayer.Interfaces;

/// <summary>
/// Read-only data service
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="AggregateRootEntity"/></typeparam>
/// <typeparam name="TContext">Type of the <see cref="DbContext"/> to use </typeparam>
public interface IReadOnlyDataService<TEntity, TContext> : IEfCoreDataServiceBase<TContext>
    where TEntity : AggregateRootEntity where TContext : class, IEfDbContext
{
    /// <summary>
    /// Gets an entity based on given primary key values
    /// </summary>
    /// <param name="keyValues">Primary key values</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any</returns>
    Task<Result<TEntity>> GetAsync(params object[] keyValues);

    /// <summary>
    /// Gets an entity based on given primary key values and maps it to another type
    /// </summary>
    /// <param name="shouldProject">Whether to use AutoMappers ProjectTo method</param>
    /// <param name="keyValues">Primary key values</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any</returns>
    Task<Result<TGetResult>> GetAsync<TGetResult>(bool shouldProject = false, params object[] keyValues) where TGetResult : class;

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification"/>
    /// </summary>
    /// <param name="specification">Specification with query settings</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any</returns>
    Task<Result<TEntity>> GetSingleBySpecAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T}"/> and maps it to another type
    /// </summary>
    /// <param name="specification">Specification with query settings</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any</returns>
    Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(ISpecification<TEntity> specification)
        where TGetResult : class;

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T, TProjectTo}"/> and projects it to another type using AutoMappers ProjectTo method
    /// </summary>
    /// <param name="specification">Specification with query settings</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any</returns>
    Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class;

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/>
    /// </summary>
    /// <param name="specification">Specification with query settings</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/> and maps them to another type
    /// </summary>
    /// <param name="specification">Specification with query settings</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(ISpecification<TEntity> specification)
        where TGetResult : class;

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T, TProjectTo}"/> and projects them to another type using AutoMappers ProjectTo method
    /// </summary>
    /// <param name="specification">Specification with query settings</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any</returns>
    Task<Result<IReadOnlyList<TGetProjectedResult>>> GetBySpecAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class;

    /// <summary>
    /// Gets all entities and maps them to another type
    /// </summary>
    /// <param name="shouldProject">Whether to use AutoMappers ProjectTo method</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false)
        where TGetResult : class;

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync();

    /// <summary>
    /// Counts the entities with optional query parameters set by passing a <see cref="ISpecification{T}"/>
    /// </summary>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation </returns>
    Task<Result<long>> LongCountAsync(ISpecification<TEntity>? specification = null);
}
