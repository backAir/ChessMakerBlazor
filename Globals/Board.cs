using System.Globalization;
using System.Net;
using MyApplication.Pages;

public class ChessBoard
{
    //^ Legal moves
    //Dictionary<(destinationX,destinationY),MoveProperties>[startingX,startingY]
    public Dictionary<(int, int), MoveProperties>[,] legalMoves;


    //^ Normal Chess things
    public Piece[,] board;
    public int[] boardSize;
    public int turn; // white or black
    int halfMoveClock;
    int fullMoveNumber; //increases every time black moves
    public int actionsMade = 0; //increases every time an action is made
    public Portal[,] portals;
    public List<(int, int)> enPassantSquares;
    public List<int[,]> allMoves;
    bool temporary = false;
    public Rules rules;
    public bool gameOver = false;
    public bool closedEndGameBox = false;
    public string winnerText;
    public int winner = -1;
    public ChessBoard Clone()
    {
        return new ChessBoard(this, true);
    }
    public ChessBoard(ChessBoard oldBoard, bool temporary)
    {
        this.legalMoves = oldBoard.legalMoves;
        this.board = oldBoard.board.Clone() as Piece[,];;
        this.boardSize = oldBoard.boardSize;
        this.turn = oldBoard.turn;
        this.halfMoveClock = oldBoard.halfMoveClock;
        this.fullMoveNumber = oldBoard.fullMoveNumber;
        // this.castling = castling;
        this.portals = oldBoard.portals.Clone() as Portal[,];
        this.enPassantSquares = oldBoard.enPassantSquares;
        this.allMoves = new List<int[,]>(oldBoard.allMoves);
        this.temporary = temporary;
        this.rules = oldBoard.rules;
    }
    public ChessBoard(string gameModeName)
    {

        GameMode gameMode = GamemodeList.GetGameMode(gameModeName);
        this.rules = gameMode.rules;
        this.boardSize = gameMode.boardSize;

        this.board = new Piece[boardSize[0], boardSize[1]];

        this.portals = new Portal[boardSize[0], boardSize[1]];
        this.allMoves = new List<int[,]>();
        this.enPassantSquares = null;

        // int[][] portalsPos = { new int[] { 3, 2 }, new int[] { 5, 4 }, new int[] { 2, 4 } };
        // AddPortalGroup(180,portalsPos);

        // int[][] portalsPos2 = {new int[]{3,4},new int[]{5,6},new int[]{2,6}};
        // AddPortalGroup(270,portalsPos2);

        SetPosition(gameMode.FEN, boardSize);

        this.legalMoves = Movement.GetAllPseudoLegalMoves(this);
        this.legalMoves = Movement.GetAllLegalMoves(this,legalMoves);

    }

    void AddPortalGroup(int color, int[][] portalsPos)
    {
        var portal = new Portal(color);

        for (var i = 0; i < portalsPos.Length; i++)
        {
            portals[portalsPos[i][0], portalsPos[i][1]] = portal;
            portal.AddLinkedPortal((portalsPos[i][0], portalsPos[i][1]));
        }
    }

