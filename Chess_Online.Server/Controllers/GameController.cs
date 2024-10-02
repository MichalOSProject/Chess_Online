using Chess_Online.Server.Models.InputModels;
using Chess_Online.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Chess_Online.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly IGameActionService _gameService;
        private readonly IGameInstanceService _gameInstanceService;

        public GameController(IGameActionService gameService, IGameInstanceService gameInstanceService)
        {
            _gameService = gameService;
            _gameInstanceService = gameInstanceService;
        }

        [HttpPost("initialize")]
        public async Task<string> InitializeGame(CreateNewGameModelInput RequestData)
        {
            string json = JsonConvert.SerializeObject(await _gameInstanceService.Create(RequestData));
            return json;

        }

        [HttpPost("gameInfo")]
        public async Task<string> GetGameInstance(GetGameInfoModelInput game)
        {
            string json = JsonConvert.SerializeObject(await _gameInstanceService.GetGameInfo(game.gameId));
            return json;
        }
    }
}
