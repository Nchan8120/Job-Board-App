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

        public JobService(JobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<ServiceResult<object>> GetAllAsync(string? search, int page, int pageSize)
        {
            var jobs = await _jobRepository.GetAllActiveAsync(search, page, pageSize);
            var total = await _jobRepository.GetTotalCountAsync(search);

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
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
                return ServiceResult<JobResponseDto>.Failure("Job not found.");

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
            var job = new Job
            {
                Summary = dto.Summary,
                Body = dto.Body,
                PostedById = userId
            };

            await _jobRepository.CreateAsync(job);

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
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
                return ServiceResult<JobResponseDto>.Failure("Job not found.");

            if (job.PostedById != userId)
                return ServiceResult<JobResponseDto>.Failure("You are not the owner of this job.");

            job.Summary = dto.Summary;
            job.Body = dto.Body;

            await _jobRepository.UpdateAsync(job);

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
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
                return ServiceResult<bool>.Failure("Job not found.");

            if (job.PostedById != userId)
                return ServiceResult<bool>.Failure("You are not the owner of this job.");

            await _jobRepository.DeleteAsync(job);
            return ServiceResult<bool>.Success(true, "Job deleted successfully.");
        }
    }
}