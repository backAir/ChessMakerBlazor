public class Piece
{
    public int color;
    public PieceMovement[] pieceMovement = new PieceMovement[2];
    public string[] pieceString = new string[2];
    public bool[] royal = new bool[2];
    public bool[] canCastle = new bool[2];
    public bool[] canBeUsedToCastle = new bool[2];
    public bool[] blastResistant = new bool[2];
    public Piece(string[] piece, bool[] royal, bool[] canCastle, bool[] canBeUsedToCastle, Rules rules, int color)
    {


        this.pieceString = piece;
        this.royal = royal;
        this.color = color;
        for (var i = 0; i < 2; i++)
        {
            if (color != i && color != -1)
            {
                continue;
            }
            if (i == 0)
            {
                this.pieceMovement[i] = Pieces.whitePieces[piece[i]].Clone();
            }
            else
            {
                this.pieceMovement[i] = Pieces.blackPieces[piece[i]].Clone();
            }

            //^this makes every piece able to castle
            // this.pieceMovement.castling = new int[]{2,2,2,2,2,2,2,2};
            // this.pieceMovement.FinishPiece();


            if (canCastle[i])
            {
                this.canCastle[i] = this.pieceMovement[i].castling != null;
            }
            else
            {
                this.canCastle[i] = false;
            }

            if (canBeUsedToCastle[i])
            {
                this.canBeUsedToCastle[i] = this.pieceMovement[i].canBeUsedToCastle;
            }
            else
            {
                this.canBeUsedToCastle[i] = false;
            }

            //^rules thingy
            if (rules.kingCanCastleAnything)
            {
                this.canBeUsedToCastle[i] = true;
            }

            this.blastResistant[i] = this.pieceMovement[i].blastResistant;
        }



    }

    public void PromotePiece(int color, string piece){
        if(color == 0){
            pieceMovement[color] = Pieces.whitePieces[piece];
        }else
        {
            pieceMovement[color] = Pieces.blackPieces[piece];
        }
        pieceString[color] = piece;
        canBeUsedToCastle[color] = pieceMovement[color].canBeUsedToCastle;
        blastResistant[color] = pieceMovement[color].blastResistant;

    }
    public bool IsBlastResistant(){
        if(color == -1){
            for (var i = 0; i < blastResistant.Length; i++){
                if(blastResistant[i]){
                    return true;
                }
            }
            return false;
        }
        return blastResistant[color];
    }
    public string GetSprite(int color)
    {
        return pieceString[color];
    }
    public bool IsTeam(int color)
    {
        // TODO: add this.color == -1
        return this.color == color || this.color == -1;
    }
    public bool CanBeUsedToCastle(int color)
    {
        if (this.color == color || this.color == -1)
        {
            return canBeUsedToCastle[color];
        }
        return false;
    }

    public bool CanBeCapturedBy(int color)
    {
        if(this.color >= 0){
            if(!this.pieceMovement[this.color].canBeCaptured){
                return false;
            }
        }
        // if(this.color >=0 && this.pieceString[this.color]=="ST"){
        //     Debug.Log("statue thingy: "+this.pieceMovement[this.color].canBeCaptured+" "+this.color);
        // }
        return this.color != color;
    }

    public bool IsRoyal(int color)
    {
        return royal[color];
    }
    public bool IsRoyalAnyTeam()
    {
        foreach (var item in royal)
        {
            if(item == true){
                return true;
            }
        }
        return false;
    }
}

public class PieceMovement
{
    public bool[] attackLine; //{North,South,West,East,NorthWest,NorthEast,SouthWest,SouthEast}
    public bool[] moveLine;
    public int[][] attackJump;
    public int[][] moveJump;
    public int[][] allJump;
    public bool[] checkersAttack;
    public bool[] checkersQueenAttack;
    // public somthing castling;
    public Dictionary<int[], int[][]> initialMove; //Dictionary<destination,squares to check if valid and can be attacked by enpassant>
    public bool enPassant;
    public byte canPromote = 0;
    public bool canBeCaptured = true;
    public int[] castling;
    public bool canBeUsedToCastle;
    public List<string> moveTypes;
    public bool blastResistant;
    public bool chainCaptures = false;
    public PieceMovement Clone()
    {
        var newAttack = attackLine == null ? null : attackLine.Clone() as bool[];
        var newMove = moveLine == null ? null : moveLine.Clone() as bool[];
        var newAttackJump = CloneArray(attackJump);
        var newMoveJump = CloneArray(moveJump);
        var newJump = CloneArray(allJump);
        var newCheckersAttack = checkersAttack == null ? null : checkersAttack.Clone() as bool[];
        var newcheckersQueenAttack = checkersQueenAttack == null ? null : checkersQueenAttack.Clone() as bool[];
        var newInitialMove = CloneDictionary(initialMove);
        var newCastling = castling == null ? null : castling.Clone() as int[];
        var newCanBeUsedToCastle = canBeUsedToCastle;
        var newBlastResistant = blastResistant;
        var newCanbeCaptured = canBeCaptured;
        var newChainCaptures = chainCaptures;
        return new PieceMovement(newAttack, newMove, newAttackJump, newMoveJump, newInitialMove, enPassant, canPromote, newCheckersAttack, newcheckersQueenAttack, newCastling, newJump, newCanBeUsedToCastle, newBlastResistant, newCanbeCaptured, newChainCaptures);
    }

