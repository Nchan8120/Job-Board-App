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
        private readonly ILogger<InterestController> _logger;

        public InterestController(InterestService interestService, ILogger<InterestController> logger)
        {
            _interestService = interestService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Viewer")]
        public async Task<IActionResult> ToggleInterest(int jobId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _interestService.ToggleInterestAsync(jobId, userId);
                if (!result.Successful)
                    return NotFound(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling interest for job {JobId}", jobId);
                return StatusCode(500, "An unexpected error occurred while processing your interest.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> GetInterestedUsers(int jobId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _interestService.GetInterestedUsersAsync(jobId, userId);
                if (!result.Successful)
                    return NotFound(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interested users for job {JobId}", jobId);
                return StatusCode(500, "An unexpected error occurred while retrieving interested users.");
            }
        }
    }
}