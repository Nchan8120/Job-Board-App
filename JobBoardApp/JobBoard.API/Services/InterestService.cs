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

        public async Task<ServiceResult<List<InterestResponseDto>>> GetInterestedUsersAsync(int jobId, int userId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
                return ServiceResult<List<InterestResponseDto>>.Failure("Job not found.");

            if (job.PostedById != userId)
                return ServiceResult<List<InterestResponseDto>>.Failure("You are not the poster of this job.");

            var interests = await _interestRepository.GetByJobIdAsync(jobId);

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
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
                return ServiceResult<object>.Failure("Job not found.");

            var existing = await _interestRepository.GetByUserAndJobAsync(userId, jobId);

            if (existing != null)
            {
                await _interestRepository.DeleteAsync(existing);
                return ServiceResult<object>.Success(new { status = "removed" }, "Interest removed successfully.");
            }

            var interest = new Interest
            {
                JobId = jobId,
                UserId = userId
            };

            await _interestRepository.CreateAsync(interest);
            return ServiceResult<object>.Success(new { status = "added" }, "Interest added successfully.");
        }
    }
}