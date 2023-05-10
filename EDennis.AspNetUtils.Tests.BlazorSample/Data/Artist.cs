using EDennis.AspNetUtils.Tests.BlazorSample;
using System.ComponentModel.DataAnnotations;

namespace EDennis.AspNetUtils.Tests.BlazorSample
{
    public partial class Artist : EntityBase
    {
        [Required]
        public string Name { get; set; }
        public bool IsSolo { get; set; }
        public ICollection<Song> Songs { get; set; }

    }
}