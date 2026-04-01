namespace JobBoard.API.Models
{
    /// <summary>
    /// Represents a Viewer's expressed interest in a job posting.
    /// A unique index on UserId and JobId ensures a user can only
    /// express interest in any given job once.
    /// </summary>
    public class Interest
    {
        public int Id { get; set; }
        public DateTime ExpressedAt { get; set; } = DateTime.UtcNow;

        // Foreign key — which user is interested
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Foreign key — which job they're interested in
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
    }
}