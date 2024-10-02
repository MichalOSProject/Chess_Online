using Chess_Online.Server.Data;
using Chess_Online.Server.Models.InputModels;
using Chess_Online.Server.Models.OutputModels;
using Chess_Online.Server.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Models.Pieces;
using Microsoft.AspNetCore.Identity;

namespace Chess_Online.Server.Services.Services;

public class LobbyService : ILobbyService
{
    private readonly ApplicationDbContext _context;
    private readonly IGameInstanceService _gameInstanceService;
    private readonly IAuthService _authService;
    private readonly UserManager<IdentityUser> _userManager;
    private static readonly Dictionary<int, LobbyInfoModelOutput> SingleGameInstance = new Dictionary<int, LobbyInfoModelOutput>();
    private static int LobbyId = 0;
    private static readonly Dictionary<int, List<WebSocket>> AvailableGamesList = new Dictionary<int, List<WebSocket>>();
    private static readonly List<WebSocket> ViewersList = new List<WebSocket>();

    public LobbyService(ApplicationDbContext context, IGameInstanceService gameInstanceService, IAuthService authService, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _gameInstanceService = gameInstanceService;
        _authService = authService;
        _userManager = userManager;
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket, string token)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        ViewersList.Add(webSocket);

        while (!result.CloseStatus.HasValue)
        {
            if (await _authService.ValidateToken(token))
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                JwtTokens tokenJWT = await _authService.GetTokenAsync(token);
                var user = await _userManager.FindByIdAsync(tokenJWT.UserId);

                WebSocketMessage command = System.Text.Json.JsonSerializer.Deserialize<WebSocketMessage>(message);
                switch (command.Action)
                {
                    case "newSession":
                        await CreateNewSession(webSocket, user);
                        BroadcastToViewers();
                        break;

                    case "delSession":
                        await DeleteSession(command.Data, user);
                        BroadcastToViewers();
                        break;

                    case "leaveSession":
                        await LeaveSession(webSocket, command.Data, user);
                        BroadcastToViewers();
                        break;
                    case "joinSession":
                        await JoinSession(webSocket, command.Data, user);
                        BroadcastToViewers();
                        break;
                    case "startSession":
                        await StartSession(webSocket, command.Data, user);
                        BroadcastToViewers();
                        break;
                    case "getLobby":
                        await GetLobbyInfo(webSocket);
                        break;
                    default:
                        await HandleUnknownActionAsync(webSocket);
                        break;
                }
            }
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        try
        {
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        finally
        {
            ViewersList.Remove(webSocket);
        };
    }

    private async Task GetLobbyInfo(WebSocket webSocket)
    {
        var message = SingleGameInstance.Any() ? JsonConvert.SerializeObject(SingleGameInstance) : null;
        await SendLobbyData(webSocket, message);
    }
    private async Task JoinSession(WebSocket webSocket, string data, IdentityUser user)
    {
        int id = int.Parse(data);
        if (SingleGameInstance[id].PlayerTwoId == null && SingleGameInstance[id].PlayerOneId != user.Id)
        {
            SingleGameInstance[id].PlayerTwo = user.UserName;
            SingleGameInstance[id].PlayerTwoId = user.Id;
            AvailableGamesList[id].Add(webSocket);
        }
    }
    private async Task LeaveSession(WebSocket webSocket, string data, IdentityUser user)
    {
        int id = int.Parse(data);
        if (SingleGameInstance[id].PlayerTwoId == user.Id)
        {
            SingleGameInstance[id].PlayerTwoId = null;
            SingleGameInstance[id].PlayerTwo = null;
            AvailableGamesList[id].Remove(webSocket);
        }
    }
    private async Task DeleteSession(string data, IdentityUser user)
    {
        int id = int.Parse(data);
        if (SingleGameInstance[id].PlayerOneId == user.Id)
        {
            SingleGameInstance.Remove(id);
            AvailableGamesList.Remove(id);
        }
    }

    private async Task StartSession(WebSocket webSocket, string data, IdentityUser user)
    {
        int id = int.Parse(data);
        if (SingleGameInstance[id].PlayerOneId == user.Id &&
            SingleGameInstance[id].PlayerTwoId != null &&
            SingleGameInstance[id].PlayerTwoId != user.Id)
        {
            CreateNewGameModelInput gameOptions = new CreateNewGameModelInput
            {
                playerTeamWhite = SingleGameInstance[id].PlayerOneId,
                playerTeamBlack = SingleGameInstance[id].PlayerTwoId,
                firstTeam = TeamEnum.White
            };
            GameDataSimpleModelOutput game = await _gameInstanceService.Create(gameOptions);
            BroadcastToPlayers(id, game.Id.ToString());
            DeleteSession(data, user);
        }
    }

    private async Task CreateNewSession(WebSocket webSocket, IdentityUser user)
    {
        LobbyId++;
        int id = LobbyId;
        if (!AvailableGamesList.ContainsKey(id))
        {
            AvailableGamesList[id] = new List<WebSocket>();
        }
        AvailableGamesList[id].Add(webSocket);
        var game = new LobbyInfoModelOutput
        {
            Id = LobbyId,
            PlayerOneId = user.Id,
            PlayerOne = user.UserName
        };
        SingleGameInstance[id] = game;

        var message = JsonConvert.SerializeObject(id);
        await SendLobbyData(webSocket, message);
    }
    private async Task HandleUnknownActionAsync(WebSocket webSocket)
    {
        var response = Encoding.UTF8.GetBytes("{\"error\": \"Unknown action\"}");
        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task BroadcastToPlayers(int gameId, string message)
    {
        if (AvailableGamesList.ContainsKey(gameId))
        {
            var gameConnections = AvailableGamesList[gameId];

            foreach (var connection in gameConnections)
            {
                if (connection != null)
                    if (connection.State == WebSocketState.Open)
                    {
                        await SendNewGameId(connection, message);
                    }
            }
        }
    }
    private async Task BroadcastToViewers()
    {
        foreach (var connection in ViewersList)
        {
            if (connection != null)
                if (connection.State == WebSocketState.Open)
                {
                    await GetLobbyInfo(connection);
                }
        }
    }
    private async Task SendNewGameId(WebSocket webSocket, string message)
    {
        var dataToSend = new
        {
            action = "newGameId",
            Data = message
        };
        var gameInfoJson = JsonConvert.SerializeObject(dataToSend);
        var gameInfoBuffer = Encoding.UTF8.GetBytes(gameInfoJson);

        await webSocket.SendAsync(new ArraySegment<byte>(gameInfoBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task SendLobbyData(WebSocket webSocket, string message)
    {
        var dataToSend = new
        {
            action = "lobbyUpdate",
            Data = message
        };
        var gameInfoJson = JsonConvert.SerializeObject(dataToSend);
        var gameInfoBuffer = Encoding.UTF8.GetBytes(gameInfoJson);
        await webSocket.SendAsync(new ArraySegment<byte>(gameInfoBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

}

public class WebSocketMessage
{
    public string Action { get; set; }
    public string Data { get; set; }
}
