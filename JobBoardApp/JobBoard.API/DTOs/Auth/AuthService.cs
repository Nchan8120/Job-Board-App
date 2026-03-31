using JobBoard.API.DTOs.Auth;
using JobBoard.API.Models;
using JobBoard.API.Repositories;

namespace JobBoard.API.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly TokenService _tokenService;

        public AuthService(UserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
                return null; // email already taken

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.CreateAsync(user);
            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}