    // Clone for the fake boards (removes non attacking moves better for legal move calc)
    public PieceMovement FakeClone()
    {
        var newAttack = attackLine == null ? null : attackLine.Clone() as bool[];
        var newMove = null as bool[];
        var newAttackJump = CloneArray(attackJump);
        var newMoveJump = null as int[][];
        var newJump = CloneArray(allJump);
        var newCheckersAttack = checkersAttack == null ? null : checkersAttack.Clone() as bool[];
        var newcheckersQueenAttack = checkersQueenAttack == null ? null : checkersQueenAttack.Clone() as bool[];
        var newInitialMove = null as Dictionary<int[], int[][]>;
        var newCastling = castling == null ? null : castling.Clone() as int[];
        var newCanBeUsedToCastle = canBeUsedToCastle;
        var newChainCaptures = chainCaptures;
        return new PieceMovement(newAttack, newMove, newAttackJump, newMoveJump, newInitialMove, enPassant, canPromote, newCheckersAttack, newcheckersQueenAttack, newCastling, newJump, newCanBeUsedToCastle, blastResistant, canBeCaptured,newChainCaptures);
    }

    public PieceMovement CreateOpposite()
    {
        var newAttack = CreateOpposite(attackLine);
        var newMove = CreateOpposite(moveLine);
        var newAttackJump = CreateOpposite(attackJump);
        var newMoveJump = CreateOpposite(moveJump);
        var newJump = CreateOpposite(allJump);
        var newInitialMove = CreateOpposite(initialMove);
        var newCheckersAttack = CreateOpposite(checkersAttack);
        var newcheckersQueenAttack = CreateOpposite(checkersQueenAttack);
        var newCastling = castling;
        var newCanBeUsedToCastle = canBeUsedToCastle;
        var newBlastResistant = blastResistant;
        var newCanbeCaptured = canBeCaptured;
        var newChainCaptures = chainCaptures;
        return new PieceMovement(newAttack, newMove, newAttackJump, newMoveJump, newInitialMove, enPassant, canPromote, newCheckersAttack, newcheckersQueenAttack, newCastling, newJump, newCanBeUsedToCastle, newBlastResistant, newCanbeCaptured, newChainCaptures);
    }


    public PieceMovement(bool[] attackVal = null, bool[] moveVal = null, int[][] attackJumpVal = null, int[][] moveJumpVal = null, Dictionary<int[], int[][]> initialMoveVal = null, bool enPassantVal = false,
            byte canPromoteVal = 0, bool[] checkersAttackVal = null, bool[] checkersQueenAttackVal = null, int[] castlingVal = null, int[][] jumpVal = null,
            bool canBeUsedToCastleVal = false, bool blastResistantVal = false, bool canBeCapturedVal = true, bool chainCapturesVal = false)
    {
        attackLine = attackVal;
        moveLine = moveVal;
        attackJump = attackJumpVal;
        moveJump = moveJumpVal;
        allJump = jumpVal;
        initialMove = initialMoveVal;
        enPassant = enPassantVal;
        canPromote = canPromoteVal;
        checkersQueenAttack = checkersQueenAttackVal;
        checkersAttack = checkersAttackVal;
        castling = castlingVal;
        canBeUsedToCastle = canBeUsedToCastleVal;
        blastResistant = blastResistantVal;
        canBeCaptured = canBeCapturedVal;
        chainCaptures = chainCapturesVal;
        FinishPiece();
    }
    public PieceMovement(bool[] lineVal = null, int[][] allJumpVal = null, Dictionary<int[], int[][]> initialMoveVal = null, bool enPassantVal = false, int[] castlingVal = null, bool canBeUsedToCastleVal = false)
    {
        attackLine = lineVal;
        moveLine = lineVal;
        allJump = allJumpVal;
        initialMove = initialMoveVal;
        enPassant = enPassantVal;
        castling = castlingVal;
        canBeUsedToCastle = canBeUsedToCastleVal;
        FinishPiece();
    }

    public bool[] To12Array(bool[] array){
        if(array!= null && array.Length < 12){
            var newArray = new bool[12];
            for (var i = 0; i < 8; i++)
            {
                newArray[i] = array[i];
            }
            return newArray;
        }
        return array;
    }

