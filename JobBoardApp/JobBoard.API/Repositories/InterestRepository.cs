using JobBoard.API.Data;
using JobBoard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.API.Repositories
{
    /// <summary>
    /// Handles all database operations for the Interest model.
    /// Enforces the unique constraint that a user can only express interest in a job once.
    /// </summary>
    public class InterestRepository
    {
        private readonly AppDbContext _context;

        public InterestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Interest>> GetByJobIdAsync(int jobId)
        {
            return await _context.Interests
                .Include(i => i.User)
                .Where(i => i.JobId == jobId)
                .ToListAsync();
        }

        public async Task<Interest?> GetByUserAndJobAsync(int userId, int jobId)
        {
            return await _context.Interests
                .FirstOrDefaultAsync(i => i.UserId == userId && i.JobId == jobId);
        }

        public async Task<Interest> CreateAsync(Interest interest)
        {
            _context.Interests.Add(interest);
            await _context.SaveChangesAsync();
            return interest;
        }

        public async Task DeleteAsync(Interest interest)
        {
            _context.Interests.Remove(interest);
            await _context.SaveChangesAsync();
        }
    }
}