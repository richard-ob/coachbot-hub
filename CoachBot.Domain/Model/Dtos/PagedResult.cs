using System;
using System.Collections.Generic;

namespace CoachBot.Domain.Model.Dtos
{
    public class PagedResult<T>: PagedRequest where T : class
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages
        {
            get
            {
                var pageCount = (double)TotalItems / PageSize;

                return (int)Math.Ceiling(pageCount);
            }
        }        
    }
}
