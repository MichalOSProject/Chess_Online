using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Data;
using Chess_Online.Server.Models.InputModels;
using Chess_Online.Server.Models.Pieces;
using Chess_Online.Server.Services.Interfaces;
using Chess_Online.Server.Models.OutputModels;
using Microsoft.EntityFrameworkCore;

namespace Chess_Online.Server.Services.Services;
public class GameInstanceService : IGameInstanceService
{
    private readonly ApplicationDbContext _context;
    private readonly IDataConversionService _dataConversionService;

    public GameInstanceService(ApplicationDbContext context, IDataConversionService dataConversionService)
    {
        _context = context;
        _dataConversionService = dataConversionService;
    }
    public async Task<GameDataSimpleModelOutput> GetGameInfo(int gameId)
    {
        GameInstance _gameInstance = await GetGameInstance(gameId);
        GameDataSimpleModelOutput gameDataSimpleModelOutput = await _dataConversionService.GetSimpleGameInfo(_gameInstance);

        return gameDataSimpleModelOutput;
    }
    public async Task<GameDataSimpleModelOutput> Create(CreateNewGameModelInput newGame)
    {
        Piece[,] pieces = new Piece[8, 8];

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                pieces[i, j] = new EmptyPiece();

        pieces[0, 0] = new Rook(TeamEnum.Black);
        pieces[0, 7] = new Rook(TeamEnum.Black);
        pieces[7, 0] = new Rook(TeamEnum.White);
        pieces[7, 7] = new Rook(TeamEnum.White);
        pieces[0, 1] = new Knight(TeamEnum.Black);
        pieces[0, 6] = new Knight(TeamEnum.Black);
        pieces[7, 1] = new Knight(TeamEnum.White);
        pieces[7, 6] = new Knight(TeamEnum.White);
        pieces[0, 2] = new Bishop(TeamEnum.Black);
        pieces[0, 5] = new Bishop(TeamEnum.Black);
        pieces[7, 2] = new Bishop(TeamEnum.White);
        pieces[7, 5] = new Bishop(TeamEnum.White);
        pieces[0, 4] = new Queen(TeamEnum.Black);
        pieces[7, 4] = new Queen(TeamEnum.White);
        pieces[0, 3] = new King(TeamEnum.Black);
        pieces[7, 3] = new King(TeamEnum.White);
        for (int i = 0; i < 8; i++)
            pieces[1, i] = new Pawn(TeamEnum.Black);
        for (int i = 0; i < 8; i++)
            pieces[6, i] = new Pawn(TeamEnum.White);

        var gameInstance = new GameInstanceEntity
        {
            ChessBoardMap = await _dataConversionService.ArrayToBoard(pieces),
            PlayerTeamWhite = newGame.playerTeamWhite,
            PlayerTeamBlack = newGame.playerTeamBlack,
            CheckByWhite = CheckmateStatusEnum.Safe,
            CheckByBlack = CheckmateStatusEnum.Safe,
            GameEnded = false,
            PlayerTurn = newGame.firstTeam

        };

        _context.GameInstancesEntity.Add(gameInstance);

        await _context.SaveChangesAsync();

        return await GetGameInfo(gameInstance.Id);
    }
    public async Task<GameInstance> GetGameInstance(int id)
    {
        GameInstanceEntity gameInstanceEntity = _context.GameInstancesEntity.Include(game => game.ChessBoardMap).Where(game => game.Id == id).FirstOrDefault();
        GameInstance gameInstance = await _dataConversionService.ConvertSqlDataToGameInstance(gameInstanceEntity);
        CheckPossibleMoves(gameInstance);
        CheckMate(gameInstance);
        return gameInstance;
    }

