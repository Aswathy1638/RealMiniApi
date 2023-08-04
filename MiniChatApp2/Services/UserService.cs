using Microsoft.IdentityModel.Tokens;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniChatApp2.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public Task<User> GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> RegisterUserAsync(User model)
        {
            if (await _userRepository.GetUserByEmailAsync(model.Email) != null)
            {
                throw new ArgumentException("Email is already registered.");
            }
            model.Password = HashPassword(model.Password);

            // Create the user object
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password // Store the hashed password in the database.
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null || !VerifyPassword(model.Password, user.Password))
            {
                throw new ArgumentException("Invalid Credential");
            }

            var token = GenerateJwtToken(user.Id, user.Name, user.Email);

            return new LoginResponseDto
            {
               
                Token = token,
                Profile = new UserProfile
                {
                    Id = user.Id.ToString(),
                    Name = user.Name,
                    Email = user.Email
                },
                
            };
        }

        public async Task<List<User>> GetAllUserAsync(string currentUserEmail)
        {
            var users = await _userRepository.GetAllUsersAsync();

            // Exclude the current user from the list
            users = users.Where(u => u.Email != currentUserEmail).ToList();

          
            return users.Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            }).ToList();
        }

        private string HashPassword(string password)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }

        private string GenerateJwtToken(int id, string name, string email)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("name and email cannot be null or empty.");
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);


            string Token = new JwtSecurityTokenHandler().WriteToken(token);

            return Token;
        }
        private bool VerifyPassword(string password, string passwordHash)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = Convert.ToBase64String(hashedBytes);
                return string.Equals(hashedPassword, passwordHash);
            }
        }
    }
}

