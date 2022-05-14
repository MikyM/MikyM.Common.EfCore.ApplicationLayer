using AutoMapper;
using MikyM.Common.EfCore.ApplicationLayer.Interfaces;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.EfCore.DataAccessLayer.Specifications;
using MikyM.Common.Utilities.Results;
using MikyM.Common.Utilities.Results.Errors;

namespace MikyM.Common.EfCore.ApplicationLayer.Services;

/// <summary>
/// Read-only data service
/// </summary>
/// <inheritdoc cref="IReadOnlyDataService{TEntity,TContext}"/>
public class ReadOnlyDataService<TEntity, TContext> : DataServiceBase<TContext>, IReadOnlyDataService<TEntity, TContext>
    where TEntity : AggregateRootEntity where TContext : DbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="IReadOnlyDataService{TEntity,TContext}"/>
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/></param>
    /// <param name="uof">Instance of <see cref="IUnitOfWork"/></param>
    public ReadOnlyDataService(IMapper mapper, IUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }

    /// <summary>
    /// Gets the base repository for this data service
    /// </summary>
    protected virtual IRepositoryBase BaseRepository => UnitOfWork.GetRepository<ReadOnlyRepository<TEntity>>();
    /// <summary>
    /// Gets the read-only version of the <see cref="BaseRepository"/> (essentially casts it for you)
    /// </summary>
    protected ReadOnlyRepository<TEntity> ReadOnlyRepository =>
        (ReadOnlyRepository<TEntity>)BaseRepository;

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetAsync<TGetResult>(bool shouldProject = false, params object[] keyValues) where TGetResult : class
    {
        var res = await GetAsync(keyValues);
        return !res.IsDefined() ? Result<TGetResult>.FromError(new NotFoundError()) : Result<TGetResult>.FromSuccess(Mapper.Map<TGetResult>(res.Entity));
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetAsync(params object[] keyValues)
    {
        var res = await ReadOnlyRepository.GetAsync(keyValues);
        return res is null ? Result<TEntity>.FromError(new NotFoundError()) : Result<TEntity>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetSingleBySpecAsync(ISpecification<TEntity> specification)
    {
        var res = await ReadOnlyRepository.GetSingleBySpecAsync(specification);
        return res is null ? Result<TEntity>.FromError(new NotFoundError()) : Result<TEntity>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetSingleBySpecAsync<TGetResult>(ISpecification<TEntity> specification) where TGetResult : class
    {
        var res = await GetSingleBySpecAsync(specification);
        return !res.IsDefined(out var entity) ? Result<TGetResult>.FromError(new NotFoundError()) : Result<TGetResult>.FromSuccess(Mapper.Map<TGetResult>(entity));
    }

    /// <inheritdoc />
    public virtual async Task<Result<TGetProjectedResult>> GetSingleBySpecAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class
    {
        var res = await ReadOnlyRepository.GetSingleBySpecAsync(specification);
        return res is null ? Result<TGetProjectedResult>.FromError(new NotFoundError()) : Result<TGetProjectedResult>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetBySpecAsync(ISpecification<TEntity> specification)
    {
        var res = await ReadOnlyRepository.GetBySpecAsync(specification);
        return Result<IReadOnlyList<TEntity>>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetBySpecAsync<TGetResult>(
        ISpecification<TEntity> specification) where TGetResult : class
    {
        var res = await GetBySpecAsync(specification);
        return Result<IReadOnlyList<TGetResult>>.FromSuccess(Mapper.Map<IReadOnlyList<TGetResult>>(res.Entity));
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetProjectedResult>>> GetBySpecAsync<TGetProjectedResult>(ISpecification<TEntity, TGetProjectedResult> specification) where TGetProjectedResult : class
    {
        var res = await ReadOnlyRepository.GetBySpecAsync(specification);
        return Result<IReadOnlyList<TGetProjectedResult>>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false) where TGetResult : class
    {
        IReadOnlyList<TGetResult> res;
        if (shouldProject) res = await ReadOnlyRepository.GetAllAsync<TGetResult>();
        else res = Mapper.Map<IReadOnlyList<TGetResult>>(await ReadOnlyRepository.GetAllAsync());
        return Result<IReadOnlyList<TGetResult>>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync()
    {
        var res = await ReadOnlyRepository.GetAllAsync();
        return Result<IReadOnlyList<TEntity>>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<long>> LongCountAsync(ISpecification<TEntity>? specification = null)
    {
        var res = await ReadOnlyRepository.LongCountAsync(specification);
        return Result<long>.FromSuccess(res);
    }
}