using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Piece : MonoBehaviour , ICloneable
{
    public PlayerColor playerColor;

    public PIECENAME pieceName;

    private GameObject EleminationStage;
    public bool IsCastling { get; set; }

    protected int prevIndex;

    [SerializeField]
    protected List<MoveDirection> directions;

    public List<int> ValidMoves;
    public bool isFirstMove { get; set; } = true;

    public int Index;

    public bool MoveCoroute { get; set; }

    public abstract IEnumerator Move(int index);
    public abstract void SelectChangeMaterial();

    public abstract List<int> CalculateValidMoves();

    public static List<Piece> opponentActivePieces;

    protected float speed;

    private void Start()
    {
        EleminationStage = (playerColor == PlayerColor.WHITE) ? GameObject.Find("WhiteEliminateStage") : GameObject.Find("BlackEliminateStage");

        if (EleminationStage == null) Debug.Log("No eleimination stage is found");

        ValidMoves = new List<int>();
        opponentActivePieces = new List<Piece>();

        speed = .5f;

    }
    public Vector3 GlobaCoordinates() {

        return Board.Origin + Index % 8 * new Vector3(0, 1.5f, 0) + ((int)(Index / 8)) * new Vector3 (0, 0, 1.5f);
    
    }

    public PlayerColor GetPlayerColor() {

        return playerColor;
    
    }
    public List<MoveDirection> GetAllMoveDirections() {

        return directions;
    
    }
    protected bool CheckInSameRow(int index1, int index2)
    {

        if ((int)(index1 / 8) == (int)(index2 / 8)) {
            return true;
        }

        return false;

    }
    protected bool CheckInSameColumn(int index1 , int index2) {

        if ((index1 % 8) == (index2 % 8))
        {
            return true;
        }

        return false;
    }
    protected bool CheckAdjacentDiagonal(int index1, int index2)
    {
        if (Mathf.Abs((index1 % 8) - (index2 % 8)) == Mathf.Abs((int)(index1 / 8) - (int)(index2 / 8))) 
        {
            return true;        
        }

        return false;
    }

    protected bool IsAdjacentColumn(int index1, int index2)
    {

        if (Mathf.Abs((int)(index2 % 8) - ((int)(index1 % 8))) == 1) return true;

        return false;

    }

    protected bool IsAdjacentRow(int index1, int index2)
    {

        if (Mathf.Abs((int)(index1 / 8) - ((int)(index2 / 8))) == 1) return true;

        return false;

    }


    protected bool CheckSameColorPiece(int index)
    {
        if (Board.Instance.ChessBoard[index] == null) return false;

        bool same = (Board.Instance.ChessBoard[index].playerColor == this.playerColor) ? true : false;

        return same;
    }


    protected bool InsideTheBoard(int index) {

        bool valid = (index <= 63 && 0 <= index) ? true : false;
        return valid;
    
    }

    protected Vector3 CalculateLocalPosition(int index)
    {

        return new Vector3(5.25f, 0, -5.25f) + new Vector3((int)(index / 8) * -1.5f, 0 , (int)(index % 8) * 1.5f);

    }

    protected Vector3 CalculateGlobalPosition(int index)
    {

        return transform.TransformPoint(CalculateLocalPosition(index));

    }

    public void Eliminate() {

        EleminationStage = (playerColor == PlayerColor.WHITE) ? GameObject.Find("WhiteEliminateStage") : GameObject.Find("BlackEliminateStage");

        Board.Instance.ActivePiecesUpdate();

        EleminationStage.GetComponent<EliminatePieceorganize>().PushEliminatePiece(this.transform.gameObject);
        Destroy(this);

        //Destroy(this.transform.gameObject);
    }


    public bool KingCheck(int index) {

        Piece originalPiece = (Piece)Board.Instance.ChessBoard[Index].Clone();

        Piece goToIndex = (Board.Instance.ChessBoard[index] != null ) ? (Piece)Board.Instance.ChessBoard[index].Clone() : null ;

        Board.Instance.ChessBoard[originalPiece.Index] = null;
        Board.Instance.ChessBoard[index] = this;
        
        Board.Instance.ChessBoard[index].Index = index;

        int kingIndex = (playerColor == PlayerColor.WHITE) ? Board.Instance.WhiteKing.GetComponent<Piece>().Index : Board.Instance.BlackKing.GetComponent<Piece>().Index;

        if (pieceName == PIECENAME.KING) kingIndex = index; 

        opponentActivePieces = (playerColor == PlayerColor.WHITE) ? Board.Instance.BlackActivePieces :  Board.Instance.WhiteActivePieces;   
        
        

        if (goToIndex != null && goToIndex.playerColor != this.playerColor) {

            opponentActivePieces.Remove(opponentActivePieces.Find(x => x.pieceName == goToIndex.pieceName && x.playerColor == goToIndex.playerColor && x.Index == index));

        }  

        foreach (Piece piece in opponentActivePieces)
        {

            if (piece.CalculateValidMoves().Find(x => x == kingIndex) == kingIndex) {

                Board.Instance.ChessBoard[originalPiece.Index] = originalPiece;
                Board.Instance.ChessBoard[index] = goToIndex ;
                this.Index = originalPiece.Index;

                //UIManager.Instance.DebugBannerUpdate("try Index is " + index + " Still checked checked by " + piece.pieceName);
                 
                if (goToIndex != null) {
                    opponentActivePieces.Add(goToIndex);
                }
                return false;
            }
            

        }

        Board.Instance.ChessBoard[originalPiece.Index] = originalPiece;
        Board.Instance.ChessBoard[index] = goToIndex;
        this.Index = originalPiece.Index;

        if (goToIndex != null)
        {
            opponentActivePieces.Add(goToIndex);
        }
        return true;

    }

    protected void AddIndex(int index) {


        if (Board.Instance.playerTurn == playerColor && CheckPinPinMove(index))
        {
            ValidMoves.Add(index); 

        }
        else if (Board.Instance.playerTurn != playerColor)
        {
            ValidMoves.Add(index);
        }
    
    
    }


    protected bool CheckPinPinMove(int index) {

        bool val = false;
        Piece originalPiece;
        if (Board.Instance.ChessBoard[index] != null) originalPiece = (Piece)Board.Instance.ChessBoard[index].Clone();
        else originalPiece = null;

        Board.Instance.ChessBoard[index] = (Piece) this.Clone();
        Board.Instance.ChessBoard[this.Index] = null;
        int originalIndex = this.Index;
        Board.Instance.ChessBoard[index].Index = index;

        if (pieceName == PIECENAME.KING && playerColor == PlayerColor.WHITE) Board.Instance.WhiteKingIndex = index;
        else if (pieceName == PIECENAME.KING && playerColor == PlayerColor.BLACK) Board.Instance.BlackKingIndex = index;

        if (playerColor == PlayerColor.WHITE)
        {

            val = Board.Instance.IsCheckWhitePlayer();
        }
        else {

            val = Board.Instance.IsCheckBlackPlayer();
        }

        Board.Instance.ChessBoard[index] = originalPiece;
        Board.Instance.ChessBoard[originalIndex] = this;

        if (pieceName == PIECENAME.KING && playerColor == PlayerColor.WHITE) Board.Instance.WhiteKingIndex = originalIndex;
        else if (pieceName == PIECENAME.KING && playerColor == PlayerColor.BLACK) Board.Instance.BlackKingIndex = originalIndex;

        return !val;

    }

    public object Clone()
    {
        return this.MemberwiseClone() as Piece;
    }

    public virtual void ChangePosition(int index) {

        MoveCoroute = true;
        if (Board.Instance.ChessBoard[index] != null){

            Board.Instance.ChessBoard[index].Eliminate();

        }

        Board.Instance.ChessBoard[index] = this;
        Board.Instance.ChessBoard[this.Index] = null;
        prevIndex = this.Index;

        Board.Instance.ChessBoard[index].Index = index;

        if (pieceName == PIECENAME.KING && playerColor == PlayerColor.WHITE) Board.Instance.WhiteKingIndex = index ;
        else if (pieceName == PIECENAME.KING && playerColor == PlayerColor.BLACK) Board.Instance.BlackKingIndex = index ;

        this.transform.gameObject.GetComponent<Piece>().Index = index;

        if (this.isFirstMove) isFirstMove = false;

        StartCoroutine(this.Move(index));

    }

}
