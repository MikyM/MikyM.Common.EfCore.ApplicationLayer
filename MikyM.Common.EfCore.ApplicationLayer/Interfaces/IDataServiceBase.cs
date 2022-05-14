using MikyM.Common.ApplicationLayer.Interfaces;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.ApplicationLayer.Interfaces;

/// <summary>
/// Base data service
/// </summary>
/// <typeparam name="TContext">Type that derives from <see cref="DbContext"/></typeparam>
public interface IDataServiceBase<TContext> : IDataServiceBase where TContext : DbContext
{
    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <returns>Task with a <see cref="Result"/> representing the async operation</returns>
    Task<Result> BeginTransactionAsync();
    
    /// <summary>
    /// Current <see cref="DbContext"/>
    /// </summary>
    TContext Context { get; }

    /// <inheritdoc cref="IDataServiceBase.CommitAsync(string)"/>
    /// <returns>Number of affected rows</returns>
    Task<Result<int>> CommitWithCountAsync(string auditUserId);

    /// <inheritdoc cref="IDataServiceBase.CommitAsync()"/>
    /// <returns>Number of affected rows</returns>
    Task<Result<int>> CommitWithCountAsync();
}
