using JobBoard.API.DTOs.Interest;
using JobBoard.API.Models;
using JobBoard.API.Repositories;

namespace JobBoard.API.Services
{
    /// <summary>
    /// Contains business logic for the interest/upvote feature.
    /// Toggles interest on and off for a given user and job.
    /// Restricts the interested users list to the job's original poster.
    /// </summary>
    public class InterestService
    {
        private readonly InterestRepository _interestRepository;
        private readonly JobRepository _jobRepository;

        public InterestService(InterestRepository interestRepository, JobRepository jobRepository)
        {
            _interestRepository = interestRepository;
            _jobRepository = jobRepository;
        }

        // Get all interested users for a job (for Posters to view)
        public async Task<List<InterestResponseDto>?> GetInterestedUsersAsync(int jobId, int userId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null) return null;

            // Only the poster of the job can see who's interested
            if (job.PostedById != userId) return null;

            var interests = await _interestRepository.GetByJobIdAsync(jobId);

            return interests.Select(i => new InterestResponseDto
            {
                Id = i.Id,
                Username = i.User.Username,
                ExpressedAt = i.ExpressedAt
            }).ToList();
        }

        // Toggle interest — if already interested, remove it. Otherwise add it.
        public async Task<string?> ToggleInterestAsync(int jobId, int userId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null) return null;

            var existing = await _interestRepository.GetByUserAndJobAsync(userId, jobId);

            if (existing != null)
            {
                await _interestRepository.DeleteAsync(existing);
                return "removed";
            }

            var interest = new Interest
            {
                JobId = jobId,
                UserId = userId
            };

            await _interestRepository.CreateAsync(interest);
            return "added";
        }
    }
}