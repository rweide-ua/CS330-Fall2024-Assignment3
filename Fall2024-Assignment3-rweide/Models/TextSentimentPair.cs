using System;
namespace Fall2024_Assignment3_rweide.Models
{
	public class TextSentimentPair
	{
		public string Text;
		public double SentimentCompound;
		public readonly string SentimentString;

		public TextSentimentPair(string text, double sentimentCompound)
		{
			Text = text;
            SentimentCompound = sentimentCompound;
            if (SentimentCompound >= 0.05)
            {
                SentimentString = "Positive";
            }
            else if (SentimentCompound > -0.05 && SentimentCompound < 0.05)
            {
                SentimentString = "Neutral";
            }
            else
            {
                SentimentString = "Negative";
            }
        }
	}
}

