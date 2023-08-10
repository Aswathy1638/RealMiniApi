using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniChatApp2.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        IHttpClientFactory _httpClientFactory;

        public UserService(SignInManager<IdentityUser> signInManager,IHttpClientFactory httpClientFactory, IUserRepository userRepository, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _signInManager = signInManager;
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
            var newUser = new IdentityUser
            {
                UserName = request.Name,
                Email = request.Email
            };

            var result = await _userRepository.CreateAsync(newUser, request.Password);

            if (result.Succeeded)
            {
                // Return the successful registration response
                return (true, new
                {
                   id=newUser.Id,
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

         return true;
        }





        public async Task<LoginResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new ArgumentException("Invalid credentials.");
            }

            var token = GenerateJwtToken(user.Id,user.UserName,user.Email);
            await _userManager.AddLoginAsync(user, new UserLoginInfo("PostMan", user.Id, "PostMan"));

            return new LoginResponseDto
            {
                Token = token,
                Profile = new UserProfile
                {
                    Name = user.UserName,
                    Email = user.Email
                }
            };

        }


        //public async Task<List<User>> GetAllUserAsync(string currentUserEmail)
        // {
        //     var users = await _userRepository.GetAllUsersAsync();

        //     // Exclude the current user from the list
        //     users = users.Where(u => u.Email != currentUserEmail).ToList();


        //     return users.Select(u => new User
        //     {
        //         Id = u.Id,
        //         Name = u.Name,
        //         Email = u.Email
        //     }).ToList();
        // }
        public async Task<LoginResponseDto> VerifyTokenAsync(string tokenId)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
            settings.Audience = new[] { "741527376978-3ednvlp0982shao300v82o9umag8re9n.apps.googleusercontent.com" }; // Replace with your actual Google Client ID

            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(tokenId, settings);

                if (payload.EmailVerified)
                {
                    IdentityUser user = await _userManager.FindByEmailAsync(payload.Email);

                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = payload.GivenName,
                            Email = payload.Email,
                            EmailConfirmed = true 
                        };

                        await _userManager.CreateAsync(user);
                    }
                    string jwtToken = GenerateJwtToken(user);
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));

                    var loginResponse = new LoginResponseDto
                    {
                        Token = jwtToken,
                        Profile = new UserProfile
                        {
                            Id = user.Id,
                            Name = user.UserName,
                            Email = user.Email
                        }
                    };

                    return loginResponse;
                }
            }
            catch (Exception ex)
            {
                // Handle token validation exception
                throw new Exception("Invalid token: " + ex.Message);
            }

            return null;
        }
        public async Task<List<UserProfile>> GetAllUsersAsync(string currentUserEmail)
        {
            return await _userRepository.GetAllUsersAsync(currentUserEmail);
        }

        private string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
        }


        private string GenerateJwtToken(string id, string name, string email)
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
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(10),
                signingCredentials: signin);


            string Token = new JwtSecurityTokenHandler().WriteToken(token);

            return Token;
        }
        private string GenerateJwtToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
          
        }
    }
}

