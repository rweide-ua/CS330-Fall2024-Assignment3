using Fall2024_Assignment3_rweide.Models;

namespace Fall2024_Assignment3_rweide.Models
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<Actor> Actors { get; set; }
        public IEnumerable<TextSentimentPair> Reviews { get; set; }

        public readonly string OverallReviewSentiment;
        // NOTE: In here is where we should store the AI-generated reviews for the movie, as well as store the sentiment analysis
        // For sentiment analysis, we store the compound value? And from there, we sum all of the values up to come up with the overall sentiment

        public MovieDetailsViewModel(Movie movie, IEnumerable<Actor> actors, IEnumerable<TextSentimentPair> reviews)
        {
            Movie = movie;
            Actors = actors;
            Reviews = reviews;
            double SentimentSum = 0.0;
            foreach (var review in reviews)
            {
                SentimentSum += review.SentimentCompound;
            }

            SentimentSum /= reviews.Count();

            if (SentimentSum >= 0.05)
            {
                OverallReviewSentiment = "Positive";
            }
            else if (SentimentSum > -0.05 && SentimentSum < 0.05)
            {
                OverallReviewSentiment = "Neutral";
            }
            else
            {
                OverallReviewSentiment = "Negative";
            }
        }
    }
}
