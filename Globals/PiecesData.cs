public static class Pieces
{
    //{North,South,West,East,NorthWest,NorthEast,SouthWest,SouthEast}
    public static Dictionary<string, PieceMovement> whitePieces = new Dictionary<string, PieceMovement>(){
        //& Normal pieces:
        {"K", new PieceMovement(
            allJumpVal: new int[][]{new int[]{-1,-1},new int[]{0,-1},new int[]{1,-1},new int[]{-1,0},new int[]{1,0},new int[]{-1,1},new int[]{0,1},new int[]{1,1}},
            castlingVal: new int[]{2,2,2,2,2,2,2,2}
        )},
        {"Q", new PieceMovement(
            lineVal: new bool[]{true,true,true,true,true,true,true,true}
        )},
        {"B", new PieceMovement(
            lineVal: new bool[]{false,false,false,false,true,true,true,true}
        )},
        {"N", new PieceMovement(
            allJumpVal: new int[][]{new int[]{-2,1},new int[]{-2,-1},new int[]{-1,2},new int[]{-1,-2},new int[]{1,2},new int[]{1,-2},new int[]{2,1},new int[]{2,-1}}
        )},
        {"R", new PieceMovement(
            lineVal: new bool[]{true,true,true,true,false,false,false,false},
            canBeUsedToCastleVal: true
        )},
        {"P",  new PieceMovement(
            attackJumpVal: new int[][]{new int[]{-1,1},new int[]{1,1}},
            moveJumpVal: new int[][]{new int[]{0,1}},
            initialMoveVal: new Dictionary<int[], int[][]>{{new int[]{0,1,-1},new int[][]{new int[]{0,1}}}},
            enPassantVal: true,
            canPromoteVal: 1,
            blastResistantVal: true
        )},
        
        //& checkers
        //checkers pawn
        {"CP",new PieceMovement(
            moveJumpVal: new int[][]{new int[]{1,1},new int[]{-1,1}},
            checkersAttackVal: new bool[]{false,false,false,false,true,true,false,false},
            canPromoteVal: 1,
            chainCapturesVal: true)},
        //checkers king
        {"CK",new PieceMovement(
            moveJumpVal: new int[][]{new int[]{1,1},new int[]{-1,1},new int[]{1,-1},new int[]{-1,-1}},
            checkersAttackVal: new bool[]{false,false,false,false,true,true,true,true},
            canPromoteVal: 1,
            chainCapturesVal: true)},
            
        //& nothing
        //*Statue (does nothing)
        {"ST",new PieceMovement(
            canBeCapturedVal: false
        )},
        //& fairyChess
        //*Berolina pawn
        {"BP",new PieceMovement(
            attackJumpVal: new int[][]{new int[]{0,1}},
            moveJumpVal: new int[][]{new int[]{1,1},new int[]{-1,1}},
            initialMoveVal: new Dictionary<int[], int[][]>{{new int[]{1,1,-1},new int[][]{new int[]{1,1}}},{new int[]{-1,1,-1},new int[][]{new int[]{-1,1}}}},
            enPassantVal: true,
            canPromoteVal: 1,
            blastResistantVal: true
        )},
        //*ArchBishop
        {"AB",new PieceMovement(
            lineVal: new bool[]{false,false,false,false,true,true,true,true},
            allJumpVal: new int[][]{new int[]{-2,1},new int[]{-2,-1},new int[]{-1,2},new int[]{-1,-2},new int[]{1,2},new int[]{1,-2},new int[]{2,1},new int[]{2,-1}}
        )},
        //* Camel
        {"C", new PieceMovement(
            allJumpVal: new int[][]{new int[]{-3,1},new int[]{-3,-1},new int[]{-1,3},new int[]{-1,-3},new int[]{1,3},new int[]{1,-3},new int[]{3,1},new int[]{3,-1}}
        )},
        //* General
        {"G", new PieceMovement(
            allJumpVal: new int[][]{new int[]{-2,1},new int[]{-2,-1},new int[]{-1,2},new int[]{-1,-2},new int[]{1,2},new int[]{1,-2},new int[]{2,1},new int[]{2,-1},new int[]{-1,-1},new int[]{0,-1},new int[]{1,-1},new int[]{-1,0},new int[]{1,0},new int[]{-1,1},new int[]{0,1},new int[]{1,1}}
        )},
        //* Amazon
        {"A", new PieceMovement(
            lineVal: new bool[]{true,true,true,true,true,true,true,true},
            allJumpVal: new int[][]{new int[]{-2,1},new int[]{-2,-1},new int[]{-1,2},new int[]{-1,-2},new int[]{1,2},new int[]{1,-2},new int[]{2,1},new int[]{2,-1}}
        )},
        //& anarchychess
        //*Knook 
        {"KN",new PieceMovement(
            lineVal: new bool[]{true,true,true,true,false,false,false,false},
            allJumpVal: new int[][]{new int[]{-2,1},new int[]{-2,-1},new int[]{-1,2},new int[]{-1,-2},new int[]{1,2},new int[]{1,-2},new int[]{2,1},new int[]{2,-1}},
            canBeUsedToCastleVal: true)},

        //& Hex Chess
        //* Hex King
        {"HK", new PieceMovement(
            allJumpVal: new int[][]{new int[]{ 0, 1 }, new int[]{ 0, -1 }, new int[]{ -1, 0 }, new int[]{ 1, 0 }, new int[]{ -1, 1 }, new int[]{ 1, 1 }, new int[]{ -1, -1 }, new int[]{ 1, -1 }, new int[]{ 1 , 2 }, new int[]{ 2 , 1 }, new int[]{ -1, -2}, new int[]{ -2, -1}},
            castlingVal: new int[]{2,2,2,2,2,2,2,2,2,2,2,2}
        )},
        //* Hex Queen
        {"HQ", new PieceMovement(
            lineVal: new bool[]{true,true,true,true,true,true,true,true,true,true,true, true}
        )},
        //* Hex Rook
        {"HR", new PieceMovement(
            lineVal: new bool[]{true,    true,      true,    true,    false,     true,        true,     false,     false,     false,     false,      false},
            canBeUsedToCastleVal: true //   { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { 1 , 2 }, { 2 , 1 }, { -1, -2}, { -2, -1} }
        )},
        //* Hex Bishop
        {"HB", new PieceMovement(
            lineVal: new bool[]{false,    false,      false,    false,    true,     false,     false,     true,     true,     true,     true,      true}
                          //   { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { 1 , 2 }, { 2 , 1 }, { -1, -2}, { -2, -1} }
        )},
        //* Hex Knight
        {"HN", new PieceMovement(
            jumpVal: new int[][]{new int[]{-1,2},new int[]{1,3},new int[]{2,3},new int[]{3,2},new int[]{3,1},new int[]{2,-1},new int[]{1,-2},new int[]{-1,-3},new int[]{-2,-3},new int[]{-3,-2},new int[]{-3,-1},new int[]{-2,1}}
        )},
        //* Hex Pawn
        {"HP",  new PieceMovement(
            attackJumpVal: new int[][]{new int[]{-1,0},new int[]{1,1}},
            moveJumpVal: new int[][]{new int[]{0,1}},
            initialMoveVal: new Dictionary<int[], int[][]>{{new int[]{0,1,-1},new int[][]{new int[]{0,1}}}},
            enPassantVal: true,
            canPromoteVal: 1,
            blastResistantVal: true
        )},

        

    };

    static Dictionary<string, PieceMovement> blackPiecesVal;
    public static Dictionary<string, PieceMovement> blackPieces
    {
        get
        {
            if (blackPiecesVal == null)
            {
                CreateOpposite();
            }
            return blackPiecesVal;
        }
        set { blackPiecesVal = value; }
    }

    public static void CreateOpposite()
    {
        blackPiecesVal = new Dictionary<string, PieceMovement>();
        foreach (var item in whitePieces)
        {
            blackPiecesVal[item.Key] = item.Value.CreateOpposite();
        }
    }
}