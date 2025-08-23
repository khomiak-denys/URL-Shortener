using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URL_Shortener.Domain.Entities
{
    public class UrlItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }

        public long CreatedByUserId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
