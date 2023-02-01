using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingCastlingMove : Moves
{
    bool MovePieceTwoIsFirstMove;
    public KingCastlingMove(int source, int destination , MOVETYPE kingcastlingType) : base(source, destination)
    {
        MoveScore = 120;
        MoveType = kingcastlingType;
    }

    public override void DoMove()
    {
        if (MoveType == MOVETYPE.KING_CASTLING_BLACK_LEFT)
        {
            Debug.Log(1);
            Board.Instance.ChessBoard[58] = Board.Instance.ChessBoard[60];
            Board.Instance.ChessBoard[58].Index = 58;
            MovePieceOneIsFirstMove = Board.Instance.ChessBoard[58].isFirstMove;
            Board.Instance.ChessBoard[58].isFirstMove = false;

            Board.Instance.ChessBoard[60] = null;

            Board.Instance.ChessBoard[59] = Board.Instance.ChessBoard[56];
            Board.Instance.ChessBoard[59].Index = 59;
            MovePieceTwoIsFirstMove = Board.Instance.ChessBoard[59].isFirstMove;
            Board.Instance.ChessBoard[59].isFirstMove = false;

            Board.Instance.ChessBoard[56] = null;
        }
        else if (MoveType == MOVETYPE.KING_CASTLING_BLACK_RIGHT)
        {
            Debug.Log(2);
            Board.Instance.ChessBoard[62] = Board.Instance.ChessBoard[60];
            Board.Instance.ChessBoard[62].Index = 62;
            MovePieceOneIsFirstMove = Board.Instance.ChessBoard[62].isFirstMove;
            Board.Instance.ChessBoard[62].isFirstMove = false;

            Board.Instance.ChessBoard[60] = null;

            Board.Instance.ChessBoard[61] = Board.Instance.ChessBoard[63];
            Board.Instance.ChessBoard[61].Index = 61;
            MovePieceTwoIsFirstMove = Board.Instance.ChessBoard[61].isFirstMove;
            Board.Instance.ChessBoard[61].isFirstMove = false;

            Board.Instance.ChessBoard[63] = null;

        }
        else if (MoveType == MOVETYPE.KING_CASTLING_WHITE_LEFT)
        {
            Debug.Log(3);
            Board.Instance.ChessBoard[2] = Board.Instance.ChessBoard[4];
            Board.Instance.ChessBoard[2].Index = 2;
            MovePieceOneIsFirstMove = Board.Instance.ChessBoard[2].isFirstMove;
            Board.Instance.ChessBoard[2].isFirstMove = false;

            Board.Instance.ChessBoard[4] = null;

            Board.Instance.ChessBoard[3] = Board.Instance.ChessBoard[0];
            Board.Instance.ChessBoard[3].Index = 3;
            MovePieceTwoIsFirstMove = Board.Instance.ChessBoard[3].isFirstMove;
            Board.Instance.ChessBoard[3].isFirstMove = false;

            Board.Instance.ChessBoard[0] = null;

        }
        else if (MoveType == MOVETYPE.KING_CASTLING_WHITE_RIGHT)
        {
            Debug.Log(4);
            Board.Instance.ChessBoard[6] = Board.Instance.ChessBoard[4];
            Board.Instance.ChessBoard[6].Index = 6;
            MovePieceOneIsFirstMove = Board.Instance.ChessBoard[6].isFirstMove;
            Board.Instance.ChessBoard[6].isFirstMove = false;

            Board.Instance.ChessBoard[4] = null;

            Board.Instance.ChessBoard[5] = Board.Instance.ChessBoard[7];
            Board.Instance.ChessBoard[5].Index = 5;
            MovePieceTwoIsFirstMove = Board.Instance.ChessBoard[5].isFirstMove;
            Board.Instance.ChessBoard[5].isFirstMove = false;

            Board.Instance.ChessBoard[7] = null;
        }

        Debug.Log( Board.Instance.PrintBoardNow());

    }

    public override void UndoMove()
    {

        Debug.Log(Board.Instance.PrintBoardNow());
        if (MoveType == MOVETYPE.KING_CASTLING_BLACK_LEFT)
        {
            Debug.Log(1);
            Board.Instance.ChessBoard[60] = Board.Instance.ChessBoard[58];
            Board.Instance.ChessBoard[60].Index = 60;
            Board.Instance.ChessBoard[60].isFirstMove = MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[58] = null;

            Board.Instance.ChessBoard[56] = Board.Instance.ChessBoard[59];
            Board.Instance.ChessBoard[56].Index = 60;
            Board.Instance.ChessBoard[56].isFirstMove = MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[59] = null;

        }
        else if (MoveType == MOVETYPE.KING_CASTLING_BLACK_RIGHT)
        {
            Debug.Log(2);
            Board.Instance.ChessBoard[60] = Board.Instance.ChessBoard[62];
            Board.Instance.ChessBoard[60].Index = 60;
            Board.Instance.ChessBoard[60].isFirstMove = MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[62] = null;

            Board.Instance.ChessBoard[63] = Board.Instance.ChessBoard[61];
            Board.Instance.ChessBoard[63].Index = 63;
            Board.Instance.ChessBoard[63].isFirstMove = MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[61] = null;

        }
        else if (MoveType == MOVETYPE.KING_CASTLING_WHITE_LEFT)
        {
            Debug.Log(3);
            Board.Instance.ChessBoard[4] = Board.Instance.ChessBoard[2];
            Board.Instance.ChessBoard[4].Index = 4;
            Board.Instance.ChessBoard[4].isFirstMove = MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[2] = null;

            Board.Instance.ChessBoard[0] = Board.Instance.ChessBoard[3];
            Board.Instance.ChessBoard[0].Index = 0;
            Board.Instance.ChessBoard[0].isFirstMove = MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[3] = null;

        }
        else if (MoveType == MOVETYPE.KING_CASTLING_WHITE_RIGHT)
        {
            Debug.Log(4);
            Board.Instance.ChessBoard[4] = Board.Instance.ChessBoard[6];
            Board.Instance.ChessBoard[4].Index = 4;
            Board.Instance.ChessBoard[4].isFirstMove = MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[6] = null;

            Board.Instance.ChessBoard[7] = Board.Instance.ChessBoard[5];
            Board.Instance.ChessBoard[7].Index = 7;
            Board.Instance.ChessBoard[7].isFirstMove = MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[5] = null;

        }

    }
}
