using System.Security.Claims;
using JobBoard.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.API.Controllers
{
    /// <summary>
    /// Handles interest (upvote) functionality on job postings.
    /// Viewers can toggle their interest on a job.
    /// Posters can view the list of interested users for their own jobs.
    /// </summary>
    [ApiController]
    [Route("api/jobs/{jobId}/interest")]
    public class InterestController : ControllerBase
    {
        private readonly InterestService _interestService;

        public InterestController(InterestService interestService)
        {
            _interestService = interestService;
        }

        // Viewers can toggle interest on a job
        [HttpPost]
        [Authorize(Roles = "Viewer")]
        public async Task<IActionResult> ToggleInterest(int jobId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _interestService.ToggleInterestAsync(jobId, userId);

            if (result == null) return NotFound("Job not found.");
            return Ok(new { status = result });
        }

        // Posters can see who is interested in their job
        [HttpGet]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> GetInterestedUsers(int jobId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _interestService.GetInterestedUsersAsync(jobId, userId);

            if (result == null) return NotFound("Job not found or you are not the poster.");
            return Ok(result);
        }
    }
}