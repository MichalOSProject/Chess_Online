using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Chess_Online.Server.Models.InputModels;
using Microsoft.AspNetCore.Authorization;
using Chess_Online.Server.Services.Interfaces;

namespace Chess_Online.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthService _authService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] registerUserModelInput model)
        {
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelInput model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                Console.WriteLine("finded: " + user.UserName);
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
                Console.WriteLine("Result: " + result);

                if (result.Succeeded)
                {
                    var token = await _authService.GenerateTokenAsync(user);
                    return Ok(new { token });
                }
                else if (result.IsLockedOut)
                {
                    return Unauthorized("User account locked out");
                }
                else
                {
                    return Unauthorized("Invalid login or password");
                }
            }

            return BadRequest("Invalid Data");
        }
    }
}
