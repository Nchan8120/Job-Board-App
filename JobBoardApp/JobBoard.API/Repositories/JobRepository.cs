using JobBoard.API.Data;
using JobBoard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.API.Repositories
{
    /// <summary>
    /// Handles all database operations for the Job model.
    /// Automatically filters out jobs older than 2 months in all listing queries.
    /// </summary>
    public class JobRepository
    {
        private readonly AppDbContext _context;

        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Job>> GetAllActiveAsync(string? search, int page, int pageSize)
        {
            var cutoff = DateTime.UtcNow.AddMonths(-2);

            var query = _context.Jobs
                .Include(j => j.PostedBy)
                .Include(j => j.Interests)
                .Where(j => j.PostedDate >= cutoff && j.IsActive);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(j =>
                    j.Summary.Contains(search) ||
                    j.Body.Contains(search));

            return await query
                .OrderByDescending(j => j.PostedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? search)
        {
            var cutoff = DateTime.UtcNow.AddMonths(-2);

            var query = _context.Jobs
                .Where(j => j.PostedDate >= cutoff && j.IsActive);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(j =>
                    j.Summary.Contains(search) ||
                    j.Body.Contains(search));

            return await query.CountAsync();
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.PostedBy)
                .Include(j => j.Interests)
                    .ThenInclude(i => i.User)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<Job> CreateAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<Job> UpdateAsync(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task DeleteAsync(Job job)
        {
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
        }
    }
}