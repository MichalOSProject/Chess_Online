using Chess_Online.Server.Data;
using Chess_Online.Server.Models.InputModels;
using Chess_Online.Server.Models.OutputModels;

namespace Chess_Online.Server.Services.Interfaces;
public interface IGameInstanceService
{
    Task<GameDataSimpleModelOutput> GetGameInfo(int gameId);
    Task<GameDataSimpleModelOutput> Create(CreateNewGameModelInput newGame);
    Task<GameInstance> GetGameInstance(int id);
    Task<GameInstance> UpdateGameInstance(GameInstance _gameInstance);
    Task<bool> isThisMyMove(int id, string userId);
}
