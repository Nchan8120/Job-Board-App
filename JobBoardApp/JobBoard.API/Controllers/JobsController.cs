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

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _jobService.GetAllAsync(search, page, pageSize);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _jobService.GetByIdAsync(id);
            if (!result.Successful)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Create(CreateJobDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _jobService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Update(int id, UpdateJobDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _jobService.UpdateAsync(id, dto, userId);
            if (!result.Successful)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _jobService.DeleteAsync(id, userId);
            if (!result.Successful)
                return NotFound(result.Message);

            return NoContent();
        }
    }
}