    public bool CanMove(int fromX, int fromY, int toX, int toY)
    {

        if (legalMoves[fromX, fromY] == null)
        {
            return false;
        }

        if (legalMoves[fromX, fromY].TryGetValue((toX, toY), out MoveProperties move))
        {
            return true;
        }

        return false;
    }
    public int MovePiece(int fromX, int fromY, int toX, int toY, MyApplication.Pages.Chess PageClass = null)
    {

        actionsMade += 1;
        allMoves.Add(new int[,] { { fromX, fromY }, { toX, toY } });

        bool isCapture = IsCapture(fromX, fromY, toX, toY);


        if(!temporary){
            var sound = isCapture ? Chess.Sound.Capture : Chess.Sound.Move;
            PageClass.PlaySound(sound);
        }

        // prevents the king from moving in atomic chess
        if (temporary && rules.atomic && board[fromX, fromY].IsRoyal(turn) && isCapture)
        {
            return 1;
        }
        
        var piece = board[fromX, fromY];

        var activeColor = turn;
        var opponentColor = (turn == 0) ? 1 : 0;

        var killedOpponentKing = false;

        List<Piece> killedPieces = new();

        DeletePiece(fromX, fromY, opponentColor);
        if (portals[toX, toY] != null)
        {
            // clones the piece on every places where a portal of the same color exist
            foreach (var item in portals[toX, toY].GetDestinations((toX, toY)))
            {
                board[item[0], item[1]] = board[toX, toY];
            }
        }
        enPassantSquares = null;
        // Deletes captured pieces by en passant or checkers captures
        if (legalMoves[fromX, fromY].TryGetValue((toX, toY), out MoveProperties move))
        {
            if (move.capturedPiece != null)
            {
                var killedPiece = DeletePiece(move.capturedPiece[0], move.capturedPiece[1], opponentColor);
                killedPieces.Add(killedPiece);
                if (killedPiece.IsRoyal(opponentColor)) { killedOpponentKing = true; }
            }

            if (move.enPassantSquares != null)
            {
                enPassantSquares = new List<(int, int)>();
                foreach (var item in move.enPassantSquares)
                {
                    AddEnPassantSquare(item);
                }
            }

            if (move.castlingRookStart != null)
            {
                var start = move.castlingRookStart;
                var dest = move.castlingRookDestination;
                PutPiece(dest[0], dest[1], board[start[0], start[1]]);
                DeletePiece(start[0], start[1], opponentColor);

                if(temporary){
                    CastlingLegalityCheck(fromX,fromY,toX,toY,piece);
                }
            }
        }


        if(board[toX,toY] != null){
             killedPieces.Add(board[toX,toY]);
        }

        PutPiece(toX, toY, piece);


        if(isCapture && rules.atomic){
            var explodedPieces = ExplodePiece(toX,toY,opponentColor);
            foreach (var item in explodedPieces)
            {
                killedPieces.Add(item);
            }
        }



        var killedKings = KilledKings(killedPieces);
        if(killedKings.Contains(turn)){
            if (temporary)
            {
                turn = (turn == 0) ? 1 : 0;
                return 1;
            }
        }else if(killedKings.Contains(opponentColor) || killedKings.Contains(-1)){
            killedOpponentKing = true;
        }


        if (temporary)
        {
            turn = (turn == 0) ? 1 : 0;
            return 0;
        }

        var rank = (turn==0) ? toY+1 : boardSize[1]-toY;
        
        if(piece.pieceMovement[turn].canPromote == 1){
            if(CanPromote(toX, toY, turn, rank)){
                var promotesTo = rules.promotePiece;

                piece.PromotePiece(turn,promotesTo);

                piece.pieceString[turn] = promotesTo;
            }
        }

        
        bool chainCapture = isCapture && piece.pieceMovement[turn].chainCaptures;
        if(chainCapture){
            var forcedCaptureFound = true;
            var fakeAllMoves = new Dictionary<(int, int), MoveProperties>[boardSize[0], boardSize[1]];
            var tempLegalMoves = Movement.GetPseudoMoves(this, toX, toY, turn, ref forcedCaptureFound, ref fakeAllMoves);
            if(tempLegalMoves.Count>0){
                fakeAllMoves[toX,toY] = tempLegalMoves;
                legalMoves = fakeAllMoves;
            }else
            {
                chainCapture = false;
            }
        }

        var turnPlayed = turn;

        if(!chainCapture){
            turn = (turn == 0) ? 1 : 0;
        }
        


        //^ Removes castling privilege of the piece
        if (!rules.infiniteCastling)
        {
            piece.canCastle[activeColor] = false;
            piece.canBeUsedToCastle[activeColor] = false;
        }
        
        PageClass.DeselectSquare();
        PageClass.RefreshBoard();

        if(chainCapture){
            PageClass.SelectSquare(toX,toY);
        }

        int gameEnd;
        if (killedOpponentKing)
        {
            // The one that played the last turn wins
            gameEnd = turnPlayed+1;
        }
        else
        {
            if(!chainCapture){
                var pseudoLegalMoves = Movement.GetAllPseudoLegalMoves(this);
                if (rules.checkMate)
                {
                    legalMoves = Movement.GetAllLegalMoves(this, pseudoLegalMoves);
                }
                else
                {
                    legalMoves = pseudoLegalMoves;
                }
            }
            gameEnd = CheckGameEnd();
        }


        switch (gameEnd)
        {
            case 0:
                break;
            case 1:
                winner = 0;
                winnerText = "White Wins";
                gameOver = true;
                break;
            case 2:
                winnerText = "Black Wins";
                gameOver = true;
                break;
            case -1:
                winnerText = "Stalemate";
                gameOver = true;
                break;
            default:
                break;
        }
        return gameEnd;
    }

