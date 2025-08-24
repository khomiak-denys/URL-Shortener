namespace URL_Shortener.Application.Dto.Url
{
    public class UrlListDto
    {
        public long Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
    }
}
