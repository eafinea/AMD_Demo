namespace UrlShortener.Dtos
{
    public class PaginatedResult<T>
    {
        public int TotalEntries { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<T> Data { get; set; }
    }
}
