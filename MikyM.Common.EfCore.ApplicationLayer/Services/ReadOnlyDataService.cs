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
    where TEntity : Entity<TId>
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
    public virtual async Task<Result<TGetResult>> GetAsync<TGetResult>(object?[]? keyValues, CancellationToken cancellationToken = default) where TGetResult : class
    {       
        try
        {
            var entity = await ReadOnlyRepository.GetAsync(keyValues, cancellationToken).ConfigureAwait(false);
            if (entity is null)
                return new NotFoundError();
            
            return Mapper.Map<TGetResult>(entity);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetAsync(object?[]? keyValues, CancellationToken cancellationToken)
    {       
        try
        {
            var entity = await ReadOnlyRepository.GetAsync(keyValues, cancellationToken).ConfigureAwait(false);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification, cancellationToken);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
    {
        try
        {
            var res = await GetSingleBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
            return !res.IsDefined(out var entity) ? Result<TGetResult>.FromError(res) : Mapper.Map<TGetResult>(entity);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(
        ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default) where TGetProjectedResult : class
    {
        try
        {
            var entity = await ReadOnlyRepository.GetSingleBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
            return entity is null ? new NotFoundError() : entity;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => await ExToResultWrapAsync(async () =>
            await ReadOnlyRepository.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default) where TGetResult : class
    {
        try
        {
            var res = await GetBySpecAsync(specification, cancellationToken);
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
        ISpecification<TEntity, TGetProjectedResult> specification, CancellationToken cancellationToken = default) where TGetProjectedResult : class
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false,
        CancellationToken cancellationToken = default)
        where TGetResult : class
    {
        try
        {
            if (shouldProject)
                return Result<IReadOnlyList<TGetResult>>.FromSuccess(await ReadOnlyRepository.GetAllAsync<TGetResult>(cancellationToken).ConfigureAwait(false));

            return Result<IReadOnlyList<TGetResult>>.FromSuccess(Mapper.Map<IReadOnlyList<TGetResult>>(
                await ReadOnlyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false)));
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false));
    
    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.LongCountAsync(specification, cancellationToken).ConfigureAwait(false));
    
    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(CancellationToken cancellationToken = default)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.LongCountAsync(cancellationToken).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.AnyAsync(predicate, cancellationToken).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<Result<bool>> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => await ExToResultWrapAsync(async () => await ReadOnlyRepository.AnyAsync(specification, cancellationToken).ConfigureAwait(false));
}

/// <summary>
/// Read-only data service.
/// </summary>
/// <inheritdoc cref="IReadOnlyDataService{TEntity,TContext}"/>
[PublicAPI]
public class ReadOnlyDataService<TEntity, TContext> : ReadOnlyDataService<TEntity, long, TContext>, IReadOnlyDataService<TEntity, TContext>
    where TEntity : Entity<long>
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
    
    /// <inheritdoc />
    protected override IRepositoryBase BaseRepository => UnitOfWork.GetRepository<IReadOnlyRepository<TEntity>>();
}
