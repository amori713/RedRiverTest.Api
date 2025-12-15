namespace RedRiverTest.Api.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        // Vi använder detta som vanligt lösenord — plaintext, enkelt för uppgiften
        public string Password { get; set; } = null!;
    }
}
