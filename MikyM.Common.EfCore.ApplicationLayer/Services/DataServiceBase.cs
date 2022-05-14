using AutoMapper;
using MikyM.Common.EfCore.ApplicationLayer.Interfaces;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.ApplicationLayer.Services;

/// <summary>
/// Base data service
/// </summary>
/// <inheritdoc cref="IDataServiceBase{TContext}"/>
public abstract class DataServiceBase<TContext> : IDataServiceBase<TContext> where TContext : DbContext
{
    /// <summary>
    /// <see cref="IMapper"/> instance
    /// </summary>
    protected readonly IMapper Mapper;
    /// <summary>
    /// <see cref="IUnitOfWork"/> instance
    /// </summary>
    protected readonly IUnitOfWork<TContext> UnitOfWork;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="DataServiceBase{TContext}"/>
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/></param>
    /// <param name="uof">Instance of <see cref="IUnitOfWork{TContext}"/></param>
    protected DataServiceBase(IMapper mapper, IUnitOfWork<TContext> uof)
    {
        Mapper = mapper;
        UnitOfWork = uof;
    }

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync(string auditUserId)
    {
        await UnitOfWork.CommitAsync(auditUserId ?? string.Empty);
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync()
    { 
        await UnitOfWork.CommitAsync();
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result<int>> CommitWithCountAsync(string auditUserId)
    {
        return await UnitOfWork.CommitWithCountAsync(auditUserId);
    }

    /// <inheritdoc />
    public virtual async Task<Result<int>> CommitWithCountAsync()
    {
        return await UnitOfWork.CommitWithCountAsync();
    }

    /// <inheritdoc />
    public virtual async Task<Result> RollbackAsync()
    {
        await UnitOfWork.RollbackAsync();
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> BeginTransactionAsync()
    {
        await UnitOfWork.UseTransactionAsync();
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public TContext Context => UnitOfWork.Context;

    // Public implementation of Dispose pattern callable by consumers.
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    /// <summary>
    /// Dispose action
    /// </summary>
    /// <param name="disposing">Whether disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) UnitOfWork.Dispose();

        _disposed = true;
    }
}