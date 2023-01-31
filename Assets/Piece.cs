using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public abstract class Piece : MonoBehaviour
{
    public PlayerColor playerColor { get; set; }

    public PIECENAME pieceName { get; set; }

    private GameObject EleminationStage;
    public bool IsCastling { get; set; }

    protected int prevIndex;

    [SerializeField]
    protected List<MoveDirection> directions;
    public List<Moves> ValidMoves { get; set; }
    public bool isFirstMove { get; set; } = true;

    public int PieceValue { get; set; }
    public int AbsPieceValue { get; set; }
    public int Index { get; set; }

    public bool MoveCoroute { get; set; }

    public abstract IEnumerator Move(int index);
    public abstract void SelectChangeMaterial();

    public abstract List<Moves> CalculateValidMoves();

    public static List<Piece> opponentActivePieces;

    protected float speed;

    public float AttackingMovesScore { get; set; }
    public float DefendingMovesScore { get; set; }
    public float ThreatScore { get; set; }

    public float PieceThreatCoef { get; set; }

    private void Start()
    {
        Init();
    }

    public virtual void Init() 
    {

        EleminationStage = (playerColor == PlayerColor.WHITE) ? GameObject.Find("WhiteEliminateStage") : GameObject.Find("BlackEliminateStage");

        if (EleminationStage == null) Debug.Log("No eleimination stage is found");

        ValidMoves = new List<Moves>();
        opponentActivePieces = new List<Piece>();

        speed = Vector3.Magnitude(CalculateLocalPosition(0) - CalculateLocalPosition(2)) / 100;
        AttackingMovesScore = 0;
        DefendingMovesScore = 0;
        isFirstMove = true;

    }

    public Piece(PlayerColor playerColor, int index) {

        this.playerColor = playerColor;
        this.Index = index;
        ValidMoves = new List<Moves>();
        isFirstMove = true;
        opponentActivePieces = new List<Piece>();

    }
    public Vector3 GlobaCoordinates()
    {

        return Board.Origin + Index % 8 * new Vector3(0, 1.5f, 0) + ((int)(Index / 8)) * new Vector3(0, 0, 1.5f);

    }

    public PlayerColor GetPlayerColor()
    {

        return playerColor;

    }
    public List<MoveDirection> GetAllMoveDirections()
    {

        return directions;

    }
    protected bool CheckInSameRow(int index1, int index2)
    {

        if ((int)(index1 / 8) == (int)(index2 / 8))
        {
            return true;
        }

        return false;

    }
    protected bool CheckInSameColumn(int index1, int index2)
    {

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


    protected bool InsideTheBoard(int index)
    {

        bool valid = (index <= 63 && 0 <= index) ? true : false;
        return valid;

    }

    protected Vector3 CalculateLocalPosition(int index)
    {

        return new Vector3(5.25f, 0, -5.25f) + new Vector3((int)(index / 8) * -1.5f, 0, (int)(index % 8) * 1.5f);

    }

    protected Vector3 CalculateGlobalPosition(int index)
    {

        return transform.TransformPoint(CalculateLocalPosition(index));

    }

    public void Eliminate()
    {

        EleminationStage = (playerColor == PlayerColor.WHITE) ? GameObject.Find("WhiteEliminateStage") : GameObject.Find("BlackEliminateStage");

        Board.Instance.RemoveActivePiece(this);

        EleminationStage.GetComponent<EliminatePieceorganize>().PushEliminatePiece(this.transform.gameObject);
        Destroy(this);

        //Destroy(this.transform.gameObject);
    }

    protected void AddIndex(int index , MOVETYPE moveType)
    {

        if (CheckPinMove(index))
        {
            if (moveType == MOVETYPE.ATTACKING)
            {
                
                this.AttackingMovesScore = this.PieceValue + Board.Instance.ChessBoard[index].PieceValue;
                Board.Instance.ChessBoard[index].ThreatScore = this.PieceValue + Board.Instance.ChessBoard[index].PieceValue;

                ValidMoves.Add(new Moves(this.Index,  index , MOVETYPE.ATTACKING , PIECENAME.NOPIECE));

            }

            ValidMoves.Add(new Moves(this.Index, index, moveType , PIECENAME.NOPIECE));
        }

    }


    protected bool CheckPinMove(int index)
    {

        bool val = false;
        Piece originalPiece;
        if (Board.Instance.ChessBoard[index] != null)
        {

            originalPiece = Board.Instance.ChessBoard[index];
            Board.Instance.RemoveActivePiece(originalPiece);

        }
        else originalPiece = null;

        //Board.Instance.RemoveActivePiece(this);

        Board.Instance.ChessBoard[index] = this;
        Board.Instance.ChessBoard[this.Index] = null;
        int originalIndex = this.Index;
        Board.Instance.ChessBoard[index].Index = index;

        //Board.Instance.AddActivePiece(this);
        val = Board.Instance.IsCheck(this.playerColor);

        if (originalPiece != null)
        {

            Board.Instance.AddActivePiece(originalPiece);

        }

        //Board.Instance.RemoveActivePiece(this);

        Board.Instance.ChessBoard[index] = originalPiece;
        Board.Instance.ChessBoard[originalIndex] = this;
        this.Index = originalIndex;

        //Board.Instance.AddActivePiece(this);

        return !val;

    }

    public virtual void ChangePosition(int index)
    {

        MoveCoroute = true;

        Board.Instance.UpdateHashValue(this.Index, index);

        if (Board.Instance.ChessBoard[index] != null)
        {

            Board.Instance.ChessBoard[index].Eliminate();

        }

        Board.Instance.ChessBoard[index] = this;
        Board.Instance.ChessBoard[this.Index] = null;
        prevIndex = this.Index;

        Board.Instance.ChessBoard[index].Index = index;

        this.transform.gameObject.GetComponent<Piece>().Index = index;

        if (this.isFirstMove) isFirstMove = false;

        StartCoroutine(this.Move(index));

    }

    public string PrintValidMoves()
    {
        StringBuilder validString = new StringBuilder();
        foreach (Moves i in ValidMoves)
        {

            validString.Append(i.Destination.ToString());
            validString.Append(",");

        }

        if (validString.Length > 0) validString.Length--;

        return validString.ToString();

    }

    public string GetCurrentStatusPiece()
    {

        string status = "PiecName" + playerColor.ToString() + pieceName + " Index " + Index + "ValidMoves" + PrintValidMoves();

        return status;
    }

    public void ShowValidMoves() 
    {
        CalculateValidMoves();
        if (ValidMoves == null) return;

        foreach (var i in ValidMoves)
        {

            GameObject validBox = ObjectPool.instance.GetPooledObject();

            validBox.transform.localPosition = Board.Instance.CalculateLocalPosition(i.Destination);
            validBox.GetComponent<ValidBox>().ValidIndex = i.Destination;

        }
        
    }

}
