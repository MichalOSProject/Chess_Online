using Chess_Online.Server.Data;
using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Models.OutputModels;
using Chess_Online.Server.Models.Pieces;

namespace Chess_Online.Server.Services.Interfaces;
public interface IDataConversionService
{
    Task<GameDataSimpleModelOutput> ShrinkGameInfoToSimple(GameInstance gameInstance);
    Task<GameInstance> ConvertSqlDataToGameInstance(GameInstanceEntity gameInstanceEntity);
    Task<GameInstanceEntity> ConvertGameInstanceToSqlData(GameInstance gameInstance);
    Task<Piece[,]> BoardToArray(ChessBoard chessBoardMap);
    Task<ChessBoard> ArrayToBoard(Piece[,] pieces);
}