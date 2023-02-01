using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public Pawn(PlayerColor playerColor, int index) : base(playerColor, index) {

        this.pieceName = PIECENAME.PAWN;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 10 : -10;
        this.AbsPieceValue = 10;
        this.PieceThreatCoef = 1;

    }
    public Pawn(PlayerColor playerColor, int index, PawnModel pawnModel) : base(playerColor, index , pawnModel )
    {

        this.pieceName = PIECENAME.PAWN;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 10 : -10;
        this.AbsPieceValue = 10;
        this.PieceThreatCoef = 1;

    }
    void CheckSingleDoubleUpWhite() {

        if (Index + 8 <= 63 && Board.Instance.ChessBoard[Index + 8] == null)
        {

            if ((int)(Index + 8) / 8 == 7)
            {
                AddMove(new PawnPromotionMove(Index, Index + 8, PIECENAME.ROOK));
                AddMove(new PawnPromotionMove(Index, Index + 8, PIECENAME.BISHOP));
                AddMove(new PawnPromotionMove(Index, Index + 8, PIECENAME.KNIGHT));
                AddMove(new PawnPromotionMove(Index, Index + 8, PIECENAME.QUEEN));

            }
            else AddMove(new FreeMove(Index , Index + 8));
       
            if (isFirstMove) DoubleUpWhite();

        }

    }

    void CheckSingleDoubleUpBlack() {


        if (Index - 8 >=0 && Board.Instance.ChessBoard[Index - 8] == null)
        {

            if ((int)((Index - 8) / 8) == 0)
            {

                AddMove(new PawnPromotionMove(Index, Index - 8, PIECENAME.ROOK));
                AddMove(new PawnPromotionMove(Index, Index - 8, PIECENAME.BISHOP));
                AddMove(new PawnPromotionMove(Index, Index - 8, PIECENAME.KNIGHT));
                AddMove(new PawnPromotionMove(Index, Index - 8, PIECENAME.QUEEN));

            }
            else AddMove(new FreeMove(Index, Index - 8));
            
            if (isFirstMove) DoubleUpBlack();

        }

    }

    void DoubleUpWhite() {

        if (Index + 16 <= 63 && Board.Instance.ChessBoard[Index + 16] == null) AddMove(new FreeMove(Index, Index + 16)); 
    
    }
    void DoubleUpBlack() {

        if (Index - 16 >= 0 && Board.Instance.ChessBoard[Index - 16] == null) AddMove(new FreeMove(Index, Index - 16));
     
    }

    void checkEleminateCrossMoveWhite()
    {

        if (Index + 7 <= 63 &&
            Board.Instance.ChessBoard[Index + 7] != null &&
            (int)((Index + 7) / 8) == (int)(Index / 8) + 1 &&
            Board.Instance.ChessBoard[Index + 7].playerColor != playerColor 
            )
        {
            AddMove(new AttackMove(Index, Index + 7));
        }
        //ValidMoves.Add(Index + 7);
        if (Index + 9 <= 63 &&
            Board.Instance.ChessBoard[Index + 9] != null &&
            (int)((Index + 9) / 8) == (int)(Index / 8) + 1 &&
            Board.Instance.ChessBoard[Index + 9].playerColor != playerColor
            )
        {
            AddMove(new AttackMove(Index, Index + 9));
        }

    }

    void checkEleminateCrossMoveBlack() {

        if (Index - 7 >= 0 &&
            Board.Instance.ChessBoard[Index - 7] != null &&
            (int)((Index - 7) / 8) + 1 == (int)(Index / 8) &&
             Board.Instance.ChessBoard[Index - 7].playerColor != this.playerColor)
        {
            AddMove(new AttackMove(Index, Index -7));

        }
        //ValidMoves.Add(Index - 7);
        if (Index - 9 >=0 &&
            Board.Instance.ChessBoard[Index - 9] != null &&
            (int)((Index - 9) / 8) + 1 == (int)(Index / 8) &&
            Board.Instance.ChessBoard[Index - 9].playerColor != this.playerColor)
        {
            AddMove(new AttackMove(Index, Index - 9));
        }
                //ValidMoves.Add(Index - 9);

    }

    public override List<Moves> CalculateValidMoves()
    {
        ValidMoves.Clear();

        if (playerColor == PlayerColor.WHITE)
        {

            CheckSingleDoubleUpWhite();
            checkEleminateCrossMoveWhite();

        }
        else {

            CheckSingleDoubleUpBlack();
            checkEleminateCrossMoveBlack();

        }

        return ValidMoves;
    }

}
