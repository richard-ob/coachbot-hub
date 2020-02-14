using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class PagedRequest
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; }

        public string SortOrder { get; set; }

        public int Offset => (Page - 1) * PageSize + 1;

    }
}
