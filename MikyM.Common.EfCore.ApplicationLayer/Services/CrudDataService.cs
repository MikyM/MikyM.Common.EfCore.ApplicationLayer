﻿using AutoMapper;
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
/// <inheritdoc cref="ICrudDataService{TEntity,TId,TContext}"/>
[PublicAPI]
public class CrudDataService<TEntity, TId, TContext> : ReadOnlyDataService<TEntity, TId, TContext>,
    ICrudDataService<TEntity, TId, TContext>
    where TEntity : class, IEntity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Creates a new instance of <see cref="CrudDataService{TEntity,TId,TContext}"/>.
    /// </summary>
    /// <param name="mapper">Mapper instance.</param>
    /// <param name="uof">Unit of work instance.</param>
    public CrudDataService(IMapper mapper, IUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }

    /// <inheritdoc />
    protected override IRepositoryBase BaseRepository => UnitOfWork.GetRepository<IRepository<TEntity,TId>>();
    /// <summary>
    /// Gets the CRUD version of the <see cref="BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected IRepository<TEntity,TId> Repository => (IRepository<TEntity,TId>)BaseRepository;


    /// <inheritdoc />
    public virtual async Task<Result<TId?>> AddAsync<TPost>(TPost entry, bool shouldSave = false, string? userId = null)
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
                return default;
        
            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);

            return entity.Id;
        });

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TId>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries,
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
                return new List<TId>().AsReadOnly();

            if (userId is null)
                _ = await CommitAsync();
            else
                _ = await CommitAsync(userId);

            return (IReadOnlyList<TId>)entities.Select(e => e.Id).ToList().AsReadOnly();
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
    public virtual async Task<Result> DeleteAsync(TId id, bool shouldSave = false, string? userId = null)
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
    public virtual async Task<Result> DeleteRangeAsync(IEnumerable<TId> ids, bool shouldSave = false, string? userId = null)
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
    public virtual async Task<Result> DisableAsync(TId id, bool shouldSave = false, string? userId = null)
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
    public virtual async Task<Result> DisableRangeAsync(IEnumerable<TId> ids, bool shouldSave = false, string? userId = null)
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
    public virtual Result Detach<TDetach>(TDetach entry) where TDetach : class
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

/// <summary>
/// CRUD data service.
/// </summary>
/// <inheritdoc cref="ICrudDataService{TEntity,TContext}"/>
[PublicAPI]
public class CrudDataService<TEntity, TContext> : CrudDataService<TEntity, long, TContext>, ICrudDataService<TEntity, TContext>
    where TEntity : class, IEntity<long> where TContext : class, IEfDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="CrudDataService{TEntity,TContext}"/>.
    /// </summary>
    /// <param name="mapper">Mapper instance.</param>
    /// <param name="uof">Unit of work instance.</param>
    public CrudDataService(IMapper mapper, IUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }
}
