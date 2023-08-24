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
using Google.Apis.Auth.OAuth2.Requests;
using Humanizer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NuGet.Common;
using Newtonsoft.Json;

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
        private readonly SignInManager<IdentityUser> _signInManager;
        public UsersController(SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager,RealAppContext context, IConfiguration configuration, IUserService userService, ILogger<UsersController> logger,IUserRepository userRepository)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
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
                return Ok( users );
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

       
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
        [HttpPost("social-login")]
        public async Task<ActionResult> SocialLogin(TokenModel token)
        {
            Console.WriteLine(token.id);
            
            var user = await _userService.VerifyTokenAsync(token.id);

            if (user != null)
            {
                 return Ok(user);
            }

            return Unauthorized();
        }

    }
}
