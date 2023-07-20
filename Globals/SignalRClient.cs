using Microsoft.AspNetCore.SignalR.Client;


public class SignalRClient
{
    public HubConnection hub;
    private List<string> messages {get;} = new List<string>();
    public MyApplication.Pages.Chess page = null;
    // private string userInput;
    // private string messageInput;

    public SignalRClient()
    {

    }

    public async Task Connect(){

        #if DEBUG
            var url = "http://localhost:5125/ChessHub";
        #else
            var url = "https://signalr.chessmaker.be/ChessHub";
        #endif
        hub = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                // options.CloseTimeout = TimeSpan.FromSeconds(10);
                options.Headers.Add("user", "identifier"); 
                options.AccessTokenProvider = () => Task.FromResult("hello");
                // options.UseDefaultCredentials = true;
            })
            .WithAutomaticReconnect()
            .Build();

        //Reconnect on disconnect

        hub.Closed += async (error) =>
        {
            Debug.Log("Connection closed");
            await Task.Delay(new Random().Next(0, 5) * 100);
            
            await hub.StartAsync();
            await hub.SendAsync("Reconnect", new ReconnectArgs(page.onlineSessionID,page.onlineGameID));
        };


        hub.On("ReconnectMidGame", async () => {
            Debug.Log("ReconnectMidGame");
            await hub.SendAsync("Reconnect", new ReconnectArgs(page.onlineSessionID,page.onlineGameID));
        });

        hub.On<GameCreatedArgs>("CreateGame", (GameCreatedArgs gameCreatedArgs) =>
        {
            Debug.Log(gameCreatedArgs.gameMode+" "+gameCreatedArgs.playerID);
            page.CreateOnlineGame(gameCreatedArgs);
        });

        hub.On<int>("AllowMoving", (int gameID) =>
        {
            Debug.Log("AllowMoving");
            page.StartOnlineGame();
        });

        hub.On<MovePiece>("MovePiece", (MovePiece movePieceArgs) =>
        {
            Debug.Log("MovePiece");
            page.ReceiveMove(movePieceArgs);
        });
        
        hub.On<ToClientReconnectArgs>("Reconnect", (ToClientReconnectArgs reconnectArgs) =>
        {
            Debug.Log("Reconnect");
            page.Reconnect(reconnectArgs);
        });

        await hub.StartAsync();
    }

    // Send to server
    public void CreateGame(CreateGameArgs createGameArgs){
        Debug.Log("send CreateGame to server");
        hub.SendAsync("CreateGame", createGameArgs);
    }

    public void Reconnect(ReconnectArgs reconnectArgs){
        Debug.Log("Send reconnect to server");
        hub.SendAsync("Reconnect", reconnectArgs);
    }

    public void JoinGame(JoinedGameArgs joinedGameArgs){
        Debug.Log("Send JoinGame to server");
        hub.SendAsync("JoinGame", joinedGameArgs);

    }

    public void SendMove(SendMoveArgs sendMoveArgs){
        hub.SendAsync("SendMove", sendMoveArgs);

    }


}