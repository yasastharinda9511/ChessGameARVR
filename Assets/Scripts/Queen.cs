using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{

    public Queen(PlayerColor playerColor , int index) : base(playerColor , index )
    {

        this.pieceName = PIECENAME.QUEEN;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 90 : -90;
        this.AbsPieceValue = 90;
        this.PieceThreatCoef = 1;

    }
    
    public Queen(PlayerColor playerColor , int index , QueenModel queenModel) : base(playerColor , index , queenModel )
    {

        this.pieceName = PIECENAME.QUEEN;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 90 : -90;
        this.AbsPieceValue = 90;
        this.PieceThreatCoef = 1;

    }
    // Start is called before the first frame update

    void CheckInfiniteAllDirection()
    {

        CheckSquareInfinite();
        CheckCrossInfinite();

    }

    void CheckCrossInfinite()
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
                    if ((i == 7 || i == -7) && CheckAdjacentDiagonal(index, prevIndex) && InsideTheBoard(index)) AddMove(new AttackMove(Index, index));
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

    void CheckSquareInfinite()
    {

        int[] directions = new int[4] { 1, -1, 8, -8 };
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

                    if ((i == 1 || i == -1) && CheckInSameRow(index, prevIndex) && InsideTheBoard(index)) AddMove(new FreeMove(Index, index));
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, prevIndex) && InsideTheBoard(index)) AddMove(new FreeMove(Index, index));
                    else break;

                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor != this.playerColor))
                {
                    if ((i == 1 || i == -1) && CheckInSameRow(index, prevIndex) && InsideTheBoard(index)) AddMove(new AttackMove(Index, index));
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, prevIndex) && InsideTheBoard(index)) AddMove(new AttackMove(Index, index));
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

    public override List<Moves> CalculateValidMoves()
    {
        ValidMoves.Clear();
        CheckInfiniteAllDirection();
        return ValidMoves;
    }
}
