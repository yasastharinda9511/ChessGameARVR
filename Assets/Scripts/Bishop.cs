using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Bishop : Piece
{

    private const int scoreValue = 30;

    public Bishop(PlayerColor playerColor , int index) : base(playerColor , index)
    {

        this.pieceName = PIECENAME.BISHOP;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 30 : -30;
        this.AbsPieceValue = 30;
        this.PieceThreatCoef = 1;

    }
    public Bishop(PlayerColor playerColor, int index , PieceModel bishopModel) : base(playerColor, index , bishopModel)
    {

        this.pieceName = PIECENAME.BISHOP;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 30 : -30;
        this.AbsPieceValue = 30;
        this.PieceThreatCoef = 1;

    }

    public override List<Moves> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkCrossInfinite();
        return ValidMoves;
    }
    void checkCrossInfinite()
    {

        int[] directions = new int[4] { 7, -7, 9, -9 };
        int index;
        int prevIndex;
        foreach (var i in directions)
        {
            index = Index + i;
            prevIndex = Index;
            while (InsideTheBoard(index))
            {

                if (Board.Instance.ChessBoard[index] == null)
                {

                    if ((i == 7 || i == -7) && CheckAdjacentDiagonal(index, prevIndex) && InsideTheBoard(index)) AddMove(new FreeMove(Index , index));
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex)) AddMove(new FreeMove(Index, index));
                    else break;

                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor != this.playerColor))
                {
                    if ((i == 7 || i == -7) && CheckAdjacentDiagonal(index, prevIndex) && InsideTheBoard(index)) AddMove(new AttackMove(Index , index));
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex)) AddMove(new AttackMove(Index, index));
                    break;
                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor == this.playerColor))
                {
                    DefendingMovesScore += Board.Instance.ChessBoard[index].PieceThreatCoef;
                    break;
                }

                prevIndex = index;
                index += i;

            }

        }
    }
}
