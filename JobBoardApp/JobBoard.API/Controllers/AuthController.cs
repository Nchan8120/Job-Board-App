using JobBoard.API.DTOs.Auth;
using JobBoard.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.API.Controllers
{
    /// <summary>
    /// Handles authentication endpoints for user registration and login.
    /// Returns a JWT token on success which the client uses for subsequent requests.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                if (!result.Successful)
                    return BadRequest(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email {Email}", dto.Email);
                return StatusCode(500, "An unexpected error occurred during registration.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                if (!result.Successful)
                    return Unauthorized(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", dto.Email);
                return StatusCode(500, "An unexpected error occurred during login.");
            }
        }
    }
}