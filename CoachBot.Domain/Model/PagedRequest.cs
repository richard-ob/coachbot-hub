namespace CoachBot.Domain.Model
{
    public class PagedRequest
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; }

        public string SortOrder { get; set; }

        public string SortOrderFull {
            get
            {
                if (string.IsNullOrEmpty(SortBy)) return null;
                if (string.IsNullOrEmpty(SortOrder)) return $"{SortBy} ASC";

                return $"{SortBy} {SortOrder}";
            }
        }

        public int Offset => (Page - 1) * PageSize + 1;

    }
}
