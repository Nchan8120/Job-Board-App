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

        public JobsController(JobService jobService)
        {
            _jobService = jobService;
        }

        // Anyone can view jobs
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _jobService.GetAllAsync(search, page, pageSize);
            return Ok(result);
        }

        // Anyone can view a single job
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        // Only Posters can create jobs
        [HttpPost]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Create(CreateJobDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var job = await _jobService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }

        // Only Posters can edit their own jobs
        [HttpPut("{id}")]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Update(int id, UpdateJobDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var job = await _jobService.UpdateAsync(id, dto, userId);
            if (job == null) return NotFound();
            return Ok(job);
        }

        // Only Posters can delete their own jobs
        [HttpDelete("{id}")]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _jobService.DeleteAsync(id, userId);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}