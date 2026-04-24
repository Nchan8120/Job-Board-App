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
        private readonly ILogger<InterestService> _logger;

        public InterestService(InterestRepository interestRepository, JobRepository jobRepository, ILogger<InterestService> logger)
        {
            _interestRepository = interestRepository;
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<List<InterestResponseDto>>> GetInterestedUsersAsync(int jobId, int userId)
        {
            _logger.LogDebug("User {UserId} requesting interested users for job {JobId}", userId, jobId);

            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                _logger.LogWarning("GetInterestedUsers failed — job {JobId} not found", jobId);
                return ServiceResult<List<InterestResponseDto>>.Failure("Job not found.");
            }

            if (job.PostedById != userId)
            {
                _logger.LogWarning("GetInterestedUsers failed — user {UserId} does not own job {JobId}", userId, jobId);
                return ServiceResult<List<InterestResponseDto>>.Failure("You are not the poster of this job.");
            }

            var interests = await _interestRepository.GetByJobIdAsync(jobId);
            _logger.LogDebug("Found {Count} interested users for job {JobId}", interests.Count, jobId);

            var data = interests.Select(i => new InterestResponseDto
            {
                Id = i.Id,
                Username = i.User.Username,
                ExpressedAt = i.ExpressedAt
            }).ToList();

            return ServiceResult<List<InterestResponseDto>>.Success(data);
        }

        public async Task<ServiceResult<object>> ToggleInterestAsync(int jobId, int userId)
        {
            _logger.LogDebug("User {UserId} toggling interest on job {JobId}", userId, jobId);

            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                _logger.LogWarning("ToggleInterest failed — job {JobId} not found", jobId);
                return ServiceResult<object>.Failure("Job not found.");
            }

            var existing = await _interestRepository.GetByUserAndJobAsync(userId, jobId);

            if (existing != null)
            {
                await _interestRepository.DeleteAsync(existing);
                _logger.LogInformation("User {UserId} removed interest from job {JobId}", userId, jobId);
                return ServiceResult<object>.Success(new { status = "removed" }, "Interest removed successfully.");
            }

            var interest = new Interest { JobId = jobId, UserId = userId };
            await _interestRepository.CreateAsync(interest);
            _logger.LogInformation("User {UserId} expressed interest in job {JobId}", userId, jobId);
            return ServiceResult<object>.Success(new { status = "added" }, "Interest added successfully.");
        }
    }
}