using System.Security.Claims;
using JobBoard.API.DTOs.Jobs;
using JobBoard.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.API.Controllers
{
    /// <summary>
    /// Handles CRUD endpoints for job postings.
    /// Public endpoints allow anyone to view jobs.
    /// Create, edit, and delete are restricted to authenticated Posters who own the job.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly JobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(JobService jobService, ILogger<JobsController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _jobService.GetAllAsync(search, page, pageSize);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job listings");
                return StatusCode(500, "An unexpected error occurred while retrieving jobs.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _jobService.GetByIdAsync(id);
                if (!result.Successful)
                    return NotFound(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobId}", id);
                return StatusCode(500, "An unexpected error occurred while retrieving the job.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Create(CreateJobDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _jobService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job");
                return StatusCode(500, "An unexpected error occurred while creating the job.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Update(int id, UpdateJobDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _jobService.UpdateAsync(id, dto, userId);
                if (!result.Successful)
                    return NotFound(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId}", id);
                return StatusCode(500, "An unexpected error occurred while updating the job.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _jobService.DeleteAsync(id, userId);
                if (!result.Successful)
                    return NotFound(result.Message);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId}", id);
                return StatusCode(500, "An unexpected error occurred while deleting the job.");
            }
        }
    }
}