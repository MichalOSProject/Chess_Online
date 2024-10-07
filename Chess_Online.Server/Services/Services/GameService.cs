using Chess_Online.Server.Data;
using Chess_Online.Server.Models.Pieces;
using Chess_Online.Server.Services.Interfaces;
using System.Net.WebSockets;
using System.Text;
using Chess_Online.Server.Models.OutputModels;
using Chess_Online.Server.Models.InputModels;
using Newtonsoft.Json;
using Chess_Online.Server.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chess_Online.Server.Services.Services
{
    public class GameService : IGameService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGameInstanceService _gameInstanceService;
        private readonly IAuthService _authService;
        private static readonly Dictionary<int, List<WebSocket>> ActiveConnections = new Dictionary<int, List<WebSocket>>();

        public GameService(ApplicationDbContext context, IGameInstanceService gameInstanceService, IAuthService authService)
        {
            _context = context;
            _gameInstanceService = gameInstanceService;
            _authService = authService;
        }

        public async Task GameAction(WebSocket webSocket, string token)
        {
            int IdOfGameInstance = 0;
            var buffer = new byte[1024 * 4];
            bool initialMessageReceived = false;

            while (webSocket.State == WebSocketState.Open)
            {
                if (await _authService.ValidateToken(token))
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        try
                        {
                            if (!initialMessageReceived)
                            {
                                var initialMessage = System.Text.Json.JsonSerializer.Deserialize<InitialMessageModel>(jsonMessage);
                                if (initialMessage != null && initialMessage.gameId > 0)
                                {
                                    IdOfGameInstance = initialMessage.gameId;
                                    initialMessageReceived = true;

                                    if (!ActiveConnections.ContainsKey(IdOfGameInstance))
                                    {
                                        ActiveConnections[IdOfGameInstance] = new List<WebSocket>();
                                    }
                                    ActiveConnections[IdOfGameInstance].Add(webSocket);

                                    await SendInitialData(webSocket, IdOfGameInstance);
                                }
                                continue;
                            }
                            else
                            {
                                var receivedMessage = System.Text.Json.JsonSerializer.Deserialize<MoveRequestModelInput>(jsonMessage);
                                if (receivedMessage != null)
                                {
                                    JwtTokens tokenJWT = await _authService.GetTokenAsync(token);
                                    if (await _gameInstanceService.isThisMyMove(IdOfGameInstance, tokenJWT.UserId))
                                    {
                                        var actionResult = await MoveAction(receivedMessage, IdOfGameInstance, tokenJWT.UserId);
                                        var confirmationMessage = JsonConvert.SerializeObject(actionResult.Item1);
                                        await BroadcastToPlayers(IdOfGameInstance, confirmationMessage, actionResult.Item2);
                                    }
                                    else
                                    {
                                        await SendErrorMessage(webSocket, "It is not your turn!");
                                    }

                                }
                            }
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            await SendErrorMessage(webSocket, "Error: Invalid JSON data.");
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection Closed", CancellationToken.None);
                        ActiveConnections[IdOfGameInstance].Remove(webSocket);
                        if (ActiveConnections[IdOfGameInstance].Count == 0)
                        {
                            ActiveConnections.Remove(IdOfGameInstance);
                        }
                    }
                }
            }
        }

        private async Task BroadcastToPlayers(int gameId, string message, bool warningStatus)
        {
            GameDataSimpleModelOutput gameInfo = await _gameInstanceService.GetGameInfoToSend(gameId);
            gameInfo.Message = message;
            gameInfo.Warning = warningStatus;
            string gameInfoJson = JsonConvert.SerializeObject(gameInfo);
            var dataToSend = new
            {
                action = "gameUpdate",
                Data = gameInfoJson
            };
            string dataJSON = JsonConvert.SerializeObject(dataToSend);
            var gameInfoBuffer = Encoding.UTF8.GetBytes(dataJSON);
            if (ActiveConnections.ContainsKey(gameId))
            {
                var gameConnections = ActiveConnections[gameId];

                foreach (var connection in gameConnections)
                {
                    if (connection != null)
                        if (connection.State == WebSocketState.Open)
                        {
                            await connection.SendAsync(new ArraySegment<byte>(gameInfoBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                }
            }
        }

        private async Task SendInitialData(WebSocket webSocket, int gameId)
        {
            GameDataSimpleModelOutput gameInfo = await _gameInstanceService.GetGameInfoToSend(gameId);
            string gameInfoJson = JsonConvert.SerializeObject(gameInfo);
            var dataToSend = new
            {
                action = "gameUpdate",
                Data = gameInfoJson
            };
            string dataJSON = JsonConvert.SerializeObject(dataToSend);
            var gameInfoBuffer = Encoding.UTF8.GetBytes(dataJSON);
            await webSocket.SendAsync(new ArraySegment<byte>(gameInfoBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }


        private async Task SendErrorMessage(WebSocket webSocket, string errorMessage)
        {
            var dataToSend = new
            {
                action = "errorMessage",
                Data = errorMessage
            };
            string dataJSON = JsonConvert.SerializeObject(dataToSend);
            var errorBuffer = Encoding.UTF8.GetBytes(dataJSON);

            await webSocket.SendAsync(new ArraySegment<byte>(errorBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<(string, bool)> MoveAction(MoveRequestModelInput requestData, int gameId, string userId)
        {
            GameInstance _gameInstance = await _gameInstanceService.GetCalculatedGameInstanceFromSQL(gameId); // game instance with map and calculated checkmates and possible moves
            if (_gameInstance == null)
                return ("Game do not Exist", true);

            if (!_gameInstance.Pieces[requestData.CoordsPiece[0], requestData.CoordsPiece[1]].Team.Equals(_gameInstance.PlayerTurn))
                return ("Tried to use wrong Team's Piece", true);

            if (!_gameInstance.Pieces[requestData.CoordsPiece[0], requestData.CoordsPiece[1]].CheckedMoves.Contains((requestData.CoordsDestination[0], requestData.CoordsDestination[1])))
                return ("This move is not allowed because of the 'checked moves map'", true);

            // Return result if game is over
            if (_gameInstance.GameEnded)
            {
                if (_gameInstance.CheckByWhite.Equals(CheckmateStatusEnum.Defeated))
                    return ("Game is Over, Team White Won", true);
                else
                    return ("Game is Over, Team Black Won", true);
            }

            // Make the move
            _gameInstance.Pieces[requestData.CoordsPiece[0], requestData.CoordsPiece[1]].Moved = true;
            _gameInstance.Pieces[requestData.CoordsDestination[0], requestData.CoordsDestination[1]] = _gameInstance.Pieces[requestData.CoordsPiece[0], requestData.CoordsPiece[1]];
            _gameInstance.Pieces[requestData.CoordsPiece[0], requestData.CoordsPiece[1]] = new EmptyPiece();

            // Switch turn
            _gameInstance.PlayerTurn = _gameInstance.PlayerTurn.Equals(TeamEnum.White) ? TeamEnum.Black : TeamEnum.White;

            // Recalculate checkmate stats and possible moves for next Player
            _gameInstance = await _gameInstanceService.SaveGameToSQL(_gameInstance);

            // Return result if game is over
            if (_gameInstance.GameEnded)
            {
                if (_gameInstance.CheckByWhite.Equals(CheckmateStatusEnum.Defeated))
                    return ("Game is Over, Team White Won", true);
                else
                    return ("Game is Over, Team Black Won", true);
            }

            // Return warning if one of Kings are Endangered
            if (_gameInstance.PlayerTurn.Equals(TeamEnum.White))
            {
                if (_gameInstance.CheckByBlack.Equals(CheckmateStatusEnum.Endangered))
                    return ("White's Team King is Endangered", true);
            }
            else
            {
                if (_gameInstance.CheckByWhite.Equals(CheckmateStatusEnum.Endangered))
                    return ("Black's Team King is Endangered", true);
            }

            return ("The move was made successfully", false);
        }
        public class InitialMessageModel
        {
            public int gameId { get; set; }
        }
    }
}
