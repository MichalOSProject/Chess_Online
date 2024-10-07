using Chess_Online.Server.Data;
using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Models.OutputModels;
using Chess_Online.Server.Models.Pieces;
using Chess_Online.Server.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chess_Online.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("PlayerStats")]
        public async Task<ActionResult<PlayerStats>> GetPlayerStats([FromQuery] string username)
        {
            PlayerStats data = await _playerService.GetPlayerStats(username);
            if (data == null)
                return BadRequest("Player with this username do not exist");
            return Ok(data);
        }
    }
}
