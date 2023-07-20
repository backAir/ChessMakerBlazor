using System.Collections.Specialized;
using System.Reflection.Metadata.Ecma335;

public static class Movement
{

    private static readonly int[,] MoveDirections = { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { 1 , 2 }, { 2 , 1 }, { -1, -2}, { -2, -1} };

    public static Dictionary<(int, int), MoveProperties>[,] GetAllLegalMoves(ChessBoard chessBoard, Dictionary<(int, int),
        MoveProperties>[,] pseudoLegalMoves)
    {
        var legalMoves = new Dictionary<(int, int), MoveProperties>[chessBoard.boardSize[0], chessBoard.boardSize[1]];
        for (var x = 0; x < pseudoLegalMoves.GetLength(0); x++)
        {
            for (var y = 0; y < pseudoLegalMoves.GetLength(1); y++)
            {
                if (pseudoLegalMoves[x, y] != null && chessBoard.board[x, y].IsTeam(chessBoard.turn))
                {
                    legalMoves[x, y] = GetLegalMoves(chessBoard, x, y, pseudoLegalMoves);
                }
            }
        }

        return legalMoves;
    }

    public static Dictionary<(int, int), MoveProperties> GetLegalMoves(ChessBoard chessBoard, int x, int y,
    Dictionary<(int, int), MoveProperties>[,] pseudoLegalMoves)
    {
        var legalMoves = new Dictionary<(int, int), MoveProperties>();


        foreach (KeyValuePair<(int, int), MoveProperties> move in pseudoLegalMoves[x, y])
        {
            var clonedBoard = chessBoard.Clone();
            clonedBoard.legalMoves = pseudoLegalMoves;
            var KingDead = clonedBoard.MovePiece(x, y, move.Key.Item1, move.Key.Item2);

            if (KingDead == 0 && !IsKingUnderAttack(clonedBoard, clonedBoard.turn))
            {
                legalMoves.Add(move.Key, move.Value);
            }
        }

        return legalMoves;
    }

