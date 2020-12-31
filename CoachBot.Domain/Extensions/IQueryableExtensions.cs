using CoachBot.Domain.Attributes;
using System;
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
                // INFO: After .EF Core 3.x+ prevented auto fall back to server-side evaluation, this slight dire workaround was needed
                var sortProp = sorting.Split(' ').FirstOrDefault();
                if (sortProp != null && !sortProp.Contains('.') && Attribute.IsDefined(typeof(T).GetProperty(sortProp), typeof(ServerSideSort)))
                {
                    result.Items = query.AsQueryable().ToList().AsQueryable().OrderBy(sorting).Skip(skip).Take(pageSize).ToList();
                }
                else
                {
                    result.Items = query.OrderBy(sorting).Skip(skip).Take(pageSize).ToList();
                }
            }

            return result;
        }
    }
}