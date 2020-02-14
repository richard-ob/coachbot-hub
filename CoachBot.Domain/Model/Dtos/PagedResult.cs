using System.Collections.Generic;

namespace CoachBot.Domain.Model.Dtos
{
    public class PagedResult<T> where T : class
    {
        public List<T> Items { get; set; }

        public PagedRequest PagedRequest { get; set; }

        public int TotalItems { get; set; }
    }
}
