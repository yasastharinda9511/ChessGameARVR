using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    enum CASTLINGTYPE { 
        NONE,
        WHITE_LEFT,
        WHITE_RIGHT,
        BLACK_LEFT,
        BLACK_RIGHT
    }

    CASTLINGTYPE castling;

    public King(PlayerColor playerColor , int index) : base(playerColor , index) {

        this.pieceName = PIECENAME.KING;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 900 : -900;
        this.AbsPieceValue = 900;
        this.PieceThreatCoef = 1;

    }
    public King(PlayerColor playerColor , int index , KingModel kingModel) : base(playerColor , index, kingModel) {

        this.pieceName = PIECENAME.KING;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 900 : -900;
        this.AbsPieceValue = 900;
        this.PieceThreatCoef = 1;

    }
    public override List<Moves> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkSingleAllDirections();
        KingCastlingValid();
        return ValidMoves;
    }

    public override void ChangePosition(int index)
    {
        bool kingCastling = (Mathf.Abs(this.Index - index) == 2) ? true : false;

        if (kingCastling)
        {
            this.KingCastling();
        }
        else {

            base.ChangePosition(index);
        }
    }

    void KingCastling()
    {
        Debug.Log("KingCastling" + castling);

        if (castling == CASTLINGTYPE.WHITE_LEFT)
        {

            Board.Instance.ChessBoard[0].ChangePosition(3);
            base.ChangePosition(2);

        }
        else if (castling == CASTLINGTYPE.WHITE_RIGHT)
        {

            Board.Instance.ChessBoard[7].ChangePosition(5);
            base.ChangePosition(6);

        }
        else if (castling == CASTLINGTYPE.BLACK_LEFT)
        {
            Board.Instance.ChessBoard[56].ChangePosition(59);
            base.ChangePosition(58);
        }
        else if (castling == CASTLINGTYPE.BLACK_RIGHT)
        {

            Board.Instance.ChessBoard[63].ChangePosition(61);
            base.ChangePosition(62);
        }

    }
    void checkSingleAllDirections()
    {

        int[] directions = new int[8] { 1, -1, 8, -8, 9, -9, 7, -7 };
        int newIndex;
        foreach (int i in directions)
        {
            newIndex = Index + i;

            if (!InsideTheBoard(newIndex)) continue;

            if (Board.Instance.ChessBoard[Index + i] == null)
            {
                if ((i == 1 || i == -1) && IsAdjacentColumn(Index + i, Index) && CheckInSameRow(Index + i, Index)) AddMove(new FreeMove(Index, Index + i));
                else if ((i == 8 || i == -8) && CheckInSameColumn(Index + i, Index) && IsAdjacentRow(Index + i, Index)) AddMove(new FreeMove(Index, Index + i));
                else if ((i == 7 || i == -7) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index)) AddMove(new FreeMove(Index, Index + i));
                else if ((i == 9 || i == -9) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index)) AddMove(new FreeMove(Index, Index + i));

            }
            else if (Board.Instance.ChessBoard[Index + i] != null && Board.Instance.ChessBoard[Index + i].playerColor != playerColor) 
            {

                if ((i == 1 || i == -1) && IsAdjacentColumn(Index + i, Index) && CheckInSameRow(Index + i, Index)) AddMove(new AttackMove(Index , Index + i));
                else if ((i == 8 || i == -8) && CheckInSameColumn(Index + i, Index) && IsAdjacentRow(Index + i, Index)) AddMove(new AttackMove(Index, Index + i));
                else if ((i == 7 || i == -7) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index)) AddMove(new AttackMove(Index, Index + i));
                else if ((i == 9 || i == -9) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index)) AddMove(new AttackMove(Index, Index + i));

            }

        }

    }

    void KingCastlingValid() 
    {
        // left castling'
        castling = CASTLINGTYPE.NONE;

        if (this.playerColor == PlayerColor.WHITE &&
            this.Index == 4 && 
            Board.Instance.ChessBoard[4] != null &&
            Board.Instance.ChessBoard[4].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[0] != null &&
            Board.Instance.ChessBoard[0].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[0].playerColor == PlayerColor.WHITE &&
            Board.Instance.ChessBoard[0].isFirstMove &&
            Board.Instance.ChessBoard[1] == null &&
            Board.Instance.ChessBoard[2] == null &&
            Board.Instance.ChessBoard[3] == null &&
            !Board.Instance.WhiteChecked
            )
        {

            if (ValidMoves.Find(x => x.Destination == 3) != null) {
                AddMove(new KingCastlingMove(Index, 2, MOVETYPE.KING_CASTLING_WHITE_LEFT));
                castling = CASTLINGTYPE.WHITE_LEFT;

            } 

        }
        
        if (this.playerColor == PlayerColor.WHITE &&
            this.Index == 4 &&
            Board.Instance.ChessBoard[4] != null &&
            Board.Instance.ChessBoard[4].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[7] != null &&
            Board.Instance.ChessBoard[7].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[7].playerColor == PlayerColor.WHITE &&
            Board.Instance.ChessBoard[7].isFirstMove &&
            Board.Instance.ChessBoard[5] == null &&
            Board.Instance.ChessBoard[6] == null &&
            !Board.Instance.WhiteChecked
            )
        {

            if (ValidMoves.Find(x => x.Destination == 5) != null) {

                AddMove(new KingCastlingMove(Index, 6, MOVETYPE.KING_CASTLING_WHITE_RIGHT));
                castling = CASTLINGTYPE.WHITE_RIGHT;
            }
            
        }



        if (this.playerColor == PlayerColor.BLACK &&
            this.Index == 60 &&
            Board.Instance.ChessBoard[60] != null &&
            Board.Instance.ChessBoard[60].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[56] != null && 
            Board.Instance.ChessBoard[56].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[56].playerColor == PlayerColor.BLACK &&
            Board.Instance.ChessBoard[56].isFirstMove &&
            Board.Instance.ChessBoard[57] == null &&
            Board.Instance.ChessBoard[58] == null &&
            Board.Instance.ChessBoard[59] == null &&
            !Board.Instance.BlackChecked
            )
        {

            if (ValidMoves.Find(x => x.Destination == 59) != null) {

                AddMove(new KingCastlingMove(Index, 58, MOVETYPE.KING_CASTLING_BLACK_LEFT));
                castling = CASTLINGTYPE.BLACK_LEFT;
            }
            
        }

        if (this.playerColor == PlayerColor.BLACK &&
            this.Index == 60 &&
            Board.Instance.ChessBoard[60] != null &&
            Board.Instance.ChessBoard[60].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[63] != null &&
            Board.Instance.ChessBoard[63].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[63].playerColor == PlayerColor.BLACK &&
            Board.Instance.ChessBoard[63].isFirstMove &&
            Board.Instance.ChessBoard[61] == null &&
            Board.Instance.ChessBoard[62] == null &&
            !Board.Instance.BlackChecked
            )
        {
            if (ValidMoves.Find(x => x.Destination == 61) != null) {

                AddMove(new KingCastlingMove(Index, 62, MOVETYPE.KING_CASTLING_BLACK_RIGHT));
                castling = CASTLINGTYPE.BLACK_RIGHT;
            }

        }

    }
}