    /// <summary>
    /// Checks if the piece can promote
    /// </summary>
    bool CanPromote(int x, int y, int color, int rank){
        //rules.promoteZone == -1 && rank == boardSize[1]
        if(rules.promoteZone == -1 && rank == boardSize[1]){
            return true;
        }

        if(rules.promotionSquares != null && rules.promotionSquares[color].Contains((x,y))){
            return true;
        }

        return false;
    }


    /// <summary>
    /// Checks the legality of castling by cloning the king through all squares it can move to while castling.
    /// </summary>
    /// <param name="fromX">The starting x-coordinate of the king.</param>
    /// <param name="fromY">The starting y-coordinate of the king.</param>
    /// <param name="toX">The destination x-coordinate of the king.</param>
    /// <param name="toY">The destination y-coordinate of the king.</param>
    /// <param name="piece">The king piece.</param>
    void CastlingLegalityCheck(int fromX, int fromY, int toX, int toY, Piece piece){
        var x = fromX;
        var y = fromY;
        while (x!=toX || y!=toY)
        {
            PutPiece(x,y,piece);
            
            if(x<toX){
                x++;
            }else if(x>toX){
                x--;
            }

            if(y<toY){
                y++;
            }else if(y>toY){
                y--;
            }
        }
    }

    /// <summary>
    /// Returns a list of the colors of the kings that were killed in the game.
    /// </summary>
    /// <param name="killedPieces">List of pieces that were killed in the game.</param>
    /// <returns>List of the colors of the kings that were killed in the game.</returns>
    public List<int> KilledKings(List<Piece> killedPieces){
        var returnVal = new List<int>();
        foreach (var item in killedPieces)
        {
            if(item.IsRoyalAnyTeam()){
                returnVal.Add(item.color);
            }
        }
        return returnVal;
    }

    /// <summary>
    /// Checks if the game is over or not
    /// </summary>
    /// <returns>
    /// -1 for stalemate
    /// 0 for nothing
    /// 1 for white winning
    /// 2 for black winning
    /// </returns>
    int CheckGameEnd()
    {

        var pieceExist = false;
        for (var x = 0; x < boardSize[0]; x++)
        {
            for (var y = 0; y < boardSize[1]; y++)
            {
                if(board[x,y] == null){
                    continue;
                }
                if (legalMoves[x, y] != null && legalMoves[x, y].Count() > 0)
                {
                    return 0;
                }
                if(board[x,y].IsTeam(turn)){
                    pieceExist = true;
                }
            }
        }
        var otherTeam = turn == 0 ? 1 : 0;
        if (!pieceExist || Movement.IsKingUnderAttack(this, otherTeam))
        {
            return otherTeam + 1;
        }

        return -1;
    }

