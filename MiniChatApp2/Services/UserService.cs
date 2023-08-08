using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniChatApp2.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public UserService(IUserRepository userRepository, UserManager<IdentityUser<int>> userManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
        }

        /*  public Task<User> GetUserByEmail(string email)
          {
              return _userRepository.GetUserByEmailAsync(email);
          }*/

        public async Task<(bool success, object result)> RegisterUserAsync(UserRegistration request)
        {
            // Validate the request data
            if (!ValidateUserRegistrationRequest(request, out string errorMessage))
            {
                return (false, new { error = errorMessage });
            }

            // Check if the email is already registered
            var existingUser = await _userRepository.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return (false, new { error = "Email is already registered." });
            }

            // Create a new user
            var newUser = new IdentityUser<int>
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userRepository.CreateAsync(newUser, request.Password);

            if (result.Succeeded)
            {
                // Return the successful registration response
                return (true, new
                {
                    userId = newUser.Id,
                    name = request.Name,
                    email = request.Email
                });
            }
            else
            {
                // Return the registration failure response
                return (false, new { error = "Registration failed due to validation errors." });
            }
        }

        private bool ValidateUserRegistrationRequest(UserRegistration request, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errorMessage = "Email is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                errorMessage = "Name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errorMessage = "Password is required.";
                return false;
            }

            // Additional validation if needed...

            return true;
        }





        public async Task<LoginResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new ArgumentException("Invalid credentials.");
            }

            var token = GenerateJwtToken(user.Id, user.UserName, user.Email);

            return new LoginResponseDto
            {
                Token = token,
                Profile = new UserProfile
                {
                    Id = user.Id.ToString(),
                    Name = user.UserName,
                    Email = user.Email
                }
            };
        }

        /* public async Task<List<User>> GetAllUserAsync(string currentUserEmail)
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
         }*/

        private string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
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
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
          
        }
    }
}

