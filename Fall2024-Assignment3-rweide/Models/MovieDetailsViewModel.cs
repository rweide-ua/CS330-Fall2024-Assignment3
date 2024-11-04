using Fall2024_Assignment3_rweide.Models;

namespace Fall2024_Assignment3_rweide.Models
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<Actor> Actors { get; set; }

        // NOTE: In here is where we should get the AI-generated reviews for the movie, as well as perform the sentiment analysis?

        public MovieDetailsViewModel(Movie movie, IEnumerable<Actor> actors)
        {
            Movie = movie;
            Actors = actors;
        }
    }
}
