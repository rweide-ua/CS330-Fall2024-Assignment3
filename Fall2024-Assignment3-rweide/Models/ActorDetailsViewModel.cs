using Fall2024_Assignment3_rweide.Models;

namespace Fall2024_Assignment3_rweide.Models
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<TextSentimentPair> Tweets { get; set; }

        // NOTE: In here is where we should store the AI-generated tweets for the actor, as well as store the sentiment analysis
        // For sentiment analysis, we store the compound value? And from there, we sum all of the values up to come up with the overall sentiment

        public ActorDetailsViewModel(Actor actor, IEnumerable<Movie> movies, IEnumerable<TextSentimentPair> tweets)
        {
            Actor = actor;
            Movies = movies;
            Tweets = tweets;
        }
    }
}
