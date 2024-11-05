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

namespace Fall2024_Assignment3_rweide.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private SentimentIntensityAnalyzer analyzer;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
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

            var reviewTexts = new List<string>();
            reviewTexts.Add("This movie is great!");
            reviewTexts.Add("This movie is terrible!");
            reviewTexts.Add("This movie is okay.");
            reviewTexts.Add("I really liked when the guy did the thing.");
            reviewTexts.Add("The worst writing I've ever seen in my life! I left the theater as soon as he said \"He's right behind me, isn't he?\"");

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
        public async Task<IActionResult> Create([Bind("Id,IMDBMovieID,Name,Genre,YearOfRelease,PosterURL")] Movie movie)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,IMDBMovieID,Name,Genre,YearOfRelease,PosterURL")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
