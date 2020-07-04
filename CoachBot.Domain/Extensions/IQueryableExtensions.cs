using System.Linq;
using System.Linq.Dynamic.Core;

namespace CoachBot.Domain.Extensions
{
    public static class IQueryableExtensions
    {
        public static Model.Dtos.PagedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize, string sorting) where T : class
        {
            var result = new Model.Dtos.PagedResult<T>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = query.Count()
            };

            var skip = (page - 1) * pageSize;

            if (string.IsNullOrEmpty(sorting))
            {
                result.Items = query.Skip(skip).Take(pageSize).ToList();
            }
            else
            {
                result.Items = query.OrderBy(sorting).Skip(skip).Take(pageSize).ToList();
            }

            return result;
        }
    }
}