using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedRiverTest.Api.Models;

namespace RedRiverTest.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        // In-memory "databas"
        private static readonly List<Book> _books = new();
        private static int _nextBookId = 1;

        // GET: api/books
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            return _books.OrderBy(b => b.Title).ToList();
        }

        // GET: api/books/5
        [HttpGet("{id:int}")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);

            if (book == null)
                return NotFound();

            return book;
        }

        // POST: api/books
        [HttpPost]
        public ActionResult<Book> CreateBook(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
                return BadRequest("Title is required");

            book.Id = _nextBookId++;
            _books.Add(book);

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: api/books/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateBook(int id, Book updatedBook)
        {
            if (id != updatedBook.Id)
                return BadRequest("Id in URL and body do not match");

            var existing = _books.FirstOrDefault(b => b.Id == id);
            if (existing == null)
                return NotFound();

            existing.Title = updatedBook.Title;
            existing.Author = updatedBook.Author;
            existing.PublishedDate = updatedBook.PublishedDate;

            return NoContent();
        }

        // DELETE: api/books/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();

            _books.Remove(book);

            return NoContent();
        }
    }
}
