using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

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

    private void Start()
    {
        EleminationStage = (playerColor == PlayerColor.WHITE) ? GameObject.Find("WhiteEliminateStage") : GameObject.Find("BlackEliminateStage");

        if (EleminationStage == null) Debug.Log("No eleimination stage is found");

        ValidMoves = new List<int>();

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
        if (Board.ChessBoard[index] == null) return false;

        bool same = (Board.ChessBoard[index].playerColor == this.playerColor) ? true : false;

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

        if (playerColor == PlayerColor.WHITE)
        {
            GameManager.Instance.WhiteActivePieces.Remove(this);
        }
        else {

            GameManager.Instance.BlackActivePieces.Remove(this);

        }

        EleminationStage.GetComponent<EliminatePieceorganize>().PushEliminatePiece(this.transform.gameObject);
        Destroy(this);

        //Destroy(this.transform.gameObject);
    }


    public bool KingCheck(int index) {

        Piece originalPiece = (Piece)this.Clone();

        Piece goToIndex = (Board.ChessBoard[index] != null ) ? (Piece)Board.ChessBoard[index].Clone() : null ;

        Board.ChessBoard[originalPiece.Index] = null;
        Board.ChessBoard[index] = this;
        
        Board.ChessBoard[index].Index = index;

      
        int kingIndex = (playerColor == PlayerColor.WHITE) ? GameManager.Instance.WhiteKing.GetComponent<Piece>().Index : GameManager.Instance.BlackKing.GetComponent<Piece>().Index;

        if (pieceName == PIECENAME.KING) kingIndex = index;
        //Debug.Log("King Index is :" + kingIndex);

        List<Piece> opponentActivePieces = (playerColor == PlayerColor.WHITE) ? GameManager.Instance.BlackActivePieces : GameManager.Instance.WhiteActivePieces;

        if (goToIndex != null && goToIndex.playerColor != this.playerColor) opponentActivePieces.Remove(goToIndex);  

        foreach (Piece piece in opponentActivePieces)
        {
            if (piece.CalculateValidMoves().Find(x => x == kingIndex) == kingIndex) {

                Board.ChessBoard[originalPiece.Index] = originalPiece;
                Board.ChessBoard[index] = goToIndex ;
                this.Index = originalPiece.Index;

                //Debug.Log("try Index is " + index + " Still checked" + piece.pieceName );

                if (goToIndex != null) {
                    opponentActivePieces.Add(goToIndex);
                }
                 
                return false;
            }
            

        }

        Board.ChessBoard[originalPiece.Index] = originalPiece;
        Board.ChessBoard[index] = goToIndex;
        this.Index = originalPiece.Index;

        if (goToIndex != null)
        {
            opponentActivePieces.Add(goToIndex);
        }

        return true;

    }

    protected void AddIndex(int index) {


        if (GameManager.Instance.playerTurn == playerColor && KingCheck(index))
        {
            //Debug.Log(this.pieceName + " Valid Index is : " + index);
            ValidMoves.Add(index);

        }
        else if (GameManager.Instance.playerTurn != playerColor)
        {

            ValidMoves.Add(index);
        }
    
    
    }

    public object Clone()
    {
        return this.MemberwiseClone() as Piece;
    }

    public virtual void ChangePosition(int index) {

        MoveCoroute = true;

        if (Board.ChessBoard[index] != null){

            Board.ChessBoard[index].Eliminate();

        }

        Board.ChessBoard[index] = this;
        Board.ChessBoard[this.Index] = null;
        prevIndex = this.Index;

        Board.ChessBoard[index].Index = index;

        this.transform.gameObject.GetComponent<Piece>().Index = index;

        Debug.Log("@@" + this.pieceName + " " + this.Index );

        if (this.isFirstMove) isFirstMove = false;

        StartCoroutine(this.Move(index));

    }

}