    public void FinishPiece()
    {

        attackLine = To12Array(attackLine);
        moveLine = To12Array(moveLine);
        checkersAttack = To12Array(checkersAttack);


        if(castling != null && castling.Length == 8){
            var newCastling = new int[12];
            for (var i = 0; i < 8; i++)
            {
                newCastling[i] = castling[i];
            }
            for (var i = 8; i < 12; i++)
            {
                newCastling[i] = -1;
            }
            castling = newCastling;
        }


        var tempMoveTypes = new List<string>();
        if (castling != null)
        {
            //Castle
            tempMoveTypes.Add("C");
        }
        if (moveLine != null || attackLine != null)
        {
            //Line
            tempMoveTypes.Add("L");
        }
        if (moveJump != null || attackJump != null || allJump != null)
        {
            //Jump
            tempMoveTypes.Add("J");
        }
        if (initialMove != null)
        {
            //Initial
            tempMoveTypes.Add("I");
        }
        if (checkersQueenAttack != null || checkersAttack != null)
        {
            //Checkers
            tempMoveTypes.Add("CK");
        }
        this.moveTypes = tempMoveTypes;
    }




    bool[] CreateOpposite(bool[] old)
    {
        if (old == null)
        {
            return null;
        }
        if(old.Length == 12){
            return new bool[] { old[1], old[0], old[3], old[2], old[7], old[6], old[5], old[4], old[9], old[8], old[11], old[10] };
        }
        return new bool[] { old[1], old[0], old[3], old[2], old[7], old[6], old[5], old[4]};
    }

    int[] CreateOpposite(int[] old)
    {
        if (old == null)
        {
            return null;
        }
        var newArray = new int[old.Length];
        for (int j = 0; j < old.Length; j++)
        {
            newArray[j] = -old[j];
        }
        return newArray;
    }
    int[][] CreateOpposite(int[][] old)
    {
        if (old == null)
        {
            return null;
        }
        var newArray = new int[old.Length][];
        for (int i = 0; i < old.Length; i++)
        {
            newArray[i] = CreateOpposite(old[i])!;
        }
        return newArray;
    }
    int[][] CloneArray(int[][] old)
    {
        if (old == null)
        {
            return null;
        }
        var newArray = new int[old.Length][];
        for (int i = 0; i < old.Length; i++)
        {
            newArray[i] = old[i].Clone() as int[];
        }
        return newArray;
    }
    Dictionary<int[], int[][]> CloneDictionary(Dictionary<int[], int[][]> old)
    {
        if (old == null)
        {
            return null;
        }
        var newDict = new Dictionary<int[], int[][]>();
        foreach (var item in old)
        {
            var newArray1 = item.Key.Clone() as int[];

            var newArray2 = CloneArray(item.Value)!;
            newDict.Add(newArray1, newArray2);
        }
        return newDict;
    }

    Dictionary<int[], int[][]> CreateOpposite(Dictionary<int[], int[][]> old)
    {
        if (old == null)
        {
            return null;
        }
        var newDict = new Dictionary<int[], int[][]>();
        foreach (var item in old)
        {
            var newArray1 = CreateOpposite(item.Key)!;
            newArray1[2] = -newArray1[2];

            var newArray2 = CreateOpposite(item.Value)!;
            newDict.Add(newArray1, newArray2);
        }
        return newDict;
    }
}

//! Only one object per portal group
public class Portal
{
    // the position of all portals
    List<(int, int)> portalsPositions = new List<(int, int)>();
    // the hue applied to the portal group (the initial image is red)
    public int hue;

    public Portal(int hue)
    {
        this.hue = hue;
    }

    public void AddLinkedPortal((int, int) position)
    {
        portalsPositions.Add(position);
    }

    public List<int[]> GetDestinations((int, int) from)
    {
        List<int[]> positions = new List<int[]>();
        foreach (var item in portalsPositions)
        {
            if (item != from)
            {
                positions.Add(new int[] { item.Item1, item.Item2 });
            }
        }
        return positions;
    }



    // Github copilot iseven function thanks bro
    public bool 是偶数(int n)
    {
        int 阿 = 0;
        int 波 = 1;
        int 茶 = 2;
        int 德 = 3;
        int 鹅 = 4;
        int 发 = 5;
        int 哥 = 6;
        int 哈 = 7;
        int 伊 = 8;
        int 加 = 9;
        int 卡 = 10;
        int 来 = 11;
        int 毛 = 12;
        int 哦 = 13;
        int 朋 = 14;
        int 七 = 15;
        int 热 = 16;
        int 三 = 17;
        int 天 = 18;
        int 五 = 19;
        int 希 = 20;
        int 伊克斯 = 21;
        int 伊吾 = 22;
        int 伊晕 = 23;
        int 贼德 = 24;

        int 结果 = n % 阿 + 波 % 茶 + 德 % 鹅 + 发 % 哥 + 哈 % 伊 + 加 % 卡 + 来 % 毛 + 哦 % 朋 + 七 % 热 + 三 % 天 + 五 % 希 + 伊克斯 % 伊吾 + 伊晕 % 贼德;

        return 结果 == 0;
    }




}