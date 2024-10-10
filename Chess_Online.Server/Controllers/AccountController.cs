using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Chess_Online.Server.Models.InputModels;
using Microsoft.AspNetCore.Authorization;
using Chess_Online.Server.Services.Interfaces;
using Chess_Online.Server.Data.Entity;

namespace Chess_Online.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
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
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            user.PhotoUrl = "";
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
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

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
        [Authorize]
        [HttpPost("passwordReset")]
        public async Task<IActionResult> resetPassword([FromBody] resetPasswordModelInput model)
        {
            var resultLogin = await _signInManager.PasswordSignInAsync(model.login, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultLogin.Succeeded)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(model.login);

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.newPassword);
                if (result.Succeeded)
                {
                    var token = await _authService.GenerateTokenAsync(user);
                    return Ok(new { token });
                }
            }

            return BadRequest("Invalid login or Password");

        }
        [Authorize]
        [HttpPost("changeUsername")]
        public async Task<IActionResult> changeUsername([FromBody] changeUsernameModelInput model)
        {

            ApplicationUser user = await _userManager.FindByNameAsync(model.oldLogin);

            if (await _userManager.FindByNameAsync(model.newLogin) != null)
            {
                return BadRequest("This username is in use by another Player");
            }
            user.UserName = model.newLogin;
            user.NormalizedUserName = model.newLogin.ToUpper();

            await _userManager.UpdateAsync(user);

            var token = await _authService.GenerateTokenAsync(user);

            return Ok(new { token });

        }
    }
}

