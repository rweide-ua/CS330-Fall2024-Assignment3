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
using Azure.AI.OpenAI;
using System.ClientModel;
using OpenAI.Chat;

namespace Fall2024_Assignment3_rweide.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private SentimentIntensityAnalyzer analyzer;

        private readonly IConfiguration _config;
        private AzureOpenAIClient azureClient;
        private ChatClient chatClient;

        public ActorsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            azureClient = new(
                new Uri("https://fall2024-rweide-openai.openai.azure.com/"),
                new ApiKeyCredential(_config["OpenAI:Secret"]));
            chatClient = azureClient.GetChatClient("gpt-35-turbo");
            analyzer = new SentimentIntensityAnalyzer();
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
              return _context.Actor != null ? 
                          View(await _context.Actor.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Actor'  is null.");
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (_context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            var movies = await _context.MovieActor
                .Include(cs => cs.Movie)
                .Where(cs => cs.ActorId == actor.Id)
                .Select(cs => cs.Movie)
                .ToListAsync();

            // Create ChatGPT responses here
            // Get sentiment analysis here
            // Pass this to MovieDetailsViewModel
            var tweet1 = new TextSentimentPair("Hi I'm an actor and I'm really cool", 0.8f);
            var tweet2 = new TextSentimentPair("This thing is AWFUL", -0.9f);
            var tweet3 = new TextSentimentPair("I am neutral about this topic", 0.01f);

            ChatCompletion completion = chatClient.CompleteChat(new UserChatMessage("Write twenty tweets as if they were written by actor " + actor.Name + ". Make sure these are true to the actor's personality and base these off of their existing tweets. These tweets should follow the real rules of Twitter, where each tweet only contains 140 characters or less. Some tweets can have hashtags in them, but not all tweets should. PLEASE make sure to return the tweets with three $ symbols separating them for easier separation in code later. Do not add brackets at the beginning and ending. An example response should be, without brackets: [Here's a tweet$$$ Here's another tweet$$$ Here's yet another tweet]. PLEASE do not surround the enclosed tweets with quotation marks. PLEASE ensure there are no brackets at the start or end of the response. Tweets should be separated by three $ symbols. DO NOT UNDER ANY CIRCUMSTANCE GIVE A RESPONSE THAT STARTS WITH \"As an AI language model\". ONLY RESPOND WITH THE REQUESTED TEXT."));

            var aiTweets = completion.Content[0].Text;
            string[] tweetTexts = aiTweets.Split("$$$");

            // tweetTexts.Add("Today just keeps getting worse and worse");
            // tweetTexts.Add("Just got a ticket to the hottest band let's goooo! I love these guys!");
            // tweetTexts.Add("Hope you're all having an awesome day :)");
            // tweetTexts.Add("Ughhh I hate waiting in line bro");

            var tweets = new List<TextSentimentPair>();

            foreach (var tweet in tweetTexts)
            {
                var sentimentCompound = analyzer.PolarityScores(tweet).Compound;
                tweets.Add(new TextSentimentPair(tweet, sentimentCompound));
            }

            var vm = new ActorDetailsViewModel(actor, movies, tweets);

            return View(vm);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IMDBActorID,Name,Gender,Age")] Actor actor, IFormFile? photo)
        {

            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                    photo.CopyTo(memoryStream);
                    actor.Photo = memoryStream.ToArray();
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IMDBActorID,Name,Gender,Age,Photo")] Actor actor, IFormFile? photo)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            var existingActor = await _context.Actor
                .AsNoTracking().FirstOrDefaultAsync(mo => mo.Id == id);

            byte[]? existingActorPhoto = existingActor!.Photo;

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                        photo.CopyTo(memoryStream);
                        actor.Photo = memoryStream.ToArray();
                    } else
                    {
                        actor.Photo = existingActorPhoto;
                    }
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Actor == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Actor'  is null.");
            }
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
          return (_context.Actor?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
