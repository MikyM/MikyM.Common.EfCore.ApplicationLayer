using MikyM.Common.ApplicationLayer.Pagination;
using MikyM.Common.EfCore.DataAccessLayer.Filters;
using MikyM.Common.EfCore.DataAccessLayer.Pagination;

namespace MikyM.Common.EfCore.ApplicationLayer.Pagination;

/// <summary>
/// Defines a response paginator.
/// </summary>
[PublicAPI]
public interface IResponsePaginator
{
    /// <summary>
    /// Creates a paginated response.
    /// </summary>
    /// <param name="pagedData">Data.</param>
    /// <param name="validFilter">Filter.</param>
    /// <param name="totalRecords">Number of total records.</param>
    /// <param name="route">Route.</param>
    /// <typeparam name="T">Type of data.</typeparam>
    /// <returns>A paginated response.</returns>
    PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, PaginationFilter validFilter,
        long totalRecords, string route);
}

/// <summary>
/// Response paginator.
/// </summary>
[PublicAPI]
public class ResponsePaginator : IResponsePaginator
{
    private readonly IUriService _uriService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uriService">Uri service.</param>
    public ResponsePaginator(IUriService uriService)
    {
        _uriService = uriService;
    }


    /// <inheritdoc />
    public PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, PaginationFilter validFilter,
        long totalRecords, string route)
    {
        var response = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
        var totalPages = totalRecords / (double)validFilter.PageSize;
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        response.NextPage = validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
            ? _uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route)
            : null;
        response.PreviousPage = validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
            ? _uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route)
            : null;
        response.FirstPage = _uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
        response.LastPage =
            _uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
        response.TotalPages = roundedTotalPages;
        response.TotalRecords = totalRecords;
        return response;
    }
}
