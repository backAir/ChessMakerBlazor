using System.Collections;
using System.Collections.Generic;

public class GameMode
{
    public string FEN;
    public int[] boardSize;
    public bool BetterFen;
    public Rules rules;
    public GameMode(string FEN, int[] boardSize, bool BetterFen, Rules rules)
    {

        if (!FEN.Contains(':'))
        {
            FEN = SharedFunctions.FENtoBetterFEN(FEN);
        }
        this.FEN = FEN;
        this.boardSize = boardSize;
        this.BetterFen = BetterFen;
        this.rules = rules;
    }
}

public class Rules
{

    public bool checkMate = true; // if disabled you need to kill a royal piece to win instead of checkmating it 
    public bool atomic = false;
    public bool infiniteCastling = false; // if enabled the king never looses castling rights
    public bool kingCanCastleAnything = false;
    public int promoteZone = -1;
    public List<(int,int)>[] promotionSquares = null;
    public List<(int,int)>[] initialMoveSquares = null;
    public int initialMoveRank = 1;
    public string promotePiece = "Q"; 
    public Rules()
    {
    }
}
public static class GamemodeList
{
    public static Dictionary<string,string> GameModes = new Dictionary<string,string>{
        {"castlingTest","test"},
        {"Mini Chess","Tiny board"},
        {"Hex Chess","Hexagon board"},
        {"Everything","Chaos"},
        // {"Shared King","Desctiption"},
        {"Classic","The usual"},
        {"Atomic","Pieces go boom"},
        {"Long Chess","8x12 board"},
        {"10x10","Big board"},
        {"Double Trouble","Two kings is better than one"},
        {"Berolina Chess","Pawns on opposite day"},
        {"XXL", "Massive board"},
        {"Checkers", "Checkers lmao"}
    };

    public static GameMode GetGameMode(string gameMode)
    {
        string FEN;
        int[] boardSize;
        bool BetterFen;

        //^ Rules:
        var rules = new Rules();
        switch (gameMode)
        {
            case "Everything Portal":
            case "Everything":
                FEN = @"R:b,N:b,B:b,AB:b,Q:b:,K:br,KN:b,B:b,N:b,R:b/
                P:b,P:b,BP:b,P:b,P:b,CP:b,P:b,BP:b,P:b,P:b/
                10/10/R:b&B:w,8,R:w&B:b/10/10/
                P:w,P:w,BP:w,P:w,P:w,CP:w,P:w,BP:w,P:w,P:w/
                R:w,N:w,B:w,AB:w,Q:w,K:wr,KN:w,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 10, 9 };
                BetterFen = true;
                rules.infiniteCastling = true;
                rules.kingCanCastleAnything = true;
                break;

            case "test2":
                FEN = "rnbqkb1p/ppp5/3p4/7n/2p5/P1p5/1P1P1PPP/RNB1KBNR w KQq - 6 4";
                boardSize = new int[2] { 8, 8 };
                BetterFen = false;
                break;

            case "castlingTest":
                FEN = @"
                R:b,3,K:rb,2,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                8/8/8/8/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,3,K:rw,2,R:w w - 0 1";
                boardSize = new int[2] { 8, 8 };
                BetterFen = true;
                break;

            case "why":
                FEN = @"rnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrkbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbqrnbq/
pppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp/
/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/100/
PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP/
RNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRKBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQRNBQ w - 0 1".Replace("\n", "").Replace("\r", "");
                boardSize = new int[2] { 100, 100 };
                BetterFen = false;
                rules.checkMate = false;
                break;

            case "Classic Portal":
            case "Classic":
                FEN = @"
                R:b,N:b,B:b,Q:b,K:rb,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                8/8/8/8/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,Q:w,K:rw,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 8, 8 };
                BetterFen = true;
                break;
            case "Atomic":
                FEN = @"R:b,N:b,B:b,Q:b,K:rb,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                8/8/8/8/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,Q:w,K:rw,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 8, 8 };
                BetterFen = true;
                rules.atomic = true;
                break;
            // case "Classic":
            //     FEN = @"8/
            //     8/
            //     8/8/8/8/
            //     8/
            //     K:wr,2,K:br,Q:b w - 0 1";
            //     boardSize = new int[2]{8,8};
            //     BetterFen = true;

            //     rules.atomic = true;
            //     break;

            case "Long Chess Portal":
            case "Long Chess":
                FEN = @"R:b,N:b,B:b,Q:b,K:rb,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                8/8/8/8/8/8/8/8/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,Q:w,K:rw,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 8, 12 };
                BetterFen = true;
                break;

            case "10x10 Portal":
            case "10x10":
                FEN = @"R:b,N:b,B:b,AB:b,Q:b,K:rb,KN:b,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                10/10/10/10/10/10/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,AB:w,Q:w,K:rw,KN:w,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 10, 10 };
                BetterFen = true;
                break;
            case "Shared King":
                FEN = @"R:b,N:b,1,B:b,Q:b,1,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                7/7/4,K:wr&K:br,4/7/7/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,1,Q:w,B:w,1,N:w,R:w w - 0 1";
                boardSize = new int[2] { 9, 9 };
                BetterFen = true;
                break;


