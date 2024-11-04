using Fall2024_Assignment3_rweide.Models;

namespace Fall2024_Assignment3_rweide.Models
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; }
        public IEnumerable<Movie> Movies { get; set; }

        // NOTE: In here is where we should get the AI-generated reviews for the movie, as well as perform the sentiment analysis?

        public ActorDetailsViewModel(Actor actor, IEnumerable<Movie> movies)
        {
            Actor = actor;
            Movies = movies;
        }
    }
}
