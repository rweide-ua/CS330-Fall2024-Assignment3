using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_rweide.Data;
using Fall2024_Assignment3_rweide.Models;
using VaderSharp2;
using System.Numerics;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace Fall2024_Assignment3_rweide.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private SentimentIntensityAnalyzer analyzer;

        private readonly IConfiguration _config;
        private AzureOpenAIClient azureClient;
        private ChatClient chatClient;

        public MoviesController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            azureClient = new(
                new Uri("https://fall2024-rweide-openai.openai.azure.com/"),
                new ApiKeyCredential(_config["OpenAI:Secret"]));
            chatClient = azureClient.GetChatClient("gpt-35-turbo");
            analyzer = new SentimentIntensityAnalyzer();
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
              return _context.Movie != null ? 
                          View(await _context.Movie.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            var actors = await _context.MovieActor
                .Include(cs => cs.Actor)
                .Where(cs => cs.MovieId == movie.Id)
                .Select(cs => cs.Actor)
                .ToListAsync();

            // Create ChatGPT responses here
            // Get sentiment analysis here
            // Pass this to MovieDetailsViewModel

            ChatCompletion completion = chatClient.CompleteChat(new UserChatMessage("Write ten INDIVIDUAL reviews for the movie " + movie.Name + " (" + movie.YearOfRelease.ToString() + "). Make sure these follow the existing online opinions and reviews of the movie. Each review should be of decent length, three to four sentences each. PLEASE make sure to return the review texts with @ separating them for easier separation in code later. Do not add brackets at the beginning and ending. An example response should be, without brackets: [Here's a review@ Here's another review@ Here's yet another review]. PLEASE do not surround the enclosed reviews with quotation marks. PLEASE ensure there are no brackets at the start or end of the response. Reviews should only be separated by a SINGLE at symbol, not two or three. DO NOT UNDER ANY CIRCUMSTANCE GIVE A RESPONSE THAT STARTS WITH \"As an AI language model\". ONLY RESPOND WITH THE REQUESTED TEXT."));

            var aiReviews = completion.Content[0].Text;
            string[] reviewTexts = aiReviews.Split('@');

            //var reviewTexts = new List<string>();
            //reviewTexts.Add("This movie is great!");
            //reviewTexts.Add("This movie is terrible!");
            //reviewTexts.Add("This movie is okay.");
            //reviewTexts.Add("I really liked when the guy did the thing.");
            //reviewTexts.Add("The worst writing I've ever seen in my life! I left the theater as soon as he said \"He's right behind me, isn't he?\"");

            var reviews = new List<TextSentimentPair>();

            foreach (var review in reviewTexts)
            {
                var sentimentCompound = analyzer.PolarityScores(review).Compound;
                reviews.Add(new TextSentimentPair(review, sentimentCompound));
            }

            // var review1 = new TextSentimentPair("This movie is really good!", 0.9);
            // var review2 = new TextSentimentPair("This movie is really BAD.", -0.9);
            // var review3 = new TextSentimentPair("This movie is okay. Not great but not bad.", 0.2);
            // var reviews = new List<TextSentimentPair> { review1, review2, review3 };

            var vm = new MovieDetailsViewModel(movie, actors, reviews);

            return View(vm);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IMDBMovieID,Name,Genre,YearOfRelease")] Movie movie, IFormFile? photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                    photo.CopyTo(memoryStream);
                    movie.Photo = memoryStream.ToArray();
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IMDBMovieID,Name,Genre,YearOfRelease,Photo")] Movie movie, IFormFile? photo)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            var existingMovie = await _context.Movie
                .AsNoTracking().FirstOrDefaultAsync(mo => mo.Id == id);

            byte[]? existingMoviePhoto = existingMovie!.Photo;

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                        photo.CopyTo(memoryStream);
                        movie.Photo = memoryStream.ToArray();
                    } else
                    {
                        movie.Photo = existingMoviePhoto;
                    }
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