            case "test":
                FEN = "r3k2r/ppp2p1p/n3bn1b/3pQ1p1/N2q2P1/2P1BN1B/PP2PP1P/R3K2R w KQkq - 0 12";
                boardSize = new int[2] { 8, 8 };
                BetterFen = false;
                break;

            case "Double Trouble Portal":
            case "Double Trouble":
                FEN = @"R:b,N:b,B:b,K:rb,KN:b,Q:b,K:rb,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                10/10/10/10/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,K:rw,KN:w,Q:w,K:rw,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 10, 8 };
                BetterFen = true;
                break;

            case "Berolina Portal Chess":
            case "Berolina Chess":
                FEN = @"R:b,N:b,B:b,Q:b:,K:br,B:b,N:b,R:b/
                BP:b,BP:b,BP:b,BP:b,BP:b,BP:b,BP:b,BP:b/
                8/8/8/8/
                BP:w,BP:w,BP:w,BP:w,BP:w,BP:w,BP:w,BP:w/
                R:w,N:w,B:w,Q:w:,K:wr,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 8, 8 };
                BetterFen = true;
                break;
            case "XXL":
                FEN = @"R:b,N:b,B:b,AB:b:,C:b,G:b,A:b,K:br,G:b,C:b,KN:b,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b,P:b/
                14/14/14/14/14/14/14/14/14/14/
                P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,AB:w:,C:w,G:w,A:w,K:wr,G:w,C:w,KN:w,B:w,N:w,R:w w - 0 1";
                boardSize = new int[2] { 14, 14};
                BetterFen = true;
                rules.promoteZone = 8;
                break;
            case "Checkers":
                FEN = @"1,CP:b,1,CP:b,1,CP:b,1,CP:b/
                        CP:b,1,CP:b,1,CP:b,1,CP:b,1/
                        1,CP:b,1,CP:b,1,CP:b,1,CP:b/
                        8/8/
                        CP:w,1,CP:w,1,CP:w,1,CP:w,1/
                        1,CP:w,1,CP:w,1,CP:w,1,CP:w/
                        CP:w,1,CP:w,1,CP:w,1,CP:w,1 w - 0 1";
                boardSize = new int[2] { 8, 8};
                BetterFen = true;
                rules.promotePiece = "CK";
                break;
            case "Hex Chess":
                FEN = @"
                ST:b,ST:b,ST:b,ST:b,ST:b,HB:b,HK:brc,HN:b,HR:b,HP:b,1/
                ST:b,ST:b,ST:b,ST:b,HQ:b,HB:b,2,HP:b,2/
                ST:b,ST:b,ST:b,HN:b,1,HB:b,1,HP:b,3/
                ST:b,ST:b,HR:b,3,HP:b,4/
                ST:b,HP:b,HP:b,HP:b,HP:b,HP:b,5/
                11/
                5,HP:w,HP:w,HP:w,HP:w,HP:w,ST:w/
                4,HP:w,3,HR:w,ST:w,ST:w/
                3,HP:w,1,HB:w,1,HN:w,ST:w,ST:w,ST:w/
                2,HP:w,2,HB:w,HK:wrc,ST:w,ST:w,ST:w,ST:w/
                1,HP:w,HR:w,HN:w,HQ:w,HB:w,ST:w,ST:w,ST:w,ST:w,ST:w w - 0 1";
                boardSize = new int[2] { 11, 11};
                BetterFen = true;
                rules.initialMoveSquares = new List<(int,int)>[2]{
                    new List<(int,int)>{(1,0),(2,1),(3,2),(4,3),(5,4),(6,4),(7,4),(8,4),(9,4)},
                    new List<(int,int)>{(1,6),(2,6),(3,6),(4,6),(5,6),(6,7),(7,8),(8,9),(9,10)}
                };
                rules.promotionSquares = new List<(int, int)>[2]{
                    new List<(int, int)>{(0,5),(1,6),(2,7),(3,8),(4,9),(5,10),(6,10),(7,10),(8,10),(9,10),(10,10)},
                    new List<(int, int)>{(0,0),(1,0),(2,0),(3,0),(4,0),(5,0),(6,1),(7,2),(8,3),(9,4),(10,5)}
                };
                rules.initialMoveRank = -1;
                rules.promoteZone = -2;
                rules.promotePiece = "HQ";
                break;
            case "Mini Chess":
                FEN = @"K:br,Q:b,B:b,N:b,R:b/
                P:b,P:b,P:b,P:b,P:b/
                5/
                P:w,P:w,P:w,P:w,P:w/
                R:w,N:w,B:w,Q:w,K:wr w - 0 1";
                boardSize = new int[2] { 5, 5};
                BetterFen = true;
                break;

            default:
                FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                boardSize = new int[2] { 8, 8 };
                BetterFen = false;
                break;
        }

        return new GameMode(FEN.Replace("    ", ""), boardSize, BetterFen, rules);

    }

}
