using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;

namespace MiniChatApp2.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User> GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> RegisterUserAsync(UserRegistrationDto model)
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

        // Other business logic methods, if needed

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
    }
}

