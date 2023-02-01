using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMove : Moves
{

    Piece attackedPiece;
    public AttackMove(int source, int destination) : base(source , destination) 
    {

        MoveType = MOVETYPE.ATTACKING;
        MoveScore = 100 + Board.Instance.ChessBoard[destination].AbsPieceValue * 0.5f;

    }
    public override void DoMove()
    {
        attackedPiece = Board.Instance.ChessBoard[Destination];
        Board.Instance.RemoveActivePiece(attackedPiece);

        Board.Instance.ChessBoard[Destination] = Board.Instance.ChessBoard[Source];

        MovePieceOneIsFirstMove = Board.Instance.ChessBoard[Destination].isFirstMove; // First Piece queen

        Board.Instance.ChessBoard[Destination].isFirstMove = false;


        Board.Instance.ChessBoard[Destination].Index = Destination;
        Board.Instance.ChessBoard[Source] = null;
    }

    public override void UndoMove()
    {
        Board.Instance.AddActivePiece(attackedPiece);

        Board.Instance.ChessBoard[Source] = Board.Instance.ChessBoard[Destination];
        Board.Instance.ChessBoard[Source].Index = Source;
        Board.Instance.ChessBoard[Source].isFirstMove = MovePieceOneIsFirstMove;

        Board.Instance.ChessBoard[Destination] = attackedPiece;
    }
}
