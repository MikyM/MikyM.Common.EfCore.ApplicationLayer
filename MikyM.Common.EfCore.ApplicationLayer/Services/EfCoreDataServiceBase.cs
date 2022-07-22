using AutoMapper;
using MikyM.Common.ApplicationLayer.Interfaces;
using MikyM.Common.DataAccessLayer;
using MikyM.Common.EfCore.ApplicationLayer.Interfaces;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.ApplicationLayer.Services;

/// <inheritdoc cref="IEfCoreDataServiceBase{TContext}"/>
public abstract class EfCoreDataServiceBase<TContext> : IEfCoreDataServiceBase<TContext> where TContext : class, IEfDbContext
{
    /// <summary>
    /// <see cref="IMapper"/> instance
    /// </summary>
    public IMapper Mapper { get; }
    /// <summary>
    /// Current Unit of Work
    /// </summary>
    public IUnitOfWork<TContext> UnitOfWork { get; }
    
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="EfCoreDataServiceBase{TContext}"/>
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/></param>
    /// <param name="uof">Instance of <see cref="IUnitOfWork{TContext}"/></param>
    protected EfCoreDataServiceBase(IMapper mapper, IUnitOfWork<TContext> uof)
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

    IUnitOfWorkBase IDataServiceBase<TContext>.UnitOfWork => UnitOfWork;


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