    public static bool IsKingUnderAttack(ChessBoard chessBoard, int attackingColor)
    {

        for (int x = 0; x < chessBoard.boardSize[0]; x++)
        {
            for (int y = 0; y < chessBoard.boardSize[1]; y++)
            {
                // Checks if there's a piece of the active team on the square [x,y]
                if (chessBoard.board[x, y] != null && chessBoard.board[x, y].IsTeam(attackingColor))
                {
                    var piece = chessBoard.board[x, y].pieceMovement[attackingColor].FakeClone();

                    foreach (var item in piece.moveTypes)
                    {
                        switch (item)
                        {

                            case "L":
                                if (GetPseudoMovesLine(chessBoard, attackingColor, piece, x, y, null))
                                {
                                    return true;
                                };
                                break;
                            case "J":
                                if (GetPseudoMovesJump(chessBoard, attackingColor, piece, x, y, null))
                                {
                                    return true;
                                };
                                break;
                            // case "I":
                            //     if(GetPseudoInitialMoves(chessBoard, attackingColor, piece, x, y, null)){
                            //         return true;
                            //     };
                            //     break;
                            // TODO: add castling thing here
                            // case "C":
                            //     GetPseudoCastling(board, color, x, y, legalMoves);
                            //     break;
                            case "CK":
                                if (GetPseudoCheckers(chessBoard, attackingColor, piece, x, y, null))
                                {
                                    return true;
                                };
                                break;
                        }
                    }
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Gets all of the pseudo legal moves for the active team
    /// </summary>
    /// <param name="chessBoard"></param>
    public static Dictionary<(int, int), MoveProperties>[,] GetAllPseudoLegalMoves(ChessBoard chessBoard)
    {
        var allMoves = new Dictionary<(int, int), MoveProperties>[chessBoard.boardSize[0], chessBoard.boardSize[1]];
        for (int x = 0; x < chessBoard.boardSize[0]; x++)
        {
            for (int y = 0; y < chessBoard.boardSize[1]; y++)
            {
                // Checks if there's a piece of the active team on the square [x,y]
                if (chessBoard.board[x, y] != null && chessBoard.board[x, y].IsTeam(chessBoard.turn))
                {
                    allMoves[x, y] = GetPseudoMoves(chessBoard, x, y, chessBoard.turn);
                }
            }
        }
        return allMoves;
    }

    static Dictionary<(int, int), MoveProperties> GetPseudoMoves(ChessBoard chessBoard, int x, int y, int color)
    {

        var moves = new Dictionary<(int, int), MoveProperties>();

        var piece = chessBoard.board[x, y].pieceMovement[color];

        foreach (var item in piece.moveTypes)
        {
            switch (item)
            {

                case "L":
                    GetPseudoMovesLine(chessBoard, color, piece, x, y, moves);
                    break;
                case "J":
                    GetPseudoMovesJump(chessBoard, color, piece, x, y, moves);
                    break;
                case "I":
                    GetPseudoInitialMoves(chessBoard, color, piece, x, y, moves);
                    break;
                case "C":
                    GetPseudoCastling(chessBoard, color, piece, x, y, moves);
                    break;
                case "CK":
                    GetPseudoCheckers(chessBoard, color, piece, x, y, moves);
                    break;
            }
        }
        return moves;
    }

    static bool CanDoInitialMove(ChessBoard chessBoard, int color, int x, int y, int pieceInitialMoveRank)
    {
        if(chessBoard.rules.initialMoveRank == -2){
            return true;
        }


        if(chessBoard.rules.initialMoveSquares != null){
            var homeSquares = chessBoard.rules.initialMoveSquares[color];
            for (var i = 0; i < homeSquares.Count; i++)
            {
                if(x == homeSquares[i].Item1 && y == homeSquares[i].Item2){
                    return true;
                }
            }
        }

        if(chessBoard.rules.initialMoveRank == -1){
            return false;
        }

        if (pieceInitialMoveRank == -2 || chessBoard.rules.initialMoveRank < 0)
        {
            return true;
        }
        if (pieceInitialMoveRank == -1)
        {
            pieceInitialMoveRank = chessBoard.rules.initialMoveRank;
        }
        if (color == 0)
        {
            return pieceInitialMoveRank == y;
        }
        else
        {
            //if the piece is black the formula is different cuz it goes down instead of up
            return pieceInitialMoveRank == chessBoard.boardSize[1] - 1 - y;
        }
    }

    static void GetPseudoCastling(ChessBoard chessBoard, int color, PieceMovement pieceMovement, int x, int y,
    Dictionary<(int, int), MoveProperties> moves)
    {
        for (int i = 0; i < MoveDirections.GetLength(0); i++)
        {
            if (pieceMovement.castling[i] == -1)
            {
                continue;
            }

            int xCoords = x;
            int yCoords = y;

            int squareMoved = 0;
            int[] kingDest = null;
            int[] rookDest = null;

            var kingPath = new List<(int,int)>();
            while (true)
            {
                var stepDest = MovePiece(xCoords, yCoords, MoveDirections[i, 0], MoveDirections[i, 1], chessBoard, null, false);
                if (stepDest.Count == 0 || chessBoard.portals[xCoords, yCoords] != null)
                {
                    break;
                }
                var dest = stepDest[0];
                
                


                squareMoved++;
                if (squareMoved == pieceMovement.castling[1])
                {
                    kingDest = new int[] { dest[0], dest[1] };
                    rookDest = new int[] { xCoords, yCoords };
                }

                if (chessBoard.board[dest[0], dest[1]] != null)
                {
                    var targetPiece = chessBoard.board[dest[0], dest[1]];
                    if (targetPiece.CanBeUsedToCastle(color))
                    {
                        if (kingDest == null)
                        {

                            kingDest = new int[] { dest[0], dest[1] };
                            rookDest = new int[] { xCoords, yCoords };

                        }
                        MoveProperties moveProperties = new MoveProperties();
                        moveProperties.castlingRookStart = dest;
                        moveProperties.castlingRookDestination = rookDest;

                        AddLegalMove(kingDest[0], kingDest[1], moves, chessBoard, moveProperties);
                    }
                    break;
                }

                xCoords = dest[0];
                yCoords = dest[1];

            }
        }
    }
    static void GetPseudoInitialMoves(ChessBoard chessBoard, int color, PieceMovement pieceMovement, int x, int y,
    Dictionary<(int, int), MoveProperties> moves)
    {


        // Dict example for berolina pawn: Dictionary<int[], int[][]>{{new int[]{2,2,-1},new int[][]{new int[]{1,1}}},{new int[]{-2,2,-1},new int[][]{new int[]{-1,1}}}}
        foreach (KeyValuePair<int[], int[][]> entry in pieceMovement.initialMove)
        {
            if (CanDoInitialMove(chessBoard, color, x, y, entry.Key[2]))
            {
                GetPseudoInitialMove(chessBoard, color, entry.Key, entry.Value, x, y, moves);
            }
        }
    }

    static void GetPseudoInitialMove(ChessBoard chessBoard, int color, int[] destination, int[][] path, int x, int y,
    Dictionary<(int, int), MoveProperties> moves, int step = 0, int[] currentPos = null, MoveProperties moveProperties = new MoveProperties())
    {

        var currentX = currentPos == null ? x : currentPos[0];
        var currentY = currentPos == null ? y : currentPos[1];
        for (var currentStep = step; currentStep < path.Length; currentStep++)
        {
            var dest = MovePiece(currentX, currentY, path[currentStep][0], path[currentStep][1], chessBoard);
            if (dest.Count == 0)
            {
                return;
            }
            currentX = dest[0][0];
            currentY = dest[0][1];

            if (chessBoard.board[currentX, currentY] != null)
            {
                return;
            }

            //TODO: not forget that in Chessboard.MovePiece() I need to add every portal that an en passant spot is in
            //TODO: in the future add a way to choose what path a piece does when making a move since they're might be different possible paths (after making a move the game highlights the different possible paths)
            moveProperties.AddEnPassantSquare((dest[0][0], dest[0][1]));

            for (var j = 1; j < dest.Count; j++)
            {
                var newCurrentPos = new int[] { dest[j][0], dest[j][1] };
                var newMoveProperties = moveProperties.Clone();
                GetPseudoInitialMove(chessBoard, color, destination, path, x, y, moves, currentStep + 1, newCurrentPos, newMoveProperties);
            }
        }
        var finalDest = MovePiece(currentX, currentY, destination[0], destination[1], chessBoard);


        for (var i = 0; i < finalDest.Count(); i++)
        {
            int xCoords = finalDest[i][0];
            int yCoords = finalDest[i][1];
            if (chessBoard.board[xCoords, yCoords] == null)
            {
                AddLegalMove(xCoords, yCoords, moves, chessBoard, moveProperties);
            }
        }

    }

    static bool GetPseudoCheckers(ChessBoard chessBoard, int color, PieceMovement pieceMovement, int x, int y, Dictionary<(int, int),
    MoveProperties> moves)
    {
        var enemyColor = (color == 0 ? 1 : 0);

        for (var i = 0; i < MoveDirections.GetLength(0); i++)
        {
            if (!pieceMovement.checkersAttack[i])
            {
                continue;
            }
            var dest = MovePiece(x, y, MoveDirections[i, 0], MoveDirections[i, 1], chessBoard, null, false);
            if (dest.Count == 0)
            {
                continue;
            }

            var destinationSquare = chessBoard.board[dest[0][0], dest[0][1]];
            if (destinationSquare != null && destinationSquare.CanBeCapturedBy(color))
            {
                var dest2 = MovePiece(dest[0][0], dest[0][1], MoveDirections[i, 0], MoveDirections[i, 1], chessBoard, null, false);
                if (dest2.Count == 0)
                {
                    continue;
                }
                if (chessBoard.board[dest2[0][0], dest2[0][1]] == null)
                {
                    //^ KingCheck
                    if (moves == null)
                    {
                        if (chessBoard.board[dest[0][0], dest[0][1]].IsRoyal(enemyColor))
                        {
                            return true;
                        }
                        break;
                    }
                    var moveProperties = new MoveProperties(new int[] { dest[0][0], dest[0][1] });
                    AddLegalMove(dest2[0][0], dest2[0][1], moves, chessBoard, moveProperties);
                }
            }
        }
        return false;
    }
    static bool GetPseudoMovesLine(ChessBoard chessBoard, int color, PieceMovement pieceMovement, int x, int y,
                Dictionary<(int, int), MoveProperties> moves, List<(int, int)> portalsExited = null, int dirrection = -1)
    {
        var enemyColor = (color == 0 ? 1 : 0);
        // if values don't exist I just put them in so my code doesn't crash
        if (pieceMovement.attackLine == null)
        {
            pieceMovement.attackLine = new bool[12];
        }
        else if (pieceMovement.moveLine == null)
        {
            pieceMovement.moveLine = new bool[12];
        }




        for (int i = 0; i < MoveDirections.GetLength(0); i++)
        {
            if (!pieceMovement.attackLine[i] && !pieceMovement.moveLine[i])
            {
                continue;
            }
            if (dirrection != -1)
            {
                if (dirrection != i)
                {
                    continue;
                }
            }
            else
            {
                portalsExited = new List<(int, int)>();
            }

            int xCoords = x;
            int yCoords = y;

            while (true)
            {
                var stepDest = MovePiece(xCoords, yCoords, MoveDirections[i, 0], MoveDirections[i, 1], chessBoard, portalsExited);
                if (stepDest.Count == 0)
                {
                    break;
                }
                xCoords = stepDest[0][0];
                yCoords = stepDest[0][1];

                if (chessBoard.board[xCoords, yCoords] == null)
                {
                    var enPassant = CanEnPassant(chessBoard, pieceMovement, (xCoords, yCoords), color);
                    if (enPassant != null && pieceMovement.attackLine[i])
                    {
                        if (chessBoard.board[enPassant[0], enPassant[1]].CanBeCapturedBy(color))
                        {
                            //^ KingCheck
                            if (moves == null)
                            {
                                if (chessBoard.board[enPassant[0], enPassant[1]].IsRoyal(enemyColor))
                                {
                                    return true;
                                }
                                break;
                            }
                            var moveProperties = new MoveProperties(new int[] { enPassant[0], enPassant[1] });
                            AddLegalMove(xCoords, yCoords, moves, chessBoard, moveProperties);
                        }
                        break;
                    }
                    if (pieceMovement.moveLine[i])
                    {
                        AddLegalMove(xCoords, yCoords, moves, chessBoard);
                    }
                    if (stepDest.Count > 1)
                    {
                        for (var j = 1; j < stepDest.Count; j++)
                        {
                            GetPseudoMovesLine(chessBoard, color, pieceMovement, stepDest[j][0], stepDest[j][1], moves,
                                                portalsExited, i);
                        }
                        continue;
                    }
                }
                else
                {
                    if (pieceMovement.attackLine[i] && chessBoard.board[xCoords, yCoords].CanBeCapturedBy(color))
                    {
                        //^ KingCheck
                        if (moves == null)
                        {
                            if (chessBoard.board[xCoords, yCoords].IsRoyal(enemyColor))
                            {
                                return true;
                            }
                            break;
                        }
                        AddLegalMove(xCoords, yCoords, moves, chessBoard);
                    }
                    break;
                }
            }
        }


        return false;

    }

    static bool GetPseudoMovesJump(ChessBoard chessBoard, int color, PieceMovement pieceMovement,
                int x, int y, Dictionary<(int, int), MoveProperties> moves)
    {
        var enemyColor = (color == 0 ? 1 : 0);
        if (pieceMovement.moveJump != null)
        {
            foreach (var item in pieceMovement.moveJump)
            {
                var stepDest = MovePiece(x, y, item[0], item[1], chessBoard);
                if (stepDest.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < stepDest.Count(); i++)
                {
                    int xCoords = stepDest[i][0];
                    int yCoords = stepDest[i][1];
                    if (chessBoard.board[xCoords, yCoords] == null)
                    {
                        AddLegalMove(xCoords, yCoords, moves, chessBoard);
                    }
                }
            }
        }
        if (pieceMovement.attackJump != null)
        {
            foreach (var item in pieceMovement.attackJump)
            {
                var stepDest = MovePiece(x, y, item[0], item[1], chessBoard);
                if (stepDest.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < stepDest.Count(); i++)
                {
                    int xCoords = stepDest[i][0];
                    int yCoords = stepDest[i][1];
                    if (chessBoard.board[xCoords, yCoords] == null)
                    {
                        var enPassant = CanEnPassant(chessBoard, pieceMovement, (xCoords, yCoords), color);
                        if (enPassant != null)
                        {
                            //^ KingCheck
                            if (moves == null)
                            {
                                if (chessBoard.board[enPassant[0], enPassant[1]].IsRoyal(enemyColor))
                                {
                                    return true;
                                }
                                break;
                            }
                            if (chessBoard.board[enPassant[0], enPassant[1]].CanBeCapturedBy(color))
                            {
                                var moveProperties = new MoveProperties(new int[] { enPassant[0], enPassant[1] });
                                AddLegalMove(xCoords, yCoords, moves, chessBoard, moveProperties);
                            }
                            break;
                        }
                    }
                    else if (chessBoard.board[xCoords, yCoords].CanBeCapturedBy(color))
                    {
                        //^ KingCheck
                        if (moves == null)
                        {
                            if (chessBoard.board[xCoords, yCoords].IsRoyal(enemyColor))
                            {
                                return true;
                            }
                            break;
                        }
                        AddLegalMove(xCoords, yCoords, moves, chessBoard);
                    }
                }
            }
        }
        if (pieceMovement.allJump != null)
        {
            foreach (var item in pieceMovement.allJump)
            {
                var stepDest = MovePiece(x, y, item[0], item[1], chessBoard);
                if (stepDest.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < stepDest.Count(); i++)
                {
                    int xCoords = stepDest[i][0];
                    int yCoords = stepDest[i][1];
                    if (chessBoard.board[xCoords, yCoords] == null || chessBoard.board[xCoords, yCoords].CanBeCapturedBy(color))
                    {
                        //^ KingCheck
                        if (moves == null)
                        {
                            if (chessBoard.board[xCoords, yCoords] != null && chessBoard.board[xCoords, yCoords].IsRoyal(enemyColor))
                            {
                                return true;
                            }
                            break;
                        }
                        AddLegalMove(xCoords, yCoords, moves, chessBoard);
                    }
                }
            }
        }
        return false;
    }

    static void AddLegalMove(int toX, int toY, Dictionary<(int, int), MoveProperties> moves, ChessBoard chessBoard,
                MoveProperties moveProperties = new MoveProperties())
    {
        moves[(toX, toY)] = moveProperties;
        if (chessBoard.portals[toX, toY] != null)
        {
            foreach (var item in chessBoard.portals[toX, toY].GetDestinations((toX, toY)))
            {
                moves[(item[0], item[1])] = moveProperties;
            }
        }
    }

    static List<int[]> MovePiece(int xPos, int yPos, int XDisplacement, int YDisplacement, ChessBoard chessBoard,
                                List<(int, int)> portalsExited = null, bool portals = true)
    {
        var destinations = new List<int[]>();

        var destX = xPos + XDisplacement;
        var destY = yPos + YDisplacement;

        if (destX >= 0 && destX < chessBoard.boardSize[0] && destY >= 0 && destY < chessBoard.boardSize[1])
        {


            if (chessBoard.portals[destX, destY] != null && portals == true)
            {
                foreach (var item in chessBoard.portals[destX, destY].GetDestinations((destX, destY)))
                {
                    if (portalsExited != null)
                    {
                        if (portalsExited.Contains((item[0], item[1])))
                        {
                            continue;
                        }
                        else
                        {
                            portalsExited.Add((item[0], item[1]));
                        }
                    }
                    destinations.Add(new int[] { item[0], item[1] });
                }
            }
            else
            {
                destinations.Add(new int[] { destX, destY });
            }
        }

        return destinations;
    }

    static int[] CanEnPassant(ChessBoard chessBoard, PieceMovement pieceMovement, (int, int) to, int color)
    {
        if (chessBoard.enPassantSquares == null)
        {
            return null;
        }
        if (pieceMovement.enPassant && chessBoard.enPassantSquares.Contains(to))
        {
            var lastMove = chessBoard.allMoves[chessBoard.allMoves.Count - 1];
            return new int[] { lastMove[1, 0], lastMove[1, 1] };
        }
        return null;
    }
}

public struct MoveProperties
{
    public MoveProperties(int[] capturedPiece)
    {
        this.capturedPiece = capturedPiece;
        enPassantSquares = null;
        castlingRookStart = null;
        castlingRookDestination = null;
    }
    public MoveProperties(List<(int, int)> enPassantSquares)
    {
        capturedPiece = null;
        this.enPassantSquares = enPassantSquares;
        castlingRookStart = null;
        castlingRookDestination = null;
    }
    public void AddEnPassantSquare((int, int) square)
    {
        if (enPassantSquares == null)
        {
            enPassantSquares = new List<(int, int)>();
        }
        enPassantSquares.Add(square);
    }
    public MoveProperties Clone()
    {
        var newMoveProperties = new MoveProperties();
        if (capturedPiece != null) { newMoveProperties.capturedPiece = capturedPiece.Clone() as int[]; }
        newMoveProperties.enPassantSquares = new List<(int, int)>(enPassantSquares);
        return newMoveProperties;
    }
    public int[] capturedPiece;
    public List<(int, int)> enPassantSquares;
    public int[] castlingRookStart;
    public int[] castlingRookDestination;
}