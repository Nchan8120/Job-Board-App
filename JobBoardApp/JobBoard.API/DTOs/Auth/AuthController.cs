using JobBoard.API.DTOs.Auth;
using JobBoard.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return BadRequest("Email is already registered.");

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized("Invalid email or password.");

            return Ok(result);
        }
    }
}