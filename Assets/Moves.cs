using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moves
{
    public int Source { get; set; }
    public int Destination { get; set; }
    public MOVETYPE MoveType { get; set; }
    public PIECENAME AttackedPiece { get; set; }
    public PIECENAME PromotedPiece { get; set; }
    public int MoveScore { get; set; }

    public Moves(int source , int destination , MOVETYPE moveType, PIECENAME promotedPiece ) 
    {
        this.Source = source;
        this.Destination = destination;
        this.MoveType = moveType;

        this.PromotedPiece = promotedPiece;

        if (moveType == MOVETYPE.ATTACKING)
        {
            //if (Board.Instance.ChessBoard[destination] == null) Debug.Log("Null Value");
            MoveScore = 100 + Board.Instance.ChessBoard[destination].AbsPieceValue;

        }
        else if (moveType == MOVETYPE.PAWN_PROMOTION)
        {

            MoveScore = 150;


        }
        else if (moveType == MOVETYPE.KING_CASTLING_BLACK_LEFT ||
                moveType == MOVETYPE.KING_CASTLING_BLACK_RIGHT ||
                moveType == MOVETYPE.KING_CASTLING_WHITE_LEFT ||
                moveType == MOVETYPE.KING_CASTLING_WHITE_RIGHT)
        {

            MoveScore = 120;

        }
        else {

            MoveScore = 10;
            
        }
    }
}
