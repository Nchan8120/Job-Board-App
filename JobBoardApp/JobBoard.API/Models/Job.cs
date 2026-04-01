namespace JobBoard.API.Models
{
    /// <summary>
    /// Represents a job posting. Jobs are automatically hidden from listings
    /// after 2 months from their PostedDate. IsActive can be used to manually
    /// hide a job without deleting it from the database.
    /// </summary>
    public class Job
    {
        public int Id { get; set; }
        public string Summary { get; set; } = string.Empty;   // the title
        public string Body { get; set; } = string.Empty;      // the description
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Foreign key — which user posted this job
        public int PostedById { get; set; }
        public User PostedBy { get; set; } = null!;

        public ICollection<Interest> Interests { get; set; } = new List<Interest>();
    }
}