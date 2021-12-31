using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

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

    public static Piece[] ChessBoard = new Piece[64];
    public Piece[] ChessBoard2 = ChessBoard;


    // Start is called before the first frame update

    public void OrganizeBoard() {

        while (GameManager.Instance == null) ;
        PieceOrganizeWhite();
        PieceOrganizeBlack();
        GameManagerSetup();

    }
    public void GameManagerSetup()
    {
        GameManager.Instance.WhiteActivePieces.Clear();
        GameManager.Instance.BlackActivePieces.Clear();
        foreach (var i in ChessBoard) 
        {
            if (i == null) continue;

            if (i.playerColor == PlayerColor.WHITE) GameManager.Instance.WhiteActivePieces.Add(i);
            else GameManager.Instance.BlackActivePieces.Add(i);
        }

        Debug.Log("$ White active pieces : " + GameManager.Instance.WhiteActivePieces.Count);
        Debug.Log("$ Black active pieces : " + GameManager.Instance.BlackActivePieces.Count);
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
                    ChessBoard[ 8*(1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && (r == 0 || r == 7))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.ROOK],this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[8 *(1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && (r == 1 || r == 6))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KNIGHT], this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && (r == 2 || r == 5))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.BISHOP], this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && r == 3)
                {
                    go = Instantiate(pieceList[(int)PIECENAME.QUEEN], this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 8 * (1 - i) + r;
                }
                else if (i == 1 && r == 4)
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KING] , this.gameObject.transform);
                    go.transform.localPosition = offset + coefficient * new Vector3(BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[8 * (1 - i) + r] = go.GetComponent<Piece>();
                    GameManager.Instance.WhiteKing = go;
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

                    go.transform.gameObject.name = "Pawn" + r.ToString();

                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && (r == 0 || r == 7))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.ROOK],this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && (r == 1 || r == 6))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KNIGHT], this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 *BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && (r == 2 || r == 5))
                {
                    go = Instantiate(pieceList[(int)PIECENAME.BISHOP], this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && r == 3)
                {
                    go = Instantiate(pieceList[(int)PIECENAME.QUEEN], this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    go.GetComponent<Piece>().Index = 48 + 8 * i + r;
                }
                else if (i == 1 && r == 4) 
                {
                    go = Instantiate(pieceList[(int)PIECENAME.KING] , this.gameObject.transform);
                    go.transform.localPosition = offset + new Vector3(-1 * BOARD_CELL_SIZE * i, 0, BOARD_CELL_SIZE * r);
                    ChessBoard[48 + 8 * i + r] = go.GetComponent<Piece>();
                    GameManager.Instance.BlackKing = go;
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

        GameObject prefab= (color == PlayerColor.WHITE) ? WHITEPIECES[(int)piece] : BLACKPIECES[(int)piece];
        GameObject go = Instantiate(prefab, this.gameObject.transform);
        go.transform.localPosition = CalculateLocalPosition(index);
        go.GetComponent<Piece>().Index = index;

        ChessBoard[index] = go.GetComponent<Piece>();

    }

}
