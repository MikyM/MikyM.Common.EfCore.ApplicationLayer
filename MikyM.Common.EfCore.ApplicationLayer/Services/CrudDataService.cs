using AutoMapper;
using MikyM.Common.Domain.Entities.Base;
using MikyM.Common.EfCore.ApplicationLayer.Interfaces;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.Utilities.Results;
using MikyM.Common.Utilities.Results.Errors;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace MikyM.Common.EfCore.ApplicationLayer.Services;

/// <summary>
/// CRUD data service.
/// </summary>
/// <inheritdoc cref="ICrudDataService{TEntity,TContext}"/>
[PublicAPI]
public class CrudDataService<TEntity, TContext> : ReadOnlyDataService<TEntity, TContext>, ICrudDataService<TEntity, TContext>
    where TEntity : class, IAggregateRootEntity where TContext : class, IEfDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="CrudDataService{TEntity,TContext}"/>.
    /// </summary>
    /// <param name="mapper">Mapper instance.</param>
    /// <param name="uof">Unit of work instance.</param>
    public CrudDataService(IMapper mapper, IUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }

    /// <inheritdoc />
    protected override IRepositoryBase BaseRepository => UnitOfWork.GetRepository<IRepository<TEntity>>();
    /// <summary>
    /// Gets the CRUD version of the <see cref="BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected IRepository<TEntity> Repository => (IRepository<TEntity>)BaseRepository;


    /// <inheritdoc />
    public virtual async Task<Result<long>> AddAsync<TPost>(TPost entry, bool shouldSave = false, string? userId = null)
        where TPost : class
        => await ExToResultWrapAsync(async () =>
        {
            TEntity entity;
            if (entry is TEntity rootEntity)
            {
                entity = rootEntity;
                Repository.Add(entity);
            }
            else
            {
                entity = Mapper.Map<TEntity>(entry);
                Repository.Add(entity);
            }

            if (!shouldSave)
                return 0;
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);

            return entity.Id;
        });

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<long>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries,
        bool shouldSave = false, string? userId = null) where TPost : class
        => await ExToResultWrapAsync(async () =>
        {
            List<TEntity> entities;

            if (entries is IEnumerable<TEntity> rootEntities)
            {
                entities = rootEntities.ToList();
                Repository.AddRange(entities);
            }
            else
            {
                entities = Mapper.Map<List<TEntity>>(entries);
                Repository.AddRange(entities);
            }

            if (!shouldSave)
                return new List<long>().AsReadOnly();

            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);

            return (IReadOnlyList<long>)entities.Select(e => e.Id).ToList().AsReadOnly();
        });


    /// <inheritdoc />
    public virtual Result BeginUpdate<TPatch>(TPatch entry, bool shouldSwapAttached = false) where TPatch : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    return new ArgumentNullError(nameof(entry));
                case TEntity rootEntity:
                    Repository.BeginUpdate(rootEntity, shouldSwapAttached);
                    break;
                default:
                    Repository.BeginUpdate(Mapper.Map<TEntity>(entry), shouldSwapAttached);
                    break;
            }

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual Result BeginUpdateRange<TPatch>(IEnumerable<TPatch> entries, bool shouldSwapAttached = false) where TPatch : class
    {
        try
        {
            switch (entries)
            {
                case null:
                    return new ArgumentNullError(nameof(entries));
                case IEnumerable<TEntity> rootEntities:
                    Repository.BeginUpdateRange(rootEntities, shouldSwapAttached);
                    break;
                default:
                    Repository
                        .BeginUpdateRange(Mapper.Map<IEnumerable<TEntity>>(entries), shouldSwapAttached);
                    break;
            }

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync<TDelete>(TDelete entry, bool shouldSave = false, string? userId = null) where TDelete : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    return new ArgumentNullError(nameof(entry));
                case TEntity rootEntity:
                    Repository.Delete(rootEntity);
                    break;
                default:
                    Repository.Delete(Mapper.Map<TEntity>(entry));
                    break;
            }

            if (!shouldSave) 
                return Result.FromSuccess();
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);
        
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync(long id, bool shouldSave = false, string? userId = null)
    {
        try
        {
            Repository.Delete(id);

            if (!shouldSave) 
                return Result.FromSuccess();
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);
        
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync(IEnumerable<long> ids, bool shouldSave = false, string? userId = null)
    {
        try
        {
            Repository.DeleteRange(ids);

            if (!shouldSave) 
                return Result.FromSuccess();
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave = false, string? userId = null)
        where TDelete : class
    {
        try
        {
            switch (entries)
            {
                case null:
                    throw new ArgumentNullException(nameof(entries));
                case IEnumerable<TEntity> rootEntities:
                    Repository.DeleteRange(rootEntities);
                    break;
                default:
                    Repository
                        .DeleteRange(Mapper.Map<IEnumerable<TEntity>>(entries));
                    break;
            }

            if (!shouldSave) 
                return Result.FromSuccess();
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);
        
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync(long id, bool shouldSave = false, string? userId = null)
    {
        try
        {
            await Repository
                .DisableAsync(id);

            if (!shouldSave) 
                return Result.FromSuccess();
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);
        
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave = false, string? userId = null) where TDisable : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    return new ArgumentNullError(nameof(entry));
                case TEntity rootEntity:
                    Repository.Disable(rootEntity);
                    break;
                default:
                    Repository.Disable(Mapper.Map<TEntity>(entry));
                    break;
            }

            if (!shouldSave) 
                return Result.FromSuccess();
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync(IEnumerable<long> ids, bool shouldSave = false, string? userId = null)
    {
        try
        {
            await Repository
                .DisableRangeAsync(ids);

            if (!shouldSave)
                return Result.FromSuccess();

            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public Result Detach<TDetach>(TDetach entry) where TDetach : class
    {
        try
        {
            switch (entry)
            {
                case null:
                    throw new ArgumentNullException(nameof(entry));
                case TEntity rootEntity:
                    Repository.Detach(rootEntity);
                    break;
                default:
                    Repository.Detach(Mapper.Map<TEntity>(entry));
                    break;
            }
            
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false, string? userId = null)
        where TDisable : class
    {
        try
        {
            switch (entries)
            {
                case null:
                    throw new ArgumentNullException(nameof(entries));
                case IEnumerable<TEntity> rootEntities:
                    Repository.DisableRange(rootEntities);
                    break;
                default:
                    Repository
                        .DisableRange(Mapper.Map<IEnumerable<TEntity>>(entries));
                    break;
            }

            if (!shouldSave) 
                return Result.FromSuccess();
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);
        
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
}
