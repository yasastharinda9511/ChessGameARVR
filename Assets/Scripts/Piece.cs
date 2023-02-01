using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading.Tasks;

public abstract class Piece
{
    public PlayerColor playerColor { get; set; }
    public PIECENAME pieceName { get; set; }
    public PieceModel PieceModel { get; set; }
    public bool IsCastling { get; set; }
    public int PrevIndex { get; set; }
    public List<Moves> ValidMoves { get; set; }
    public bool isFirstMove { get; set; } = true;

    public int PieceValue { get; set; }
    public int AbsPieceValue { get; set; }
    public int Index { get; set; }

    public bool MoveCoroute { get; set; }

    public abstract List<Moves> CalculateValidMoves();

    protected float speed;

    public float AttackingMovesScore { get; set; }
    public float DefendingMovesScore { get; set; }
    public float ThreatScore { get; set; }

    public float PieceThreatCoef { get; set; }

    public Piece(PlayerColor playerColor, int index) {

        this.playerColor = playerColor;
        this.Index = index;
        ValidMoves = new List<Moves>();
        isFirstMove = true;
        PieceModel = null;

    }

    public Piece(PlayerColor playerColor, int index , PieceModel pieceModel) 
    {
        this.playerColor = playerColor;
        this.Index = index;
        ValidMoves = new List<Moves>();
        isFirstMove = true;
        PieceModel = pieceModel;

    }
    public Vector3 GlobaCoordinates()
    {

        return Board.Origin + Index % 8 * new Vector3(0, 1.5f, 0) + ((int)(Index / 8)) * new Vector3(0, 0, 1.5f);

    }

    public PlayerColor GetPlayerColor()
    {

        return playerColor;

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

    protected int ColomnDifference(int index1 , int index2) => Mathf.Abs((int)(index1 % 8) - ((int)(index2 % 8)));
    protected int RowDifference(int index1 , int index2) => Mathf.Abs((int)(index1 / 8) - ((int)(index2 / 8)));

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

    public void Eliminate()
    {
        Board.Instance.ChessBoard[Index] = null;
        Board.Instance.RemoveActivePiece(this);

        PieceModel.Eleminate();
    }


    public void AddMove(Moves move) 
    {
        move.DoMove();
        if(!Board.Instance.IsCheck(this.playerColor)) ValidMoves.Add(move);
        move.UndoMove();
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

        Board.Instance.ChessBoard[index] = this;
        Board.Instance.ChessBoard[this.Index] = null;
        int originalIndex = this.Index;
        Board.Instance.ChessBoard[index].Index = index;
        val = Board.Instance.IsCheck(this.playerColor);

        if (originalPiece != null)
        {

            Board.Instance.AddActivePiece(originalPiece);

        }

        Board.Instance.ChessBoard[index] = originalPiece;
        Board.Instance.ChessBoard[originalIndex] = this;
        this.Index = originalIndex;

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
        PrevIndex = this.Index;

        Board.Instance.ChessBoard[index].Index = index;

        Index = index;

        if (this.isFirstMove) isFirstMove = false;

        PieceModel.Move(index);

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

}
