using System.ComponentModel.DataAnnotations;

namespace EDennis.AspNetUtils.Tests.MvcSample
{
    public partial class Song : EntityBase
    {
        [Required]
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Artist Artist { get; set; }
    }
}