    public async Task<bool> isThisMyMove(int id, string userId)
    {
        GameInstanceEntity gameInstanceEntity = await _context.GameInstancesEntity.FindAsync(id);
        await _context.Entry(gameInstanceEntity).ReloadAsync();
        if (gameInstanceEntity.PlayerTurn.Equals(TeamEnum.White))
        {
            if (gameInstanceEntity.PlayerTeamWhite.Equals(userId.Trim(), StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
        else
        {
            if (gameInstanceEntity.PlayerTeamBlack.Equals(userId.Trim(), StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }

    //Checking if used Coords are still at the map
    private static bool IsCoordsAtMap(int Column, int Row)
    {
        if (Column >= 0 && Row >= 0 && Column <= 7 && Row <= 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<GameInstance> UpdateGameInstance(GameInstance _gameInstance)
    {
        _gameInstance = CheckPossibleMoves(_gameInstance);
        _gameInstance = CheckMate(_gameInstance);
        GameInstanceEntity gameInstanceEntity = await _dataConversionService.ConvertGameInstanceToSqlData(_gameInstance);
        var dbGameInstance = _context.GameInstancesEntity.Include(game => game.ChessBoardMap).FirstOrDefault(item => item.Id == _gameInstance.Id);
        dbGameInstance.ChessBoardMap = gameInstanceEntity.ChessBoardMap;
        dbGameInstance.PlayerTurn = gameInstanceEntity.PlayerTurn;
        dbGameInstance.CheckByBlack = gameInstanceEntity.CheckByBlack;
        dbGameInstance.CheckByWhite = gameInstanceEntity.CheckByWhite;
        dbGameInstance.GameEnded = gameInstanceEntity.GameEnded;

        await _context.SaveChangesAsync();

        return _gameInstance;
    }
    private GameInstance CheckMate(GameInstance _gameInstance)
    {
        if (!_gameInstance.GameEnded)
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (_gameInstance.Pieces[i, j].PieceType.Equals(PieceTypeEnum.King))
                    {
                        List<(int column, int row)> PossibleMoves = _gameInstance.Pieces[i, j].CheckedMoves;

                        if (_gameInstance.Pieces[i, j].Team.Equals(TeamEnum.White))
                        {//For White Team King
                            if (_gameInstance.Pieces[i, j].DamageByBlack > 0)
                            {
                                if (PossibleMoves.Any())
                                {
                                    _gameInstance.GameEnded = true;
                                    _gameInstance.CheckByBlack = CheckmateStatusEnum.Defeated;
                                    foreach (var item in PossibleMoves)
                                    {
                                        if (_gameInstance.Pieces[item.column, item.row].DamageByBlack > 0)
                                            continue;
                                        else
                                        {
                                            _gameInstance.CheckByBlack = CheckmateStatusEnum.Endangered;
                                            _gameInstance.GameEnded = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (_gameInstance.Pieces[i, j].DamageByBlack >= 2)
                                    {
                                        _gameInstance.CheckByBlack = CheckmateStatusEnum.Defeated;
                                        _gameInstance.GameEnded = true;
                                    }
                                    if (_gameInstance.CheckByBlack.Equals(CheckmateStatusEnum.Endangered))
                                    {
                                        _gameInstance.CheckByBlack = CheckmateStatusEnum.Defeated;
                                        _gameInstance.GameEnded = true;
                                    }
                                    if (_gameInstance.CheckByBlack.Equals(CheckmateStatusEnum.Safe))
                                        _gameInstance.CheckByBlack = CheckmateStatusEnum.Endangered;
                                }
                            }
                        }
                        else
                        { //For Black Team King
                            if (_gameInstance.Pieces[i, j].DamageByWhite > 0)
                            {
                                if (PossibleMoves.Any())
                                {
                                    _gameInstance.GameEnded = true;
                                    _gameInstance.CheckByWhite = CheckmateStatusEnum.Defeated;
                                    foreach (var item in PossibleMoves)
                                    {
                                        if (_gameInstance.Pieces[item.column, item.row].DamageByWhite > 0)
                                            continue;
                                        else
                                        {
                                            _gameInstance.CheckByWhite = CheckmateStatusEnum.Endangered;
                                            _gameInstance.GameEnded = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (_gameInstance.Pieces[i, j].DamageByWhite >= 2)
                                    {
                                        _gameInstance.CheckByWhite = CheckmateStatusEnum.Defeated;
                                        _gameInstance.GameEnded = true;
                                    }
                                    if (_gameInstance.CheckByWhite.Equals(CheckmateStatusEnum.Endangered))
                                    {
                                        _gameInstance.CheckByWhite = CheckmateStatusEnum.Defeated;
                                        _gameInstance.GameEnded = true;
                                    }
                                    if (_gameInstance.CheckByWhite.Equals(CheckmateStatusEnum.Safe))
                                        _gameInstance.CheckByWhite = CheckmateStatusEnum.Endangered;
                                }
                            }
                        }
                    }
                }
        return _gameInstance;
    }

    private GameInstance CheckPossibleMoves(GameInstance _gameInstance)
    {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                _gameInstance.Pieces[i, j].ResetCheckedMoves();
                _gameInstance.Pieces[i, j].ResetDamage();
            }


        int[,] MoveToAssignByPiece;
        int[,] DmgToAssignByPiece;
        TeamEnum WhosMoving;

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                //If moving is EmptyPiece, check next Piece
                WhosMoving = _gameInstance.Pieces[i, j].Team;
                if (WhosMoving.Equals(TeamEnum.NoMansLand))
                    continue;

                //Get array with moves vectors
                MoveToAssignByPiece = _gameInstance.Pieces[i, j].Moves;

                //Check all possible moves, if coords do not lead beyond the map and point to EmptyPiece, assign move to Piece's Array
                for (int k = 0; k < MoveToAssignByPiece.GetLength(0); k++)
                {
                    for (int l = 0; l < _gameInstance.Pieces[i, j].GetJumpsOnMove();)
                    {
                        l++;
                        int column = MoveToAssignByPiece[k, 0] * l + i;
                        int row = MoveToAssignByPiece[k, 1] * l + j;
                        if (!IsCoordsAtMap(column, row))
                            continue;

                        //If it is not EmptyPiece, break "L" Loop
                        if (!_gameInstance.Pieces[column, row].Team.Equals(TeamEnum.NoMansLand))
                            break;

                        //If it is EmptyPiece, assign possible move
                        if (WhosMoving.Equals(TeamEnum.White))
                            _gameInstance.Pieces[column, row].SetDamageByWhite(1);
                        else
                            _gameInstance.Pieces[column, row].SetDamageByBlack(1);
                        _gameInstance.Pieces[i, j].AddCheckedMoves(column, row);
                    }
                }

                //Get array with attack vectors
                DmgToAssignByPiece = _gameInstance.Pieces[i, j].Attack;

                //Check all possible beatings
                for (int k = 0; k < DmgToAssignByPiece.GetLength(0); k++)
                {
                    for (int l = 0; l < _gameInstance.Pieces[i, j].JumpsOnAttack;)
                    {
                        l++;
                        int column = DmgToAssignByPiece[k, 0] * l + i;
                        int row = DmgToAssignByPiece[k, 1] * l + j;
                        if (!IsCoordsAtMap(column, row))
                            continue;

                        //If it is EmptyPiece, check next iterations
                        if (_gameInstance.Pieces[column, row].Team.Equals(TeamEnum.NoMansLand))
                            continue;

                        //If it is Ally, break "L" loop
                        if (_gameInstance.Pieces[column, row].Team.Equals(WhosMoving))
                            break;

                        //If it is King, assign damage and break "L" loop beacause beating King is Illegal
                        if (_gameInstance.Pieces[column, row].PieceType.Equals(PieceTypeEnum.King))
                        {
                            if (WhosMoving.Equals(TeamEnum.White))
                                _gameInstance.Pieces[column, row].SetDamageByWhite(1);
                            else
                                _gameInstance.Pieces[column, row].SetDamageByBlack(1);
                            break;
                        }

                        //If it is Enemy, assign possible move, assign damage and break
                        _gameInstance.Pieces[i, j].AddCheckedMoves(column, row);
                        break;
                    }
                }
            }
        return _gameInstance;
    }
}






