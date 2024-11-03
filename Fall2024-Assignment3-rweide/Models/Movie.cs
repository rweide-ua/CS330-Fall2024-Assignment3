namespace Fall2024_Assignment3_rweide.Models
{
    public class Movie
    {
        string Name { get; set; }
        string IMDBLink { get; set; }
        string Genre { get; set; }
        int YearOfRelease { get; set; }
        string PosterURL { get; set; }

        Movie()
        {
            Name = "Movie Name";
            IMDBLink = "IMDB URL HERE";
            Genre = "Action";
            YearOfRelease = 2024;
            PosterURL = "POSTER URL HERE";
        }
    }
}
