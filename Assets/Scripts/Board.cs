 using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class Board : MonoBehaviour
{

    public static Board Instance;
    private const float BOARD_CELL_SIZE = 1.5f;

    private Vector3 WHITE_START_POINT;
    private Vector3 BLACK_START_POINT;

    private Vector3 BOARD_WHITE_OFFSET = new Vector3(3.75f, 0, -5.25f);
    private Vector3 BOARD_BLACK_OFFSET = new Vector3(-3.75f, 0, -5.25f);

    [SerializeField]
    private List<GameObject> BLACKPIECES;
    [SerializeField]
    private List<GameObject> WHITEPIECES;
    [SerializeField]
    private GameObject validBox;
    public static Vector3 Origin { get; set; } = new Vector3(0,0,0);

    public Piece[] ChessBoard = new Piece[64];

    public GameObject WhiteKing { get; set; }
    public GameObject BlackKing { get; set; }
    public Piece WhiteKingPiece { get; set; }
    public Piece BlackKingPiece { get; set; }
    public List<Piece> WhiteActivePieces { get; set; }
    public List<Piece> BlackActivePieces { get; set; }

    ZobristHashTable ZHashTable { get; set; }
    public bool BlackChecked { get; set; }
    public bool WhiteChecked { get; set; }

    public PlayerColor playerTurn;

    float boardScore;

    // Start is called before the first frame update
    public void Awake()
    {
        if(Instance == null) Instance = this;
    }

    public void OrganizeBoard()
    {

        BlackChecked = false;
        WhiteChecked = false;

        WhiteActivePieces = new List<Piece>();
        BlackActivePieces = new List<Piece>();

        ZHashTable = new ZobristHashTable();

        this.PieceOrganizeWhite();
        this.PieceOrganizeBlack();
        //CustomWhitePieces();
        //CustomBlackPieces();

        this.ActivePiecesUpdate();

        ZHashTable.ComputeHash();


        Debug.Log("Hash Value is " + ZHashTable.HashValue);

        this.boardScore = 0;
        //this.PrintActivePieces();
    }

    void PieceOrganizeWhite() {

        GameObject go;

        Vector3 offset;
        int coefficient;

        List<GameObject> pieceList;
        pieceList = WHITEPIECES;
        offset = BOARD_WHITE_OFFSET;
        coefficient = 1;
        

        for (int i = 0; i < 2; i++) {
            for (int r = 0; r < 8; r++) {
                if (i == 0) InitPawn(PlayerColor.WHITE, 8 * (1 - i) + r);
                else if (i == 1 && (r == 0 || r == 7)) InitRook(PlayerColor.WHITE, 8 * (1 - i) + r);
                else if (i == 1 && (r == 1 || r == 6)) InitKnight(PlayerColor.WHITE, 8 * (1 - i) + r);
                else if (i == 1 && (r == 2 || r == 5)) InitBishop(PlayerColor.WHITE, 8 * (1 - i) + r);
                else if (i == 1 && r == 3) InitQueen(PlayerColor.WHITE, 8 * (1 - i) + r);
                else if (i == 1 && r == 4) InitKing(PlayerColor.WHITE, 8 * (1 - i) + r);  
            }
        }
    }
    
    void PieceOrganizeBlack() {

        GameObject go;
        Vector3 offset;
        List<GameObject> pieceList;
        pieceList = BLACKPIECES;
        offset = BOARD_BLACK_OFFSET;
        
        for (int i = 0; i < 2; i++) {
            for (int r = 0; r < 8; r++) {
                if (i == 0) InitPawn(PlayerColor.BLACK, 48 + 8 * i + r);
                else if (i == 1 && (r == 0 || r == 7)) InitRook(PlayerColor.BLACK, 48 + 8 * i + r);
                else if (i == 1 && (r == 1 || r == 6)) InitKnight(PlayerColor.BLACK , 48 + 8 * i + r);
                else if (i == 1 && (r == 2 || r == 5)) InitBishop(PlayerColor.BLACK, 48 + 8 * i + r);
                else if (i == 1 && r == 3) InitQueen(PlayerColor.BLACK, 48 + 8 * i + r);
                else if (i == 1 && r == 4) InitKing(PlayerColor.BLACK, 48 + 8 * i + r);
            }
        }
    }
    public void CreatValidBox(int Boardindex) {

        GameObject valid  = Instantiate(validBox);
        valid.transform.parent = this.transform.parent;
        valid.transform.localPosition = CalculateLocalPosition(Boardindex);
        valid.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
    
    }
    public Vector3 CalculateLocalPosition(int index) {

        return new Vector3(5.25f, 0, -5.25f) + new Vector3((int)(index/8) * -1.5f , 0.1f , (int)(index % 8) * 1.5f);
        
    }
    public Vector3 CalculateGlobalPosition(int index) {

        return transform.TransformPoint(CalculateLocalPosition(1));
    
    }
    public void OnePieceOrganize(PlayerColor color , PIECENAME piece , int index) 
    {

        if (piece == PIECENAME.PAWN) InitPawn(color , index);
        else if (piece == PIECENAME.ROOK) InitRook(color , index);
        else if (piece == PIECENAME.KNIGHT) InitKnight(color , index);
        else if (piece == PIECENAME.BISHOP) InitBishop(color , index);
        else if (piece == PIECENAME.QUEEN) InitQueen(color , index);
 
    }

    public void ActivePiecesUpdate()
    {
        this.WhiteActivePieces.Clear();
        this.BlackActivePieces.Clear();

        Debug.Log("Active Pieces Updates");
        foreach (var i in ChessBoard)
        {
            if (i == null) continue;
            if (i.playerColor == PlayerColor.WHITE) this.WhiteActivePieces.Add(i);
            else this.BlackActivePieces.Add(i);
        }

    }
    public bool IsCheckBlackPlayer() {

        bool checking = IsCheck(PlayerColor.BLACK);
        Board.Instance.BlackChecked = checking;
        return checking;

    }
    public bool IsCheckMateBlackPlayer()
    {

        foreach (Piece piece in Board.Instance.BlackActivePieces.ToArray())
        {
            if(piece.ValidMoves.Count != 0) return false;
        }

        if (!(IsCheck(PlayerColor.BLACK))) {

            return false;
        } 


        return true;

    }
    public bool IsCheckWhitePlayer()
    {

        bool checking = IsCheck(PlayerColor.WHITE);

        Board.Instance.WhiteChecked = checking;
        return checking;    

    }

    public bool IsCheckMateWhitePlayer()
    {

        foreach (Piece piece in Board.Instance.WhiteActivePieces.ToArray())
        {

            if(piece.ValidMoves.Count !=0 ) return false;

        }

        if (!IsCheck(PlayerColor.WHITE)) {
            
            return false;

        }

        return true;

    }
    bool IsAdjacentColumn(int index1, int index2)
    {

        if (Mathf.Abs((int)(index2 % 8) - ((int)(index1 % 8))) == 1) return true;

        return false;

    }

    bool IsAdjacentRow(int index1, int index2)
    {

        if (Mathf.Abs((int)(index1 / 8) - ((int)(index2 / 8))) == 1) return true;

        return false;

    }


    public StringBuilder PrintBoard() {

        StringBuilder board = new StringBuilder();
        for (int i = 0; i <=63; i ++ ) {

            if (ChessBoard[i] == null) board.Append("--");
            else board.Append("" + ChessBoard[i].playerColor.ToString()[0]+ ChessBoard[i].pieceName.ToString()[0]);

            if ((i + 1) % 8 == 0) board.Append("\n");
            else board.Append(",");
        
        
        }

        return board;
    
    }

    public StringBuilder PrintBoardNow()
    {

        StringBuilder board = new StringBuilder();
        for (int i = 0; i <= 63; i++)
        {

            if (ChessBoard[i] == null) board.Append("--");
            else board.Append("" + ChessBoard[i].playerColor.ToString()[0] + ChessBoard[i].pieceName.ToString()[0]);

            if ((i + 1) % 8 == 0) board.Append("\n");
            else board.Append(",");


        }

        return board;

    }

    public void PrintActivePieces() {

        //StringBuilder board = new StringBuilder();

        foreach (var i in WhiteActivePieces) {
            
            i.CalculateValidMoves();
            Debug.Log(i.GetCurrentStatusPiece());
        
        }

        foreach (var i in BlackActivePieces)
        {
            i.CalculateValidMoves();
            Debug.Log(i.GetCurrentStatusPiece());

        }

    }


    public void RemoveActivePiece(Piece p) {

        if (p.playerColor == PlayerColor.WHITE)
        {

            WhiteActivePieces.Remove(p);


        }
        else {

            BlackActivePieces.Remove(p);

        }
    
    }

    public void AddActivePiece(Piece p)
    {
        if (p.playerColor == PlayerColor.WHITE)
        {
            WhiteActivePieces.Add(p);

        }
        else
        {
            BlackActivePieces.Add(p);
        }


    }

    public void UpdateValidMoves() {

        foreach (Piece p in WhiteActivePieces)
        {

            p.AttackingMovesScore = 0;
            p.ThreatScore = 0;
            p.DefendingMovesScore = 0;
        }

        foreach (Piece p in BlackActivePieces)
        {

            p.AttackingMovesScore = 0;
            p.ThreatScore = 0;
            p.DefendingMovesScore = 0;
        }

        /// update valid moves in white 
        foreach (Piece p in WhiteActivePieces.ToArray()) {

            int preIndex = p.Index;
            p.CalculateValidMoves();

            if (p.Index != preIndex)
            {
                Debug.LogError("Index are not valid");
            }
        }
        
        foreach (Piece p in BlackActivePieces.ToArray()) {

            p.CalculateValidMoves();

        }

    }

    public bool IsCheck( PlayerColor playerColor) {

        int _KingIndex =(playerColor == PlayerColor.WHITE) ? WhiteKingPiece.Index : BlackKingPiece.Index;
        int _KingIndexOpponent =(playerColor == PlayerColor.WHITE) ? BlackKingPiece.Index : WhiteKingPiece.Index;
        PlayerColor opponentColor = (playerColor == PlayerColor.WHITE) ? PlayerColor.BLACK : PlayerColor.WHITE;

        int distance;

        int index = _KingIndex;

        List<Piece> opponentActivePieces = (playerColor == PlayerColor.WHITE) ? BlackActivePieces : WhiteActivePieces;

        Piece Queen = opponentActivePieces.Find(x => x.pieceName == PIECENAME.QUEEN);

        if (Queen != null) {

            if (CheckedByQueen(Queen, _KingIndex)) return true;

        }

        List<Piece> rookList = opponentActivePieces.FindAll(x => x.pieceName == PIECENAME.ROOK);

        List<Piece> bishopList = opponentActivePieces.FindAll(x => x.pieceName == PIECENAME.BISHOP);

        List<Piece> knightList = opponentActivePieces.FindAll(x => x.pieceName == PIECENAME.KNIGHT);

        if (CheckedByRook(rookList, _KingIndex)) return true;

        if (CheckedByBishop(bishopList, _KingIndex)) return true;

        

        /// pawns position 
        /// 
        int pawnCheckIndex1 = (playerColor == PlayerColor.WHITE) ? _KingIndex + 9 : _KingIndex - 9;
        int pawnCheckIndex2 = (playerColor == PlayerColor.WHITE) ? _KingIndex + 7 : _KingIndex - 7;
        
        if (InsideTheBoard(pawnCheckIndex1) &&
            Mathf.Abs((int)(_KingIndex) / 8 - (int)(pawnCheckIndex1) / 8) == 1 && 
            ChessBoard[pawnCheckIndex1] != null &&
            ChessBoard[pawnCheckIndex1].playerColor == opponentColor &&
            ChessBoard[pawnCheckIndex1].pieceName == PIECENAME.PAWN)
        {

            return true;

        }

        if (InsideTheBoard(pawnCheckIndex2) &&
            Mathf.Abs((int)(_KingIndex)/ 8 - (int)(pawnCheckIndex2)/8) == 1 &&
            ChessBoard[pawnCheckIndex2] != null &&
            ChessBoard[pawnCheckIndex2].playerColor == opponentColor &&
            ChessBoard[pawnCheckIndex2].pieceName == PIECENAME.PAWN)
        {

            return true;
        }
        /// knight positions


        foreach (var knight in knightList) {
            distance = Mathf.Abs(knight.Index -_KingIndex);
            if (distance == 17 || distance == 15 || distance == 10 || distance == 6) {

                return true;
            }
        
        }

        /// checked by opponent king 
        int dis = Mathf.Abs(_KingIndex - _KingIndexOpponent);
        if (dis == 7 || dis == 9 || dis == 8 || dis == 1) {

            return true;
        
        }
        return false;

    }

    bool InsideTheBoard(int index)
    {

        bool valid = (index <= 63 && 0 <= index) ? true : false;
        return valid;

    }

    bool CheckInSameColumn(int index1, int index2)
    {

        if ((index1 % 8) == (index2 % 8))
        {
            return true;
        }

        return false;
    }
    bool CheckAdjacentDiagonal(int index1, int index2)
    {
        if (Mathf.Abs((index1 % 8) - (index2 % 8)) == Mathf.Abs((int)(index1 / 8) - (int)(index2 / 8)))
        {
            return true;
        }

        return false;
    }


    bool CheckInSameRow(int index1, int index2)
    {

        if ((int)(index1 / 8) == (int)(index2 / 8))
        {
            return true;
        }

        
        return false;

    }

    bool CheckUpperDiagonal(int index1 , int index2)
    {
        int x1 = (index1 >= index2) ? index1 : index2;
        int x2 = (index1 >= index2) ? index2 : index1;
        if (((int)(x1 % 8) - (int)(x2 % 8)) == ((int)(x1 / 8) - (int)(x2 / 8))) return true;

        return false;
    
    }

    bool CheckLowerDiagonal(int index1 , int index2)
    {

        int x1 = (index1 >= index2) ? index1 : index2;
        int x2 = (index1 >= index2) ? index2 : index1;
        if (((int)(x1 % 8) - (int)(x2 % 8)) == -1*((int)(x1 / 8) - (int)(x2 / 8))) return true;

        return false;

    }

    public void UpdateHashValue(int from , int to) {

        ZHashTable.UpdateHashValue(from, to);
    
    }

    public void AddPieceUpdataHashValue(int index , Piece p) {

        ZHashTable.AddPiece(index , p);

    }

    public ulong GetHashValue() {

        return ZHashTable.HashValue;
    
    }

    bool CheckedByRook(List<Piece> rookList , int kingIndex) {

        foreach (var rook in rookList)
        {

            if (CheckInSameRow(rook.Index, kingIndex))
            {
                int step = (rook.Index - kingIndex);
                int factor = (int)(step / Mathf.Abs(step));
                step = Mathf.Abs(step);

                if (step == 1)
                {

                    return true;

                }
                else
                {
                    for (int i = 1; i < step ; i++)
                    {
                        if (ChessBoard[kingIndex + i * factor] != null) return false;

                    }

                    return true;
                }

            }
            else if (CheckInSameColumn(rook.Index, kingIndex))
            {

                int step = (int)(rook.Index - kingIndex) / 8;
                int factor = (int)(step / Mathf.Abs(step));
                step = Mathf.Abs(step);
                if (step == 1)
                {

                    return true;

                }
                else
                {
                    for (int i = 1; i < step; i++)
                    {
                        if (ChessBoard[kingIndex + 8 * i * factor] != null) return false;
                    }

                    return true;
                }
            }

        }

        return false;
    }

    bool CheckedByBishop(List<Piece> bishopList , int kingIndex) {

        foreach (var bishop in bishopList) {

            if (CheckLowerDiagonal(bishop.Index, kingIndex)) {

                int step = (bishop.Index - kingIndex) / 7;
                int factor = (int)(step / Mathf.Abs(step));
                step = Mathf.Abs(step);

                if (step == 1 || step == -1) return true;

                else {

                    for (int i = 1; i < step; i++) {

                        if (ChessBoard[kingIndex + factor * 7 * i] != null) return false;
                    
                    }

                    return true;
                
                }


            }
            else if (CheckUpperDiagonal(bishop.Index, kingIndex)) {

                int step = (bishop.Index - kingIndex) / 9;
                int factor = (int)(step / Mathf.Abs(step));
                step = Mathf.Abs(step);

                if (step == 1 || step == -1) return true;

                else {

                    for (int i = 1; i < step; i++) {

                        if (ChessBoard[kingIndex + factor * 9 * i] != null) return false;
                    
                    }

                    return true;
                
                }
            }
        }

        return false;
    
    }


    bool CheckedByQueen(Piece queen , int kingIndex) {

        if (CheckInSameRow(queen.Index, kingIndex))
        {

            int step = (queen.Index - kingIndex);
            int factor = (int)(step / Mathf.Abs(step));
            step = Mathf.Abs(step);

            if (step == 1)
            {

                return true;

            }
            else
            {
                for (int i = 1; i < step; i++)
                {
                    if (ChessBoard[kingIndex + i * factor] != null) return false;

                }

                return true;
            }

        }
        else if (CheckInSameColumn(queen.Index, kingIndex))
        {

            int step = (int)(queen.Index - kingIndex) / 8;
            int factor = (int)(step / Mathf.Abs(step));
            step = Mathf.Abs(step);
            if (step == 1)
            {

                return true;

            }
            else
            {
                for (int i = 1; i < step; i++)
                {
                    if (ChessBoard[kingIndex + 8 * i * factor] != null) return false;
                }

                return true;
            }
        }
        else if (CheckLowerDiagonal(queen.Index, kingIndex))
        {

            int step = (queen.Index - kingIndex) / 7;
            int factor = (int)(step / Mathf.Abs(step));
            step = Mathf.Abs(step);

            if (step == 1 || step == -1) return true;

            else
            {

                for (int i = 1; i < step; i++)
                {

                    if (ChessBoard[kingIndex + factor * 7 * i] != null) return false;

                }

                return true;

            }


        }
        else if (CheckUpperDiagonal(queen.Index, kingIndex))
        {

            int step = (queen.Index - kingIndex) / 9;
            int factor = (int)(step / Mathf.Abs(step));
            step = Mathf.Abs(step);

            if (step == 1 || step == -1) return true;

            else
            {

                for (int i = 1; i < step; i++)
                {

                    if (ChessBoard[kingIndex + factor * 9 * i] != null) return false;

                }

                return true;

            }


        }

        return false;

    }

    public void InitPawn(PlayerColor playerColor , int index)
    {
        List<GameObject> pieceList = (playerColor == PlayerColor.WHITE) ? WHITEPIECES : BLACKPIECES;

        GameObject pawn = Instantiate(pieceList[(int)PIECENAME.PAWN], this.gameObject.transform);
        PawnModel pm =  pawn.AddComponent<PawnModel>();

        Piece p = new Pawn(playerColor, index, pm);

        pawn.GetComponent<PawnModel>().Piece = p;
        pawn.GetComponent<PawnModel>().SetLocalPosition(index);
        ChessBoard[index] = p;
    }

    public void InitRook(PlayerColor playerColor, int index)
    {

        List<GameObject> pieceList = (playerColor == PlayerColor.WHITE) ? WHITEPIECES : BLACKPIECES;

        GameObject rook = Instantiate(pieceList[(int)PIECENAME.ROOK], this.gameObject.transform);
        RookModel rm = rook.AddComponent<RookModel>();

        Piece p = new Rook(playerColor, index, rm);

        rook.GetComponent<RookModel>().Piece = p;
        rook.GetComponent<RookModel>().SetLocalPosition(index);
        ChessBoard[index] = p;

    }
    public void InitBishop(PlayerColor playerColor, int index)
    {

        List<GameObject> pieceList = (playerColor == PlayerColor.WHITE) ? WHITEPIECES : BLACKPIECES;

        GameObject bishop = Instantiate(pieceList[(int)PIECENAME.BISHOP], this.gameObject.transform);
        BishopModel bm  =bishop.AddComponent<BishopModel>();

        Piece p = new Bishop(playerColor, index, bm);

        bishop.GetComponent<BishopModel>().Piece = p;
        bishop.GetComponent<BishopModel>().SetLocalPosition(index);
        ChessBoard[index] = p;

    }
    public void InitKnight(PlayerColor playerColor, int index)
    {

        List<GameObject> pieceList = (playerColor == PlayerColor.WHITE) ? WHITEPIECES : BLACKPIECES;

        GameObject knight = Instantiate(pieceList[(int)PIECENAME.KNIGHT], this.gameObject.transform);
        KnightModel km = knight.AddComponent<KnightModel>();

        Piece p = new Knight(playerColor, index, km);

        knight.GetComponent<KnightModel>().Piece = p;
        knight.GetComponent<KnightModel>().SetLocalPosition(index);
        ChessBoard[index] = p;

    }
    public void InitKing(PlayerColor playerColor, int index)
    {

        List<GameObject> pieceList = (playerColor == PlayerColor.WHITE) ? WHITEPIECES : BLACKPIECES;

        GameObject knight = Instantiate(pieceList[(int)PIECENAME.KING], this.gameObject.transform);
        KingModel km = knight.AddComponent<KingModel>();

        Piece p = new King(playerColor, index , km);

        knight.GetComponent<KingModel>().Piece = p;
        knight.GetComponent<KingModel>().SetLocalPosition(index);
        ChessBoard[index] = p;

        if (playerColor == PlayerColor.WHITE) WhiteKingPiece = p;
        else BlackKingPiece = p;

    }

    public void InitQueen(PlayerColor playerColor, int index)
    {
        List<GameObject> pieceList = (playerColor == PlayerColor.WHITE) ? WHITEPIECES : BLACKPIECES;

        GameObject queen = Instantiate(pieceList[(int)PIECENAME.QUEEN], this.gameObject.transform);
        QueenModel qm = queen.AddComponent<QueenModel>();

        Piece p = new Queen(playerColor, index , qm);

        queen.GetComponent<QueenModel>().Piece = p;
        queen.GetComponent<QueenModel>().SetLocalPosition(index);
        ChessBoard[index] = p;
    }



    #region DebugBoard

    public void CustomWhitePieces() 
    {
        GameObject go;
        InitRook(PlayerColor.WHITE ,8);
        InitKing(PlayerColor.WHITE ,2);
    }

    public void CustomBlackPieces()
    {

        InitPawn(PlayerColor.BLACK, 13);
        InitKing(PlayerColor.BLACK, 62);

    }
    #endregion


}


