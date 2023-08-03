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
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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


        //public async Task<LoginResult> LoginAsync(string email, string password)
        //{
        //    var user = await _userRepository.GetUserByEmailAsync(email);

        //    if (user == null || !VerifyPassword(password, user.Password))
        //    {
        //        return new LoginResult { Success = false, Error = LoginResultError.InvalidCredentials };
        //    }

        //    var token = GenerateJwtToken(user.Id, user.Name, user.Email);

        //    return new LoginResult
        //    {
        //        Success = true,
        //        Token = token,
        //        Profile = new User
        //        {
        //            Id = user.Id,
        //            Name = user.Name,
        //            Email = user.Email
        //        },
        //        Error = LoginResultError.None
        //    };
        //}

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

