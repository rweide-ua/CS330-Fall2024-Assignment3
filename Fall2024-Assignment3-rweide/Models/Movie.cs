﻿using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_rweide.Models
{
    public class Movie
    {
        [Key]
        public string IMDBMovieID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public int YearOfRelease { get; set; }
        [Required]
        public string PosterURL { get; set; }

        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    }
}