using Chess_Online.Server.Data;
using Chess_Online.Server.Models.InputModels;
using Chess_Online.Server.Models.OutputModels;

namespace Chess_Online.Server.Services.Interfaces;
public interface IGameInstanceService
{
    Task<GameDataSimpleModelOutput> GetGameInfoToSend(int gameId);
    Task<GameDataSimpleModelOutput> Create(CreateNewGameModelInput newGame);
    Task<GameInstance> GetCalculatedGameInstanceFromSQL(int id);
    Task<GameInstance> SaveGameToSQL(GameInstance _gameInstance);
    Task<bool> isThisMyMove(int id, string userId);
    Task<bool> isEnded(int id);
}
