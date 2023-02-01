using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public Knight(PlayerColor playerColor, int index) : base(playerColor, index)
    {
        this.pieceName = PIECENAME.KNIGHT;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 30 : -30;
        this.AbsPieceValue = 30;
        this.PieceThreatCoef = 1;
    }
    public Knight(PlayerColor playerColor, int index, KnightModel knightModel) : base(playerColor, index, knightModel)
    {
        this.pieceName = PIECENAME.KNIGHT;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 30 : -30;
        this.AbsPieceValue = 30;
        this.PieceThreatCoef = 1;
    }


    public override List<Moves> CalculateValidMoves()
    {

        ValidMoves.Clear();
        checkLShape();
        return ValidMoves;
    }
    void checkLShape()
    {
        if ((Index + 17 <= 63) && ColomnDifference(Index, Index + 17) == 1 && RowDifference(Index, Index + 17) == 2) UpdateValidMoves(Index + 17);
        if((Index - 17 >= 0) && ColomnDifference(Index, Index + 17) == 1 && RowDifference(Index, Index + 17) == 2) UpdateValidMoves(Index - 17);
        if((Index + 15 <= 63) && ColomnDifference(Index, Index + 15) == 1 && RowDifference(Index, Index + 15) == 2) UpdateValidMoves(Index +15);
        if((Index - 15 >= 0) && ColomnDifference(Index, Index + 15) == 1 && RowDifference(Index, Index + 15) == 2) UpdateValidMoves(Index -15);


        if ((Index + 10 <= 63) && ColomnDifference(Index, Index + 10) == 2 && RowDifference(Index, Index + 10) == 1) UpdateValidMoves(Index + 10);
        if ((Index - 10 >= 0) && ColomnDifference(Index, Index - 10) == 2 && RowDifference(Index, Index - 10) == 1) UpdateValidMoves(Index - 10);
        if ((Index + 6 <= 63) && ColomnDifference(Index, Index + 6) == 2 && RowDifference(Index, Index + 6) == 1) UpdateValidMoves(Index + 6);
        if ((Index - 6 >= 0) && ColomnDifference(Index, Index - 6) == 2 && RowDifference(Index, Index - 6) == 1) UpdateValidMoves(Index - 6);

    }

    public void UpdateValidMoves(int index)
    {
        if (Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor != this.playerColor) AddMove(new AttackMove(Index, index));
        else if (Board.Instance.ChessBoard[index] != null &&  Board.Instance.ChessBoard[index].playerColor == this.playerColor) this.DefendingMovesScore += Board.Instance.ChessBoard[index].PieceThreatCoef;
        else if (Board.Instance.ChessBoard[index] == null) AddMove(new FreeMove(Index, index));
    }
}
