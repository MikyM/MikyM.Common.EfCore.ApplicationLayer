using System.Linq.Expressions;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.EfCore.DataAccessLayer.Specifications;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.ApplicationLayer.Interfaces;

/// <summary>
/// Read-only data service.
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="IEntity{TId}"/>.</typeparam>
/// <typeparam name="TContext">Type of the <see cref="DbContext"/> to use.</typeparam>
/// <typeparam name="TId">Type of the Id of the entity.</typeparam>
[PublicAPI]
public interface IReadOnlyDataService<TEntity, TId, TContext> : IEfCoreDataServiceBase<TContext>
    where TEntity : class, IEntity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Gets an entity based on given primary key values.
    /// </summary>
    /// <param name="keyValues">Primary key values.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TEntity>> GetAsync(params object[] keyValues);

    /// <summary>
    /// Gets an entity based on given primary key values and maps it to another type.
    /// </summary>
    /// <param name="shouldProject">Whether to use AutoMappers ProjectTo method.</param>
    /// <param name="keyValues">Primary key values.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetResult>> GetAsync<TGetResult>(bool shouldProject = false, params object[] keyValues)
        where TGetResult : class;

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification"/>.
    /// </summary>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TEntity>> GetSingleBySpecAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T}"/> and maps it to another type.
    /// </summary>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(ISpecification<TEntity> specification)
        where TGetResult : class;

    /// <summary>
    /// Gets an entity based on given <see cref="ISpecification{T, TProjectTo}"/> and projects it to another type using AutoMappers ProjectTo method.
    /// </summary>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any.</returns>
    Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class;

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T}"/> and maps them to another type.
    /// </summary>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(ISpecification<TEntity> specification)
        where TGetResult : class;

    /// <summary>
    /// Gets entities based on given <see cref="ISpecification{T, TProjectTo}"/> and projects them to another type using AutoMappers ProjectTo method.
    /// </summary>
    /// <param name="specification">Specification with query settings.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetProjectedResult>>> GetBySpecAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class;

    /// <summary>
    /// Gets all entities and maps them to another type.
    /// </summary>
    /// <param name="shouldProject">Whether to use AutoMappers ProjectTo method.</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false)
        where TGetResult : class;

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any.</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync();

    /// <summary>
    /// Counts the entities with optional query parameters set by passing a <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation.</returns>
    Task<Result<long>> LongCountAsync(ISpecification<TEntity>? specification = null);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the condition.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the condition.
    /// </summary>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<Result<bool>> AnyAsync(ISpecification<TEntity> specification);
}

/// <summary>
/// Read-only data service.
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="IEntity{TId}"/>.</typeparam>
/// <typeparam name="TContext">Type of the <see cref="DbContext"/> to use.</typeparam>
[PublicAPI]
public interface IReadOnlyDataService<TEntity, TContext> : IReadOnlyDataService<TEntity, long, TContext>
    where TEntity : class, IEntity<long> where TContext : class, IEfDbContext
{
}
