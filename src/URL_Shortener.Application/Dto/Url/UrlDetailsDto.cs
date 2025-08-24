namespace URL_Shortener.Application.Dto.Url
{
    public class UrlDetailsDto
    {
        public long Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
