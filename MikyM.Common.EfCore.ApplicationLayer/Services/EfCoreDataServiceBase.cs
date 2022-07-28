using AutoMapper;
using MikyM.Common.ApplicationLayer.Interfaces;
using MikyM.Common.DataAccessLayer;
using MikyM.Common.EfCore.ApplicationLayer.Interfaces;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.Utilities.Results;
using MikyM.Common.Utilities.Results.Errors;

namespace MikyM.Common.EfCore.ApplicationLayer.Services;

/// <inheritdoc cref="IEfCoreDataServiceBase{TContext}"/>
public abstract class EfCoreDataServiceBase<TContext> : IEfCoreDataServiceBase<TContext> where TContext : class, IEfDbContext
{
    /// <summary>
    /// <see cref="IMapper"/> instance.
    /// </summary>
    public IMapper Mapper { get; }
    /// <summary>
    /// Current Unit of Work.
    /// </summary>
    public IUnitOfWork<TContext> UnitOfWork { get; }
    
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="EfCoreDataServiceBase{TContext}"/>.
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/>.</param>
    /// <param name="uof">Instance of <see cref="IUnitOfWork{TContext}"/>.</param>
    protected EfCoreDataServiceBase(IMapper mapper, IUnitOfWork<TContext> uof)
    {
        Mapper = mapper;
        UnitOfWork = uof;
    }

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync(string auditUserId)
        => await ExToResultWrapAsync(async () => await UnitOfWork.CommitAsync(auditUserId).ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync()
        => await ExToResultWrapAsync(async () => await UnitOfWork.CommitAsync().ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<int>> CommitWithCountAsync(string auditUserId)
        => await ExToResultWrapAsync(async () => await UnitOfWork.CommitWithCountAsync(auditUserId).ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result<int>> CommitWithCountAsync()
        => await ExToResultWrapAsync(async () => await UnitOfWork.CommitWithCountAsync().ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result> RollbackAsync()
        => await ExToResultWrapAsync(async () => await UnitOfWork.RollbackAsync().ConfigureAwait(false));

    /// <inheritdoc />
    public virtual async Task<Result> BeginTransactionAsync()
        => await ExToResultWrapAsync(async () => await UnitOfWork.UseTransactionAsync().ConfigureAwait(false));

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
        if (_disposed) 
            return;

        if (disposing) 
            UnitOfWork.Dispose();

        _disposed = true;
    }

    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <typeparam name="TResult">Result.</typeparam>
    /// <returns>Result of the call.</returns>
    protected async Task<Result<TResult>> ExToResultWrapAsync<TResult>(Func<Task<TResult>> func)
    {
        try
        {
            return await func.Invoke().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
    
    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <returns>Result of the call.</returns>
    protected async Task<Result> ExToResultWrapAsync(Func<Task> func)
    {
        try
        {
            await func.Invoke().ConfigureAwait(false);
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
    
    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <typeparam name="TResult">Result.</typeparam>
    /// <returns>Result of the call.</returns>
    protected Result<TResult> ExToResultOfTWrap<TResult>(Func<TResult> func)
    {
        try
        {
            return func.Invoke();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
    
    /// <summary>
    /// Wraps a call in a try catch block.
    /// </summary>
    /// <param name="func">Func to wrap.</param>
    /// <typeparam name="TResult">Result.</typeparam>
    /// <returns>Result of the call.</returns>
    protected Result ExToResultWrap<TResult>(Func<TResult> func)
    {
        try
        {
            func.Invoke();
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }
}
