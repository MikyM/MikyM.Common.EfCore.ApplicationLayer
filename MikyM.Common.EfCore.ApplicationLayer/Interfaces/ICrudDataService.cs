using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.ApplicationLayer.Interfaces;

/// <summary>
/// CRUD data service
/// </summary>
public interface ICrudDataService<TEntity, TContext> : IReadOnlyDataService<TEntity, TContext>
    where TEntity : AggregateRootEntity where TContext : class, IEfDbContext
{
    /// <summary>
    /// Adds an entry
    /// </summary>
    /// <typeparam name="TPost">Type of the entry</typeparam>
    /// <param name="entry">Entry to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> with the Id of the newly created entity</returns>
    Task<Result<long>> AddAsync<TPost>(TPost entry, bool shouldSave = false, string? userId = null) where TPost : class;

    /// <summary>
    /// Adds a range of entries
    /// </summary>
    /// <typeparam name="TPost">Type of the entries</typeparam>
    /// <param name="entries">Entries to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> with <see cref="IEnumerable{T}"/> containing Ids of the newly created entities</returns>
    Task<Result<IEnumerable<long>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave = false,
        string? userId = null) where TPost : class;

    /// <summary>
    /// Begins updating an entity
    /// </summary>
    /// <typeparam name="TPatch">Type of the entry</typeparam>
    /// <param name="entry">Entry to attach</param>
    /// <param name="shouldSwapAttached">Whether to swap existing entity with same primary keys attached to current <see cref="DbContext"/> with new one </param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Result BeginUpdate<TPatch>(TPatch entry, bool shouldSwapAttached = false) where TPatch : class;

    /// <summary>
    /// Begins updating a range of entries
    /// </summary>
    /// <typeparam name="TPatch">Type of the entries</typeparam>
    /// <param name="entries">Entries to attach</param>
    /// <param name="shouldSwapAttached">Whether to swap existing entities with same primary keys attached to current <see cref="DbContext"/> with new ones </param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Result BeginUpdateRange<TPatch>(IEnumerable<TPatch> entries, bool shouldSwapAttached = false) where TPatch : class;

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <typeparam name="TDelete">Type of the entry</typeparam>
    /// <param name="entry">Entry to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync<TDelete>(TDelete entry, bool shouldSave = false, string? userId = null)
        where TDelete : class;

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="id">Id of the entity to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync(long id, bool shouldSave = false, string? userId = null);

    /// <summary>
    /// Deletes a range of entities
    /// </summary>
    /// <param name="entries">Entries to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteRangeAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave = false, string? userId = null)
        where TDelete : class;

    /// <summary>
    /// Deletes a range of entities
    /// </summary>
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteRangeAsync(IEnumerable<long> ids, bool shouldSave = false, string? userId = null);

    /// <summary>
    /// Disables an entity
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entry">Entry to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave = false, string? userId = null)
        where TDisable : class;

    /// <summary>
    /// Disables an entity
    /// </summary>
    /// <param name="id">Id of the entity to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync(long id, bool shouldSave = false, string? userId = null);

    /// <summary>
    /// Disables a range of entities
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entries">Entries to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false,
        string? userId = null) where TDisable : class;

    /// <summary>
    /// Disables a range of entities
    /// </summary>
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync(IEnumerable<long> ids, bool shouldSave = false, string? userId = null);
    
    /// <summary>
    /// Detaches an entry and it's children
    /// </summary>
    /// <typeparam name="TDetach">Type of the entry</typeparam>
    /// <param name="entry">Entry to detach</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    void Detach<TDetach>(TDetach entry) where TDetach : class;
}
