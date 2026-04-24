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

        public AuthService(UserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
                return ServiceResult<AuthResponseDto>.Failure("Email is already registered.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.CreateAsync(user);
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
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult<AuthResponseDto>.Failure("Invalid email or password.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ServiceResult<AuthResponseDto>.Failure("Invalid email or password.");

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