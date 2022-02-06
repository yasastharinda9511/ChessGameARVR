 using System;
using System.Collections;
using System.Collections.Generic;
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

    public int WhiteKingIndex { get; set; }

    public int BlackKingIndex { get; set; }

    public List<Piece> WhiteActivePieces { get; set; }
    public List<Piece> BlackActivePieces { get; set; }


    public bool BlackChecked { get; set; }
    public bool WhiteChecked { get; set; }

    public PlayerColor playerTurn;

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

        this.PieceOrganizeWhite();
        this.PieceOrganizeBlack();
        this.ActivePiecesUpdate();
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
                if (i == 0)
                {
                    go = Instantiate(pieceList[(int)PIECENAME.PAWN],this.gameObject.transform);
                    go.transform.localPosition = offset +  new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    go.transform.name = "WhitePawn" + r.ToString();
                    ChessBoard[ 8*(1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && (r == 0 || r == 7))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.ROOK],this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    go.transform.name = "WhiteROOK" + r.ToString();
                    ChessBoard[8 *(1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && (r == 1 || r == 6))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KNIGHT], this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    go.transform.name = "WhiteKNIGHT" + r.ToString();
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && (r == 2 || r == 5))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.BISHOP], this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    go.transform.name = "WhiteBISHOP" + r.ToString();
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && r == 3)
                {
                    go = Instantiate(pieceList[(int)PIECENAME.QUEEN], this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    go.transform.name = "WhiteQUEEN" + r.ToString();
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && r == 4)
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KING] , this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    go.transform.name = "WhiteKING" + r.ToString();
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    WhiteKingIndex = 4;
                    this.WhiteKing = go;

                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }   
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
                if (i == 0)
                {
                    go = Instantiate(pieceList[(int) PIECENAME.PAWN],this.gameObject.transform);
                    go.transform.localPosition = offset +  new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[ 48 + 8 * i + r] = go.GetComponent<Piece>();

                    go.transform.gameObject.name = "BlackPawn" + r.ToString();

                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && (r == 0 || r == 7))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.ROOK],this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    go.transform.name = "BlackROOK" + r.ToString();
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && (r == 1 || r == 6))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KNIGHT], this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 *BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.transform.name = "BlackKNIGHT" + r.ToString();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && (r == 2 || r == 5))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.BISHOP], this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.transform.name = "BlackBISHOP" + r.ToString();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && r == 3)
                {
                    go = Instantiate(pieceList[(int)PIECENAME.QUEEN], this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.transform.name = "BlackQUEEN" + r.ToString();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && r == 4) 
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KING] , this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.transform.name = "BlackKING" + r.ToString();
                    this.BlackKing = go;

                    BlackKingIndex = 48 + 8 * i + r;

                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;

                }
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
    public void OnePieceOrganize(PlayerColor color , PIECENAME piece , int index) {

        if (ChessBoard[index] != null) ChessBoard[index].Eliminate();

        Debug.Log("pawn to queen is : " + (int) piece);

        GameObject prefab= (color == PlayerColor.WHITE) ? WHITEPIECES[(int)piece] : BLACKPIECES[(int)piece];
        GameObject go = Instantiate(prefab, this.gameObject.transform);
        go.transform.localPosition = CalculateLocalPosition(index);
        go.GetComponent<Piece>().Index = index;

        ChessBoard[index] = go.GetComponent<Piece>();

    }

    public void ActivePiecesUpdate()
    {
        this.WhiteActivePieces.Clear();
        this.BlackActivePieces.Clear();
        foreach (var i in ChessBoard)
        {
            if (i == null) continue;

            if (i.playerColor == PlayerColor.WHITE) this.WhiteActivePieces.Add(i);
            else this.BlackActivePieces.Add(i);
        }

    }

    public bool IsCheckBlackPlayer() {

        Board.Instance.playerTurn = PlayerColor.BLACK;

        ActivePiecesUpdate();
        int kingIndex = BlackKingIndex; // Board.Instance.BlackKing.GetComponent<Piece>().Index
        foreach (Piece piece in Board.Instance.WhiteActivePieces.ToArray())
        {
            if (piece.CalculateValidMoves().Find(x => x == kingIndex) == kingIndex)
            {
                Board.Instance.BlackChecked = true;
                return true;

            }

        }
        Board.Instance.BlackChecked = false;
        return false;

    }
    public bool IsCheckMateBlackPlayer()
    {

        int totalMoves = 0;

        Board.Instance.playerTurn = PlayerColor.BLACK;

        ActivePiecesUpdate();

        foreach (Piece piece in Board.Instance.BlackActivePieces.ToArray())
        {
            totalMoves += piece.CalculateValidMoves().Count;
        }

        if (IsCheckBlackPlayer() &&  totalMoves == 0)
        {

            return true;

        }
        

        return false;

    }
    public bool IsCheckWhitePlayer()
    {

        Board.Instance.playerTurn = PlayerColor.WHITE;

        ActivePiecesUpdate();
        int kingIndex = WhiteKingIndex ;//Board.Instance.WhiteKing.GetComponent<Piece>().Index;
        foreach (Piece piece in Board.Instance.BlackActivePieces.ToArray())
        {

            if (piece.CalculateValidMoves().Find(x => x == kingIndex) == kingIndex)
            {
                Board.Instance.WhiteChecked = true;
                return true;
            }

        }
        Board.Instance.WhiteChecked = false;
        return false;    

    }

    public bool IsCheckMateWhitePlayer()
    {
        int totalMoves = 0;

        Board.Instance.playerTurn = PlayerColor.WHITE;

        this.ActivePiecesUpdate();
        foreach (Piece piece in Board.Instance.WhiteActivePieces.ToArray())
        {

            totalMoves += piece.CalculateValidMoves().Count;

        }

        if (totalMoves == 0 && IsCheckWhitePlayer())
        {

            return true;

        }

        return false;

    }


}
