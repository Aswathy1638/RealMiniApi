using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using MiniChatApp2.Services;
using MiniChatApp2.Repositories;

namespace MiniChatApp2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase

    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly RealAppContext _context;
        private readonly string _jwtSecretKey = "your_secret_key_here";
        private readonly IConfiguration _configuration;
        public UsersController(UserManager<IdentityUser> userManager,RealAppContext context, IConfiguration configuration, IUserService userService, ILogger<UsersController> logger,IUserRepository userRepository)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [Authorize]
       [HttpGet]
      
     public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                // Retrieve the current user's email from the authentication context
                string currentUserEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                var users = await _userService.GetAllUsersAsync(currentUserEmail);
                return Ok(new { users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Users/5
        /*  [HttpGet("{id}")]
          public async Task<ActionResult<User>> GetUser(int id)
          {
              if (_context.User == null)
              {
                  return NotFound();
              }
              var user = await _context.User.FindAsync(id);

              if (user == null)
              {
                  return NotFound();
              }

              return user;
          }*/

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*  [HttpPut("{id}")]
          public async Task<IActionResult> PutUser(int id, User user)
          {
              if (id != user.Id)
              {
                  return BadRequest();
              }

              _context.Entry(user).State = EntityState.Modified;

              try
              {
                  await _context.SaveChangesAsync();
              }
              catch (DbUpdateConcurrencyException)
              {
                  if (!UserExists(id))
                  {
                      return NotFound();
                  }
                  else
                  {
                      throw;
                  }
              }

              return NoContent();
          }



          // DELETE: api/Users/5
        /*  [HttpDelete("{id}")]
          public async Task<IActionResult> DeleteUser(int id)
          {
              if (_context.User == null)
              {
                  return NotFound();
              }
              var user = await _context.User.FindAsync(id);
              if (user == null)
              {
                  return NotFound();
              }

              _context.User.Remove(user);
              await _context.SaveChangesAsync();

              return NoContent();
          }*/
        /*  [HttpPost("register")]
          public async Task<IActionResult> PostUser(User user)
          {
              // Check if the model is valid
              if (!ModelState.IsValid)
              {
                  return BadRequest(new { error = "Registration failed due to validation errors" });
              }

              var registeredUser = await _userService.RegisterUserAsync(user);

              if (registeredUser != null)
              {
                  // Registration successful, return a success response
                  return Ok(new { message = "Registration successful" });
              }
              else
              {
                  // Registration failed, return an error response
                  return BadRequest(new { error = "Failed to register user" });
              }

              // Check if the email is already registered

          }*/

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistration request)
        {
            var (success, result) = await _userService.RegisterUserAsync(request);

            if (success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }



       [HttpPost("login")]
       public async Task<IActionResult> Login(LoginDto model)
       {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Login failed due to validation errors" });
            }

            var result = await _userService.LoginAsync(model);


            return Ok(new { token = result.Token, profile = result.Profile });
        }


        //private string GenerateJwtToken(int id, string name, string email)
        //{
        //    var claims = new[] {
        //        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        //        new Claim(ClaimTypes.Name, name),
        //        new Claim(ClaimTypes.Email, email)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(
        //        _configuration["Jwt:Issuer"],
        //        _configuration["Jwt:Audience"],
        //        claims,
        //        expires: DateTime.UtcNow.AddMinutes(10),
        //        signingCredentials: signIn);


        //    string Token = new JwtSecurityTokenHandler().WriteToken(token);

        //    return Token;
        //}

        //private string HashPassword(string password)
        //{
        //    // Use a secure hashing algorithm, such as SHA-256 or bcrypt.
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return Convert.ToBase64String(hashedBytes);
        //    }
        //}
      /*  private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
      */
    }
}