    bool IsCapture(int fromX, int fromY, int toX, int toY)
    {

        if (this.board[toX, toY] == null)
        {
            // Check for en passant or checkers capture
            if (legalMoves[fromX, fromY][(toX, toY)].capturedPiece != null)
            {
                return true;
            }
            return false;
        }
        else
        {
            //Check for castling
            if (legalMoves[fromX, fromY][(toX, toY)].castlingRookStart != null)
            {
                return false;
            }
            return true;
        }
    }
    //Explode the piece and returns all killed pieces
    List<Piece> ExplodePiece(int xCoord, int yCoord, int color)
    {
        List<Piece> killedPieces = new();
        for (var x = xCoord - 1; x <= xCoord + 1; x++)
        {
            if (x < 0 || x >= boardSize[0])
            {
                continue;
            }
            for (var y = yCoord - 1; y <= yCoord + 1; y++)
            {
                if (y < 0 || y >= boardSize[1] || (xCoord,yCoord) == (x,y))
                {
                    continue;
                }
                if (board[x, y] != null && !board[x, y].IsBlastResistant())
                {
                    var killedPiece = DeletePiece(x, y, color);
                    killedPieces.Add(killedPiece);
                }
            }
        }
        DeletePiece(xCoord, yCoord, color);
        return killedPieces;
    }
    void AddEnPassantSquare((int, int) square)
    {
        enPassantSquares.Add(square);
        if (portals[square.Item1, square.Item2] != null)
        {
            foreach (var item in portals[square.Item1, square.Item2].GetDestinations((square.Item1, square.Item2)))
            {
                enPassantSquares.Add((item[0], item[1]));
            }
        }
    }
    void PutPiece(int x, int y, Piece piece)
    {
        board[x, y] = piece;
        if (portals[x, y] != null)
        {
            // clones the piece on every places where a portal of the same color exist
            foreach (var item in portals[x, y].GetDestinations((x, y)))
            {
                board[item[0], item[1]] = piece;
            }
        }
    }

    // Deletes piece from every portal location
    // Returns the killed piece
    Piece DeletePiece(int x, int y, int color)
    {
        var killedPiece = board[x,y];
        board[x, y] = null;
        if (portals[x, y] != null)
        {
            foreach (var item in portals[x, y].GetDestinations((x, y)))
            {
                board[item[0], item[1]] = null;
            }
        }
        return killedPiece;
    }
    void SetPosition(string betterFEN, int[] boardSize)
    {

        string[] parts = betterFEN.Replace("\n", "").Replace("\r", "").Split(' ');

        if (parts[1][0] == 'w')
        {
            turn = 0;
        }
        else
        {
            turn = 1;
        }
        halfMoveClock = ushort.Parse(parts[4]);
        fullMoveNumber = ushort.Parse(parts[3]);
        enPassantSquares = SharedFunctions.StringToCoordsList(parts[2]);

        // castling = new bool[2,2];
        // if(parts[2] != "-"){
        //     string castlingStr = parts[2];
        //     if(castlingStr.Contains("K")){

        //         castling[0,0] = true;
        //     }
        //     if(castlingStr.Contains("Q")){
        //         castling[0,1] = true;
        //     }
        //     if(castlingStr.Contains("k")){
        //         castling[1,0] = true;
        //     }
        //     if(castlingStr.Contains("q")){
        //         castling[1,1] = true;
        //     }
        // }

        int digitVal;
        string[] position = parts[0].Split('/');
        for (long i = 0; i < position.Length; i++)
        {
            digitVal = 0;
            string[] piecesPosition = position[i].Split(',');
            if (piecesPosition[0] == "")
            {
                continue;
            }
            for (ushort j = 0; j < piecesPosition.Length; j++)
            {
                bool isNumber = int.TryParse(piecesPosition[j], out int numVal);
                if (isNumber)
                {
                    digitVal += numVal - 1;
                    continue;
                }
                string[] fullPieceData = piecesPosition[j].Split('&');

                bool[] royal = new bool[2];
                bool[] canCastle = new bool[2];
                bool[] canBeUsedToCastle = new bool[2];
                int team;
                string[] piece = new string[2] { null, null };
                for (var k = 0; k < fullPieceData.Length; k++)
                {
                    string[] pieceData = fullPieceData[k].Split(':');
                    if (pieceData[1].Contains("b"))
                    {
                        team = 1;
                    }
                    else
                    {
                        team = 0;
                    }
                    piece[team] = pieceData[0];
                    royal[team] = (pieceData[1].Contains("r") ? true : false);
                    canCastle[team] = (pieceData[1].Contains("c") ? false : true);
                    canBeUsedToCastle[team] = (pieceData[1].Contains("u") ? false : true);
                }

                if (piece[0] == null)
                {
                    team = 1;
                }
                else
                {
                    if (piece[1] == null)
                    {
                        team = 0;
                    }
                    else
                    {
                        team = -1;
                    }
                }

                board[j + digitVal, (boardSize[1]) - i - 1] = new Piece(piece, royal, canCastle, canBeUsedToCastle, rules, team);
            }
        }
    }
}