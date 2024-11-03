using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_rweide.Models
{
    public class Actor
    {
        [Key]
        public string IMDBActorID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string ProfilePhoto { get; set; }

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
