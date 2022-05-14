using AutoMapper;
using MikyM.Common.EfCore.ApplicationLayer.Interfaces;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.Utilities.Results;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace MikyM.Common.EfCore.ApplicationLayer.Services;

/// <summary>
/// CRUD data service
/// </summary>
/// <inheritdoc cref="ICrudDataService{TEntity,TContext}"/>
public class CrudDataService<TEntity, TContext> : ReadOnlyDataService<TEntity, TContext>, ICrudDataService<TEntity, TContext>
    where TEntity : AggregateRootEntity where TContext : DbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="CrudDataService{TEntity,TContext}"/>
    /// </summary>
    /// <param name="mapper">Mapper instance</param>
    /// <param name="uof">Unit of work instance</param>
    public CrudDataService(IMapper mapper, IUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }

    /// <inheritdoc />
    protected override IRepositoryBase BaseRepository => UnitOfWork.GetRepository<Repository<TEntity>>();
    /// <summary>
    /// Gets the CRUD version of the <see cref="BaseRepository"/> (essentially casts it for you)
    /// </summary>
    protected Repository<TEntity> Repository => (Repository<TEntity>)BaseRepository;


    /// <inheritdoc />
    public virtual async Task<Result<long>> AddAsync<TPost>(TPost entry, bool shouldSave = false, string? userId = null) where TPost : class
    {
        if (entry  is null) throw new ArgumentNullException(nameof(entry));

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

        if (!shouldSave) return 0;

        await CommitAsync(userId);
        return Result<long>.FromSuccess(entity.Id);
    }

    /// <inheritdoc />
    public virtual async Task<Result<IEnumerable<long>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries,
        bool shouldSave = false, string? userId = null) where TPost : class
    {
        if (entries  is null) throw new ArgumentNullException(nameof(entries));

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

        if (!shouldSave) return new List<long>();

        await CommitAsync(userId);
        return Result<IEnumerable<long>>.FromSuccess(entities.Select(e => e.Id).ToList());
    }

    /// <inheritdoc />
    public virtual Result BeginUpdate<TPatch>(TPatch entry, bool shouldSwapAttached = false) where TPatch : class
    {
        switch (entry)
        {
            case null:
                throw new ArgumentNullException(nameof(entry));
            case TEntity rootEntity:
                Repository.BeginUpdate(rootEntity, shouldSwapAttached);
                break;
            default:
                Repository.BeginUpdate(Mapper.Map<TEntity>(entry), shouldSwapAttached);
                break;
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual Result BeginUpdateRange<TPatch>(IEnumerable<TPatch> entries, bool shouldSwapAttached = false) where TPatch : class
    {
        switch (entries)
        {
            case null:
                throw new ArgumentNullException(nameof(entries));
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

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync<TDelete>(TDelete entry, bool shouldSave = false, string? userId = null) where TDelete : class
    {
        switch (entry)
        {
            case null:
                throw new ArgumentNullException(nameof(entry));
            case TEntity rootEntity:
                Repository.Delete(rootEntity);
                break;
            default:
                Repository.Delete(Mapper.Map<TEntity>(entry));
                break;
        }

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync(long id, bool shouldSave = false, string? userId = null)
    {
        Repository.Delete(id);

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync(IEnumerable<long> ids, bool shouldSave = false, string? userId = null)
    {
        if (ids  is null) throw new ArgumentNullException(nameof(ids));

        Repository.DeleteRange(ids);

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync<TDelete>(IEnumerable<TDelete> entries, bool shouldSave = false, string? userId = null)
        where TDelete : class
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

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync(long id, bool shouldSave = false, string? userId = null)
    {
        await Repository
            .DisableAsync(id);

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave = false, string? userId = null) where TDisable : class
    {
        switch (entry)
        {
            case null:
                throw new ArgumentNullException(nameof(entry));
            case TEntity rootEntity:
                Repository.Disable(rootEntity);
                break;
            default:
                Repository.Disable(Mapper.Map<TEntity>(entry));
                break;
        }

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync(IEnumerable<long> ids, bool shouldSave = false, string? userId = null)
    {
        if (ids  is null) throw new ArgumentNullException(nameof(ids));

        await Repository
            .DisableRangeAsync(ids);

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false, string? userId = null)
        where TDisable : class
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

        if (shouldSave) 
            await CommitAsync(userId);

        return Result.FromSuccess();
    }
}