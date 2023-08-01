using System.Text;
public static class Globals
{


        public static string basePath = "/";//ChessMaker/ when GITHUB (GITHUB THINGY)

    public static string a = "aaaa";
}



public static class SharedFunctions
{
    // clip-path: polygon(100% 0%,100% 100%,50% 100%,50% 0%);
    // clip-path: polygon(50% 0%,50% 100%,0% 100%,0% 0%);


    public static string GetPiecePng(string piece, int color){
        if(piece == "ST"){
            return null;
        }
        return Globals.basePath + "images/" + piece + (color == 0? "w" : "b") + ".png";;
    }

    public static void GetRenderedBoard(ChessBoard board, string[,,] boardPNGs)
    {
        for (int x = 0; x < board.boardSize[0]; x++)
        {
            for (int y = 0; y < board.boardSize[1]; y++)
            {
                if (board.board[x, y] != null)
                {
                    if (board.board[x, y].color != -1)
                    {
                        boardPNGs[x, y, 0] = GetPiecePng(board.board[x, y].GetSprite(board.board[x, y].color),board.board[x, y].color);
                        boardPNGs[x, y, 1] = null;
                    }
                    else
                    {
                        boardPNGs[x, y, 0] = Globals.basePath + "images/" + board.board[x, y].GetSprite(0) + "w.png?1";
                        boardPNGs[x, y, 1] = Globals.basePath + "images/" + board.board[x, y].GetSprite(1) + "b.png?1";
                    }
                }
                else
                {
                    boardPNGs[x, y, 0] = null;
                    boardPNGs[x, y, 1] = null;
                }
            }
        }
    }

    public static string[,] GetRenderedPortals(ChessBoard board)
    {
        var renderedPortals = new string[board.boardSize[0], board.boardSize[1]];
        for (int x = 0; x < board.boardSize[0]; x++)
        {
            for (int y = 0; y < board.boardSize[1]; y++)
            {
                if (board.portals[x, y] != null)
                {
                    renderedPortals[x, y] = board.portals[x, y].hue + "deg";
                }
                else
                {
                    renderedPortals[x, y] = null;
                }
            }
        }
        return renderedPortals;
    }
    public static int[] StringToCoordsArray(string boardSquare)
    {
        Int16.TryParse(boardSquare.Substring(1), out short j);
        return new int[] { (boardSquare[0] - 97), j };
    }
    public static List<(int, int)> StringToCoordsList(string boardSquare)
    {
        if (boardSquare == "-")
        {
            return null;
        }
        var output = new List<(int, int)>();
        Int16.TryParse(boardSquare.Substring(1), out short j);
        output.Add(((boardSquare[0] - 97), j));
        return output;
    }



    public static string FENtoBetterFEN(string fen)
    // written by chatGPT for the luls
    {
        StringBuilder betterFen = new StringBuilder();
        string[] fenParts = fen.Split(' ');

        // Convert the piece placement section of the FEN
        string[] ranks = fenParts[0].Split('/');
        foreach (string rank in ranks)
        {
            int emptyCount = 0;
            for (int i = 0; i < rank.Length; i++)
            {
                char c = rank[i];
                if (Char.IsDigit(c))
                {
                    emptyCount = emptyCount * 10 + (c - '0');
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        betterFen.Append("," + emptyCount.ToString());
                        emptyCount = 0;
                    }
                    betterFen.Append("," + FENPieceToBetterFEN(c));
                }
            }
            if (emptyCount > 0)
            {
                betterFen.Append("," + emptyCount.ToString());
            }
            betterFen.Append("/");
        }
        betterFen.Remove(betterFen.Length - 1, 1); // remove last '/'

        for (int i = 1; i < fenParts.Length; i++)
        {
            betterFen.Append(" " + fenParts[i]);
        }

        betterFen = betterFen.Replace("/,", "/").Replace(",/", "/");
        if (betterFen[0] == ',')
        {
            betterFen.Remove(0, 1);
        }
        return betterFen.ToString();
    }


    static string FENPieceToBetterFEN(char c)
    {
        string betterFenPiece = "";
        switch (c)
        {
            case 'K':
                betterFenPiece = "K:rw";
                break;
            case 'Q':
                betterFenPiece = "Q:w";
                break;
            case 'R':
                betterFenPiece = "R:w";
                break;
            case 'B':
                betterFenPiece = "B:w";
                break;
            case 'N':
                betterFenPiece = "N:w";
                break;
            case 'P':
                betterFenPiece = "P:w";
                break;
            case 'k':
                betterFenPiece = "K:rb";
                break;
            case 'q':
                betterFenPiece = "Q:b";
                break;
            case 'r':
                betterFenPiece = "R:b";
                break;
            case 'b':
                betterFenPiece = "B:b";
                break;
            case 'n':
                betterFenPiece = "N:b";
                break;
            case 'p':
                betterFenPiece = "P:b";
                break;
            default:
                break;
        }
        return betterFenPiece;
    }
}
