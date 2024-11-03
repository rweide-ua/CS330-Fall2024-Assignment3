namespace Fall2024_Assignment3_rweide.Models
{
    public class Actor
    {
        string Name { get; set; }
        string Gender { get; set; }
        int Age { get; set; }
        string IMDBLink { get; set; }
        string ProfilePhoto { get; set; }

        Actor()
        {
            Name = "Actor Name";
            Gender = "Unknown";
            Age = 32;
            IMDBLink = "IMDB LINK HERE";
            ProfilePhoto = "PHOTO LINK HERE";
        }
    }
}
