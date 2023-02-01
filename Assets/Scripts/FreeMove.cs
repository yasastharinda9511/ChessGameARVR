using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMove : Moves
{

    public FreeMove(int source, int destination) : base(source, destination) 
    {
        MoveType = MOVETYPE.FREE;
        MoveScore = 10;
    
    }
    public override void DoMove()
    {
        Board.Instance.ChessBoard[Destination] = Board.Instance.ChessBoard[Source];
        MovePieceOneIsFirstMove = Board.Instance.ChessBoard[Destination].isFirstMove;

        Board.Instance.ChessBoard[Destination].isFirstMove = false;

        Board.Instance.ChessBoard[Destination].Index = Destination;
        Board.Instance.ChessBoard[Source] = null;
    }

    public override void UndoMove()
    {
        Board.Instance.ChessBoard[Source] = Board.Instance.ChessBoard[Destination];
        Board.Instance.ChessBoard[Source].Index = Source;
        Board.Instance.ChessBoard[Destination] = null;
        Board.Instance.ChessBoard[Source].isFirstMove = MovePieceOneIsFirstMove;
    }
}
