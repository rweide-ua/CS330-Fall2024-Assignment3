using System;
namespace Fall2024_Assignment3_rweide.Models
{
	public class TextSentimentPair
	{
		public string Text;
		public float SentimentCompound;

		public TextSentimentPair(string text, float sentimentCompound)
		{
			Text = text;
            SentimentCompound = sentimentCompound;
		}
	}
}

