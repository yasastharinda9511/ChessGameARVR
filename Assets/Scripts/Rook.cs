using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;

public class Rook : Piece
{
    public Rook(PlayerColor playerColor, int index) : base (playerColor , index)
    {
        this.pieceName = PIECENAME.ROOK;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 50 : -50;
        this.AbsPieceValue = 50;
        this.PieceThreatCoef = 1;
    }
    
    public Rook(PlayerColor playerColor, int index, RookModel rookModel) : base (playerColor , index, rookModel)
    {
        this.pieceName = PIECENAME.ROOK;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 50 : -50;
        this.AbsPieceValue = 50;
        this.PieceThreatCoef = 1;
    }
    public override List<Moves> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkSquareInfinite();
        return ValidMoves;

    }

    void checkSquareInfinite()
    {
        int[] directions = new int[4] { 1, -1, 8, -8 };
        int index;
        foreach (var i in directions)
        {
            index = Index + i;
            while (InsideTheBoard(index))
            {

                if (Board.Instance.ChessBoard[index] == null)
                {

                    if ((i == 1 || i == -1) && CheckInSameRow(index, Index)) AddMove(new FreeMove(Index, index));
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, Index)) AddMove(new FreeMove(Index, index));
                    else break;

                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor != this.playerColor))
                {
                    if ((i == 1 || i == -1) && CheckInSameRow(index, Index)) AddMove(new AttackMove(Index, index));
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, Index)) AddMove(new AttackMove(Index, index));
                    break;
                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor == this.playerColor))
                {
                    DefendingMovesScore += Board.Instance.ChessBoard[index].PieceThreatCoef;
                    break;
                }
                index += i;

            }

        }
    }
}
