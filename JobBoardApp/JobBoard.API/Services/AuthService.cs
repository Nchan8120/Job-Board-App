using JobBoard.API.DTOs.Auth;
using JobBoard.API.Models;
using JobBoard.API.Repositories;

namespace JobBoard.API.Services
{
    /// <summary>
    /// Contains business logic for user registration and login.
    /// Handles password hashing via BCrypt and delegates token generation to TokenService.
    /// </summary>
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserRepository userRepository, TokenService tokenService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            _logger.LogDebug("Attempting registration for email {Email}", dto.Email);

            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                _logger.LogWarning("Registration failed — email already exists: {Email}", dto.Email);
                return ServiceResult<AuthResponseDto>.Failure("Email is already registered.");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.CreateAsync(user);
            _logger.LogInformation("New user registered: {Username} ({Role})", user.Username, user.Role);

            var token = _tokenService.GenerateToken(user);

            return ServiceResult<AuthResponseDto>.Success(new AuthResponseDto
            {
                Id = user.Id,
                Token = token,
                Username = user.Username,
                Role = user.Role
            }, "Registration successful.");
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            _logger.LogDebug("Login attempt for email {Email}", dto.Email);

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed — email not found: {Email}", dto.Email);
                return ServiceResult<AuthResponseDto>.Failure("Invalid email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed — incorrect password for email: {Email}", dto.Email);
                return ServiceResult<AuthResponseDto>.Failure("Invalid email or password.");
            }

            _logger.LogInformation("User logged in: {Username}", user.Username);
            var token = _tokenService.GenerateToken(user);

            return ServiceResult<AuthResponseDto>.Success(new AuthResponseDto
            {
                Id = user.Id,
                Token = token,
                Username = user.Username,
                Role = user.Role
            }, "Login successful.");
        }
    }
}