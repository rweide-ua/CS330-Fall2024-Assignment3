using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_rweide.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string IMDBMovieID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public int YearOfRelease { get; set; }
        public byte[]? Photo { get; set; }

        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    }
}
