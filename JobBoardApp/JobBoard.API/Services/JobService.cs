using JobBoard.API.DTOs.Jobs;
using JobBoard.API.Models;
using JobBoard.API.Repositories;

namespace JobBoard.API.Services
{
    /// <summary>
    /// Contains business logic for job postings.
    /// Handles filtering, pagination, ownership checks, and mapping between models and DTOs.
    /// </summary>
    public class JobService
    {
        private readonly JobRepository _jobRepository;
        private readonly ILogger<JobService> _logger;

        public JobService(JobRepository jobRepository, ILogger<JobService> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<object>> GetAllAsync(string? search, int page, int pageSize)
        {
            _logger.LogDebug("Fetching jobs — page {Page}, pageSize {PageSize}, search '{Search}'", page, pageSize, search);

            var jobs = await _jobRepository.GetAllActiveAsync(search, page, pageSize);
            var total = await _jobRepository.GetTotalCountAsync(search);

            _logger.LogDebug("Returned {Count} jobs out of {Total}", jobs.Count, total);

            var items = jobs.Select(j => new JobResponseDto
            {
                Id = j.Id,
                Summary = j.Summary,
                Body = j.Body,
                PostedDate = j.PostedDate,
                PostedBy = j.PostedBy.Username,
                PostedById = j.PostedById,
                InterestCount = j.Interests.Count
            });

            return ServiceResult<object>.Success(new
            {
                items,
                totalCount = total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize)
            });
        }

        public async Task<ServiceResult<JobResponseDto>> GetByIdAsync(int id)
        {
            _logger.LogDebug("Fetching job {JobId}", id);

            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                _logger.LogWarning("Job {JobId} not found", id);
                return ServiceResult<JobResponseDto>.Failure("Job not found.");
            }

            return ServiceResult<JobResponseDto>.Success(new JobResponseDto
            {
                Id = job.Id,
                Summary = job.Summary,
                Body = job.Body,
                PostedDate = job.PostedDate,
                PostedBy = job.PostedBy.Username,
                PostedById = job.PostedById,
                InterestCount = job.Interests.Count
            });
        }

        public async Task<ServiceResult<JobResponseDto>> CreateAsync(CreateJobDto dto, int userId)
        {
            _logger.LogDebug("User {UserId} creating job '{Summary}'", userId, dto.Summary);

            var job = new Job
            {
                Summary = dto.Summary,
                Body = dto.Body,
                PostedById = userId
            };

            await _jobRepository.CreateAsync(job);
            _logger.LogInformation("Job {JobId} created by user {UserId}", job.Id, userId);

            return ServiceResult<JobResponseDto>.Success(new JobResponseDto
            {
                Id = job.Id,
                Summary = job.Summary,
                Body = job.Body,
                PostedDate = job.PostedDate,
                PostedById = job.PostedById,
                PostedBy = string.Empty
            }, "Job created successfully.");
        }

        public async Task<ServiceResult<JobResponseDto>> UpdateAsync(int id, UpdateJobDto dto, int userId)
        {
            _logger.LogDebug("User {UserId} attempting to update job {JobId}", userId, id);

            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                _logger.LogWarning("Update failed — job {JobId} not found", id);
                return ServiceResult<JobResponseDto>.Failure("Job not found.");
            }

            if (job.PostedById != userId)
            {
                _logger.LogWarning("Update failed — user {UserId} does not own job {JobId}", userId, id);
                return ServiceResult<JobResponseDto>.Failure("You are not the owner of this job.");
            }

            job.Summary = dto.Summary;
            job.Body = dto.Body;

            await _jobRepository.UpdateAsync(job);
            _logger.LogInformation("Job {JobId} updated by user {UserId}", id, userId);

            return ServiceResult<JobResponseDto>.Success(new JobResponseDto
            {
                Id = job.Id,
                Summary = job.Summary,
                Body = job.Body,
                PostedDate = job.PostedDate,
                PostedBy = job.PostedBy.Username,
                PostedById = job.PostedById,
                InterestCount = job.Interests.Count
            }, "Job updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id, int userId)
        {
            _logger.LogDebug("User {UserId} attempting to delete job {JobId}", userId, id);

            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                _logger.LogWarning("Delete failed — job {JobId} not found", id);
                return ServiceResult<bool>.Failure("Job not found.");
            }

            if (job.PostedById != userId)
            {
                _logger.LogWarning("Delete failed — user {UserId} does not own job {JobId}", userId, id);
                return ServiceResult<bool>.Failure("You are not the owner of this job.");
            }

            await _jobRepository.DeleteAsync(job);
            _logger.LogInformation("Job {JobId} deleted by user {UserId}", id, userId);

            return ServiceResult<bool>.Success(true, "Job deleted successfully.");
        }
    }
}