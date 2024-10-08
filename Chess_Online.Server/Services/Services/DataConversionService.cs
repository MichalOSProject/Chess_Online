using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Data;
using Chess_Online.Server.Models.Pieces;
using Chess_Online.Server.Services.Interfaces;
using Chess_Online.Server.Models.OutputModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chess_Online.Server.Services.Services;
public class DataConversionService : IDataConversionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    public DataConversionService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<GameDataSimpleModelOutput> ShrinkGameInfoToSimple(GameInstance gameInstance)
    {
        if (gameInstance == null) return null;

        SimplePiece[,] gameMap = new SimplePiece[8, 8];
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                List<(int column, int row)> PossibleMoves = [];
                if (gameInstance.Pieces[i, j].Team.Equals(gameInstance.PlayerTurn))
                    PossibleMoves = gameInstance.Pieces[i, j].CheckedMoves;
                gameMap[i, j] = new SimplePiece(gameInstance.Pieces[i, j].Team, gameInstance.Pieces[i, j].PieceType, PossibleMoves);
            }
        GameDataSimpleModelOutput gameInfo = new GameDataSimpleModelOutput
        {
            Id = gameInstance.Id,
            Pieces = gameMap,
            GameEnded = gameInstance.GameEnded,
            PlayerTurn = gameInstance.PlayerTurn,
            TeamWhite = await _userManager.GetUserNameAsync(await _userManager.FindByIdAsync(gameInstance.PlayerTeamWhite)),
            TeamBlack = await _userManager.GetUserNameAsync(await _userManager.FindByIdAsync(gameInstance.PlayerTeamBlack)),
        };
        return gameInfo;
    }

    public async Task<GameInstance> ConvertSqlDataToGameInstance(GameInstanceEntity gameInstanceEntity)
    {
        Piece[,] pieces = await BoardToArray(gameInstanceEntity.ChessBoardMap);
        GameInstance gameInstance = new GameInstance
        (
            gameInstanceEntity.Id,
            pieces,
            gameInstanceEntity.PlayerTeamWhite,
            gameInstanceEntity.PlayerTeamBlack,
            gameInstanceEntity.CheckByWhite,
            gameInstanceEntity.CheckByBlack,
            gameInstanceEntity.GameEnded,
            gameInstanceEntity.PlayerTurn
        );
        return gameInstance;
    }

    public async Task<GameInstanceEntity> ConvertGameInstanceToSqlData(GameInstance gameInstance)
    {
        GameInstanceEntity gameInstanceEntity = new GameInstanceEntity
        {
            Id = gameInstance.Id,
            ChessBoardMap = await ArrayToBoard(gameInstance.Pieces),
            PlayerTeamWhite = gameInstance.PlayerTeamWhite,
            PlayerTeamBlack = gameInstance.PlayerTeamBlack,
            CheckByWhite = gameInstance.CheckByWhite,
            CheckByBlack = gameInstance.CheckByBlack,
            GameEnded = gameInstance.GameEnded,
            PlayerTurn = gameInstance.PlayerTurn
        };
        return gameInstanceEntity;
    }
    public async Task<Piece[,]> BoardToArray(ChessBoard chessBoardMap)
    {
        var pieces = new Piece[8, 8];
        Type chessBoardType = chessBoardMap.GetType();

        for (int row = 8; row >= 1; row--)
        {
            for (int col = 0; col < 8; col++)
            {
                char column = (char)('A' + col);
                string propertyName = $"CB_{row}{column}";

                var propertyInfo = chessBoardType.GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    string jsonString = propertyInfo.GetValue(chessBoardMap)?.ToString();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        Square square = JsonConvert.DeserializeObject<Square>(jsonString);
                        pieces[8 - row, col] = CreatePiece(square.PieceType, square.Team);
                        pieces[8 - row, col].Moved = square.Moved;
                    }
                }
            }
        }

        return pieces;
    }

    private Piece CreatePiece(PieceTypeEnum pieceType, TeamEnum team)
    {
        return pieceType switch
        {
            PieceTypeEnum.Pawn => new Pawn(team),
            PieceTypeEnum.Knight => new Knight(team),
            PieceTypeEnum.Bishop => new Bishop(team),
            PieceTypeEnum.Rook => new Rook(team),
            PieceTypeEnum.Queen => new Queen(team),
            PieceTypeEnum.King => new King(team),
            PieceTypeEnum.Nomad => new EmptyPiece()
        };
    }
    public async Task<ChessBoard> ArrayToBoard(Piece[,] pieces)
    {
        ChessBoard chessBoard = new ChessBoard();

        for (int row = 8; row >= 1; row--)
        {
            for (int col = 0; col < 8; col++)
            {
                char column = (char)('A' + col);

                Piece piece = pieces[8 - row, col];

                string squareRepresentation = JsonConvert.SerializeObject(new Square
                {
                    column = col + 1,
                    row = row,
                    PieceType = GetPieceType(piece),
                    Team = piece.Team,
                    Moved = piece.Moved
                });

                string propertyName = $"CB_{row}{column}";
                var propertyInfo = typeof(ChessBoard).GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(chessBoard, squareRepresentation);
                }
            }
        }

        return chessBoard;
    }


    private PieceTypeEnum GetPieceType(Piece piece)
    {
        return piece switch
        {
            Pawn => PieceTypeEnum.Pawn,
            Knight => PieceTypeEnum.Knight,
            Bishop => PieceTypeEnum.Bishop,
            Rook => PieceTypeEnum.Rook,
            Queen => PieceTypeEnum.Queen,
            King => PieceTypeEnum.King,
            EmptyPiece => PieceTypeEnum.Nomad
        };
    }
    private class Square
    {
        [Required]
        public int column { get; set; }
        [Required]
        public int row { get; set; }
        [Required]
        public TeamEnum Team { get; set; }
        [Required]
        public PieceTypeEnum PieceType { get; set; }
        [Required]
        public bool Moved { get; set; }
    }
}






