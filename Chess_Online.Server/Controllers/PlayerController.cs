using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Models.OutputModels;
using Chess_Online.Server.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chess_Online.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlayerController(IPlayerService playerService, IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _playerService = playerService;
            _authService = authService;
            _userManager = userManager;
        }

        [HttpGet("PlayerStats")]
        public async Task<ActionResult<PlayerStats>> GetPlayerStats([FromQuery] string username)
        {
            PlayerStats data = await _playerService.GetPlayerStats(username);
            if (data == null)
                return BadRequest("Player with this username do not exist");
            return Ok(data);
        }

        [HttpPost("uploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var token = Request.Headers["Authorization"].ToString();
            Console.WriteLine("tokenik " + token);
            if (!await _authService.ValidateToken(token))
                return Unauthorized("Token is missing or invalid.");

            JwtTokens approvedToken = await _authService.GetTokenAsync(token);

            string message = await _playerService.UploadProfilePicture(file, approvedToken.UserId);

            if (string.IsNullOrEmpty(message))
                return Ok();

            return BadRequest(message);
        }

        [HttpGet("getProfilePhoto")]
        public async Task<string> GetProfilePhoto([FromQuery] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }

            string url = await _playerService.GetProfilePicture(username);

            return $"{Request.Scheme}://{Request.Host}" + url;
        }

        [HttpGet("gamesList")]
        public async Task<ActionResult<List<PlayerGame>>> GetPlayerGamesList([FromQuery] string username)
        {
            List<PlayerGame> playerGamesList = await _playerService.GetPlayerGamesList(username);
            return Ok(playerGamesList);
        }
    }
}
