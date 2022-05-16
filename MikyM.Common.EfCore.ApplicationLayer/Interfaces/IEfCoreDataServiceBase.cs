using MikyM.Common.ApplicationLayer.Interfaces;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.ApplicationLayer.Interfaces;

/// <summary>
/// Base data service for Entity Framework Core
/// </summary>
/// <typeparam name="TContext">Type that derives from <see cref="DbContext"/></typeparam>
public interface IEfCoreDataServiceBase<TContext> : IDataServiceBase<TContext> where TContext : DbContext
{
    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <returns>Task with a <see cref="Result"/> representing the async operation</returns>
    Task<Result> BeginTransactionAsync();

    /// <inheritdoc cref="IDataServiceBase{TContext}.CommitAsync(string)"/>
    /// <returns>Number of affected rows</returns>
    Task<Result<int>> CommitWithCountAsync(string auditUserId);

    /// <inheritdoc cref="IDataServiceBase{TContext}.CommitAsync()"/>
    /// <returns>Number of affected rows</returns>
    Task<Result<int>> CommitWithCountAsync();
}
