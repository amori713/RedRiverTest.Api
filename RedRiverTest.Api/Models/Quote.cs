namespace RedRiverTest.Api.Models
{
    public class Quote
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string? Author { get; set; }

        // Kopplat till inloggad användare (User.Identity.Name)
        public string UserName { get; set; } = string.Empty;
    }
}
