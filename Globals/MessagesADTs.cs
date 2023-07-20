public static class V
{

    public static Dictionary<string, List<int>> AllClients = new Dictionary<string, List<int>>();
    public static int amountOfGames = 0;
    public static Dictionary<int, Game> AllGames = new Dictionary<int, Game>();

}
public class Game
{
    public string[] originalPlayersIDs = new string[2];
    public string[] players = new string[2];
    public string gameMode;
    public int moveNumber = 0;
    public bool gameStarted = false;
    public List<int[][]> allMoves = new List<int[][]>();
    public int[] lastFrom;
    public int[] lastTo;
    public SemaphoreSlim semaphore = new(1);
    public Game(string gameModeVal)
    {
        gameMode = gameModeVal;
        lastFrom = new int[2];
        lastTo = new int[2];
    }

}


//& Client -> Server
public readonly record struct CreateGameArgs(string gameMode, string userName);

public readonly record struct JoinedGameArgs(int gameID, string userName);

public readonly record struct SendMoveArgs(int[] from, int[] to, int id, int moveNumber);

public readonly record struct ReconnectArgs(string oldPlayerID, int gameID);

//& Server -> Client

public readonly record struct GameCreatedArgs(int gameID, string gameMode, int color, string playerID);

public readonly record struct MovePiece(int[] from, int[] to, int id, int moveNumber);

public readonly record struct ToClientReconnectArgs(List<int[][]> allMoves, int gameID, string gameMode, int color, string playerID, bool gameStarted);

