namespace JobBoard.API.Models
{
    /// <summary>
    /// Represents a registered user. Role is either "Poster" or "Viewer"
    /// which determines what actions they can perform in the application.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Poster" or "Viewer"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<Interest> Interests { get; set; } = new List<Interest>();
    }
}