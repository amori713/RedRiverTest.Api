namespace RedRiverTest.Api.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        // Används av Angular som datum, funkar bra att köra DateTime här
        public DateTime PublishedDate { get; set; }
    }
}
