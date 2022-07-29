using System.Linq.Expressions;
using AutoMapper;
using MikyM.Common.EfCore.ApplicationLayer.Interfaces;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.EfCore.DataAccessLayer.Specifications;
using MikyM.Common.Utilities.Results;
using MikyM.Common.Utilities.Results.Errors;

namespace MikyM.Common.EfCore.ApplicationLayer.Services;

/// <summary>
/// Read-only data service.
/// </summary>
/// <inheritdoc cref="IReadOnlyDataService{TEntity,TId,TContext}"/>
[PublicAPI]
public class ReadOnlyDataService<TEntity, TId, TContext> : EfCoreDataServiceBase<TContext>,
    IReadOnlyDataService<TEntity, TId, TContext>
    where TEntity : class, IEntity<TId>
    where TContext : class, IEfDbContext
    where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Creates a new instance of <see cref="IReadOnlyDataService{TEntity,TId,TContext}"/>.
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/>.</param>
    /// <param name="uof">Instance of <see cref="IUnitOfWork"/>.</param>
    public ReadOnlyDataService(IMapper mapper, IUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }

    /// <summary>
    /// Gets the base repository for this data service.
    /// </summary>
    protected virtual IRepositoryBase BaseRepository => UnitOfWork.GetRepository<IReadOnlyRepository<TEntity,TId>>();
    /// <summary>
    /// Gets the read-only version of the <see cref="BaseRepository"/> (essentially casts it for you).
    /// </summary>
    protected IReadOnlyRepository<TEntity,TId> ReadOnlyRepository =>
        (IReadOnlyRepository<TEntity,TId>)BaseRepository;

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetAsync<TGetResult>(bool shouldProject = false, params object[] keyValues) where TGetResult : class
    {
        try
        {
            var res = await GetAsync(keyValues).ConfigureAwait(false);
            return !res.IsDefined() ? Result<TGetResult>.FromError(res) : Mapper.Map<TGetResult>(res.Entity);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetAsync(params object[] keyValues)
    {       
        try
        {
            var entity = await ReadOnlyRepository.GetAsync(keyValues).ConfigureAwait(false);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetSingleBySpecAsync(ISpecification<TEntity> specification)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(ISpecification<TEntity> specification) where TGetResult : class
    {
        try
        {
            var res = await GetSingleBySpecAsync(specification).ConfigureAwait(false);
            return !res.IsDefined(out var entity) ? Result<TGetResult>.FromError(res) : Mapper.Map<TGetResult>(entity);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification).ConfigureAwait(false);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(ISpecification<TEntity> specification)
        => await ExToResultWrapAsync(async () =>
            await ReadOnlyRepository.GetBySpecAsync(specification).ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(
        ISpecification<TEntity> specification) where TGetResult : class
    {
        try
        {
            var res = await GetBySpecAsync(specification);
            return !res.IsDefined(out var def)
                ? Result<IReadOnlyList<TGetResult>>.FromError(res)
                : Result<IReadOnlyList<TGetResult>>.FromSuccess(Mapper.Map<IReadOnlyList<TGetResult>>(def));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetProjectedResult>>> GetBySpecAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.GetBySpecAsync(specification).ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false)
        where TGetResult : class
        => await ExToResultWrapAsync(async () =>
        {
            IReadOnlyList<TGetResult> res;
            if (shouldProject)
                res = await ReadOnlyRepository.GetAllAsync<TGetResult>().ConfigureAwait(false);
            else
                res = Mapper.Map<IReadOnlyList<TGetResult>>(
                    await ReadOnlyRepository.GetAllAsync().ConfigureAwait(false));

            return res;
        });

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync()
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.GetAllAsync().ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(ISpecification<TEntity>? specification = null)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.LongCountAsync(specification).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.AnyAsync(predicate).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(ISpecification<TEntity> specification)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.AnyAsync(specification).ConfigureAwait(false));
}

/// <summary>
/// Read-only data service.
/// </summary>
/// <inheritdoc cref="IReadOnlyDataService{TEntity,TContext}"/>
[PublicAPI]
public class ReadOnlyDataService<TEntity, TContext> : ReadOnlyDataService<TEntity, long, TContext>
    where TEntity : class, IEntity<long>
    where TContext : class, IEfDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="IReadOnlyDataService{TEntity,TContext}"/>.
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/>.</param>
    /// <param name="uof">Instance of <see cref="IUnitOfWork"/>.</param>
    public ReadOnlyDataService(IMapper mapper, IUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }
}
