using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedRiverTest.Api.Models;

namespace RedRiverTest.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        // In-memory "databas"
        private static readonly List<Quote> _quotes = new();
        private static int _nextQuoteId = 1;

        private string? GetUserName()
        {
            return User.Identity?.Name
                   ?? User.FindFirstValue(ClaimTypes.Name);
        }

        // GET: api/quotes
        [HttpGet]
        public ActionResult<IEnumerable<Quote>> GetMyQuotes()
        {
            var userName = GetUserName();
            if (userName == null)
                return Unauthorized();

            var quotes = _quotes
                .Where(q => q.UserName == userName)
                .OrderBy(q => q.Id)
                .Take(5)
                .ToList();

            return quotes;
        }

        // GET: api/quotes/5
        [HttpGet("{id:int}")]
        public ActionResult<Quote> GetQuote(int id)
        {
            var userName = GetUserName();
            if (userName == null)
                return Unauthorized();

            var quote = _quotes
                .FirstOrDefault(q => q.Id == id && q.UserName == userName);

            if (quote == null)
                return NotFound();

            return quote;
        }

        // POST: api/quotes
        [HttpPost]
        public ActionResult<Quote> CreateQuote(Quote quote)
        {
            var userName = GetUserName();
            if (userName == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(quote.Text))
                return BadRequest("Quote text is required");

            quote.Id = _nextQuoteId++;
            quote.UserName = userName;

            _quotes.Add(quote);

            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
        }

        // PUT: api/quotes/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateQuote(int id, Quote updated)
        {
            var userName = GetUserName();
            if (userName == null)
                return Unauthorized();

            if (id != updated.Id)
                return BadRequest("Id in URL and body do not match");

            var existing = _quotes
                .FirstOrDefault(q => q.Id == id && q.UserName == userName);

            if (existing == null)
                return NotFound();

            existing.Text = updated.Text;
            existing.Author = updated.Author;

            return NoContent();
        }

        // DELETE: api/quotes/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteQuote(int id)
        {
            var userName = GetUserName();
            if (userName == null)
                return Unauthorized();

            var quote = _quotes
                .FirstOrDefault(q => q.Id == id && q.UserName == userName);

            if (quote == null)
                return NotFound();

            _quotes.Remove(quote);

            return NoContent();
        }
    }
}
