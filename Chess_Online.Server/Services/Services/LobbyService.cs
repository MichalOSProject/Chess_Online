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
using Chess_Online.Server.Models.ServiceObjectModels;

namespace Chess_Online.Server.Services.Services;

public class LobbyService : ILobbyService
{
    private readonly ApplicationDbContext _context;
    private readonly IGameInstanceService _gameInstanceService;
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;
    private static readonly Dictionary<int, LobbyInfoServiceModel> GamesInLobby = new Dictionary<int, LobbyInfoServiceModel>();
    private static int LobbyId = 0;
    private static readonly Dictionary<int, List<WebSocket>> AvailableGamesList = new Dictionary<int, List<WebSocket>>();
    private static readonly List<WebSocket> ViewersList = new List<WebSocket>();

    public LobbyService(ApplicationDbContext context, IGameInstanceService gameInstanceService, IAuthService authService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _gameInstanceService = gameInstanceService;
        _authService = authService;
        _userManager = userManager;
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket, string token)
    {
        int SignedToGameId = 0;
        int OwnedGameId = 0;
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
                        OwnedGameId = await CreateNewSession(webSocket, user, OwnedGameId, SignedToGameId);
                        break;
                    case "delSession":
                        OwnedGameId = await DeleteSession(webSocket, OwnedGameId);
                        break;
                    case "leaveSession":
                        SignedToGameId = await LeaveSession(webSocket, SignedToGameId);
                        break;
                    case "joinSession":
                        SignedToGameId = await JoinSession(webSocket, command.Data, user, SignedToGameId, OwnedGameId);
                        break;
                    case "startSession":
                        OwnedGameId = await StartSession(webSocket, user, OwnedGameId);
                        break;
                    case "switchTeam":
                        await SwitchTeamInSession(webSocket, OwnedGameId);
                        break;
                    case "getLobby":
                        await GetLobbyInfo(webSocket);
                        break;
                    default:
                        await SendDataToEndpoint(webSocket, "Unknown action", "error");
                        break;
                }
            }
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        CloseSession(webSocket, result, OwnedGameId, SignedToGameId);
    }

    private async Task SwitchTeamInSession(WebSocket webSocket, int OwnedGameId)
    {
        if (OwnedGameId == 0)
            await SendDataToEndpoint(webSocket, "You are not the owner of any game", "error");
        else
        {
            GamesInLobby[OwnedGameId].SwitchedTeam = !GamesInLobby[OwnedGameId].SwitchedTeam;
            BroadcastToViewers();
        }
    }
    private async Task GetLobbyInfo(WebSocket webSocket)
    {
        if (GamesInLobby.Any())
        {
            Dictionary<int, LobbyInfoModelOutput> GamesInLobbyOutput = new Dictionary<int, LobbyInfoModelOutput>();

            foreach (var item in GamesInLobby.Values)
            {
                var lobbyOutput = new LobbyInfoModelOutput
                {
                    Id = item.Id,
                    Owner = item.PlayerOne,
                    PlayerOne = item.SwitchedTeam ? item.PlayerTwo : item.PlayerOne,
                    PlayerTwo = item.SwitchedTeam ? item.PlayerOne : item.PlayerTwo,
                    SwitchedTeam = item.SwitchedTeam
                };

                GamesInLobbyOutput.Add(item.Id, lobbyOutput);
            }

            var message = JsonConvert.SerializeObject(GamesInLobbyOutput);

            await SendDataToEndpoint(webSocket, message, "lobbyUpdate");
        }
        else
        {
            await SendDataToEndpoint(webSocket, null, "lobbyUpdate");
        }
    }

    private async Task CloseSession(WebSocket webSocket, WebSocketReceiveResult result, int OwnedGameId, int SignedToGameId)
    {
        ViewersList.Remove(webSocket);

        if (OwnedGameId != 0)
        {
            await DeleteSession(webSocket, OwnedGameId);
        }

        if (SignedToGameId != 0)
        {
            await LeaveSession(webSocket, SignedToGameId);
        }
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    private async Task<int> JoinSession(WebSocket webSocket, string data, IdentityUser user, int SignedToGameId, int OwnedGameId)
    {
        if (SignedToGameId != 0)
            if (!AvailableGamesList.ContainsKey(SignedToGameId))
                SignedToGameId = 0;

        if (OwnedGameId != 0)
            if (!AvailableGamesList.ContainsKey(OwnedGameId))
                OwnedGameId = 0;

        if (OwnedGameId != 0)
        {
            await SendDataToEndpoint(webSocket, "You cannot join another game while you own one", "error");
            return SignedToGameId;
        }

        if (SignedToGameId != 0)
        {
            await SendDataToEndpoint(webSocket, "You are already signed to another game", "error");
            return SignedToGameId;
        }

        if (string.IsNullOrEmpty(data) || data == "NaN")
        {
            await SendDataToEndpoint(webSocket, "No ID selected", "error");
            return SignedToGameId;
        }

        int id = int.Parse(data);

        if (!AvailableGamesList.ContainsKey(id))
        {
            await SendDataToEndpoint(webSocket, "Game do not Exist!", "error");
            return SignedToGameId;
        }

        if (GamesInLobby[id].PlayerTwoId == null && GamesInLobby[id].PlayerOneId != user.Id)
        {
            GamesInLobby[id].PlayerTwo = user.UserName;
            GamesInLobby[id].PlayerTwoId = user.Id;
            AvailableGamesList[id].Add(webSocket);
            BroadcastToViewers();
            return id;
        }
        return SignedToGameId;
    }
    private async Task<int> LeaveSession(WebSocket webSocket, int SignedToGameId)
    {
        if (SignedToGameId == 0)
        {
            await SendDataToEndpoint(webSocket, "You are not signed to any game which can be leaved", "error");
            return SignedToGameId;
        }
        else
        {
            GamesInLobby[SignedToGameId].PlayerTwoId = null;
            GamesInLobby[SignedToGameId].PlayerTwo = null;
            AvailableGamesList[SignedToGameId].Remove(webSocket);
            BroadcastToViewers();
            return 0;
        }
    }
    private async Task<int> DeleteSession(WebSocket webSocket, int OwnedGameId)
    {
        if (OwnedGameId == 0)
        {
            await SendDataToEndpoint(webSocket, "You are not the owner of any game", "error");
            return OwnedGameId;
        }
        else
        {
            GamesInLobby.Remove(OwnedGameId);
            AvailableGamesList.Remove(OwnedGameId);
            BroadcastToViewers();
            return 0;
        }
    }

    private async Task<int> StartSession(WebSocket webSocket, IdentityUser user, int OwnedGameId)
    {
        if (OwnedGameId == 0)
        {
            await SendDataToEndpoint(webSocket, "You are not the owner of any game which can be started", "error");
            return OwnedGameId;
        }
        else
        {
            if (GamesInLobby[OwnedGameId].PlayerOneId == user.Id &&
                GamesInLobby[OwnedGameId].PlayerTwoId != null &&
                GamesInLobby[OwnedGameId].PlayerTwoId != user.Id)
            {
                CreateNewGameModelInput gameOptions = new CreateNewGameModelInput
                {
                    playerTeamWhite = GamesInLobby[OwnedGameId].SwitchedTeam ? GamesInLobby[OwnedGameId].PlayerTwoId : GamesInLobby[OwnedGameId].PlayerOneId,
                    playerTeamBlack = GamesInLobby[OwnedGameId].SwitchedTeam ? GamesInLobby[OwnedGameId].PlayerOneId : GamesInLobby[OwnedGameId].PlayerTwoId,
                    firstTeam = TeamEnum.White
                };
                GameDataSimpleModelOutput game = await _gameInstanceService.Create(gameOptions);
                BroadcastToPlayers(OwnedGameId, game.Id.ToString());
                DeleteSession(webSocket, OwnedGameId);
                BroadcastToViewers();
                return 0;
            }
            await SendDataToEndpoint(webSocket, "You have to find second Player!", "error");
            return OwnedGameId;
        }
    }

    private async Task<int> CreateNewSession(WebSocket webSocket, IdentityUser user, int OwnedGameId, int SignedToGameId)
    {
        if (OwnedGameId != 0)
            if (!AvailableGamesList.ContainsKey(OwnedGameId))
                OwnedGameId = 0;

        if (SignedToGameId != 0)
            if (!AvailableGamesList.ContainsKey(SignedToGameId))
                SignedToGameId = 0;

        if (OwnedGameId != 0)
        {
            await SendDataToEndpoint(webSocket, "You already have your own game", "error");
            return OwnedGameId;
        }

        if (SignedToGameId != 0)
        {
            await SendDataToEndpoint(webSocket, "You cannot create new game, while you are sign to another", "error");
            return OwnedGameId;
        }

        LobbyId++;
        int newGameId = LobbyId;
        if (!AvailableGamesList.ContainsKey(newGameId))
        {
            AvailableGamesList[newGameId] = new List<WebSocket>();
        }
        AvailableGamesList[newGameId].Add(webSocket);
        var game = new LobbyInfoServiceModel
        {
            Id = LobbyId,
            PlayerOneId = user.Id,
            PlayerOne = user.UserName,
            SwitchedTeam = false
        };
        GamesInLobby[newGameId] = game;

        var message = JsonConvert.SerializeObject(newGameId);
        await SendDataToEndpoint(webSocket, message, "ownedGameId");
        BroadcastToViewers();
        return newGameId;

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
                        await SendDataToEndpoint(connection, message, "newGameId");
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
    private async Task SendDataToEndpoint(WebSocket webSocket, string message, string title)
    {
        var dataToSend = new
        {
            action = title,
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
