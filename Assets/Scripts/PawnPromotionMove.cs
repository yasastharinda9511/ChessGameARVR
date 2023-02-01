using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPromotionMove : Moves
{
    public PIECENAME PromotedPieceName { get; set; }
    Piece promotedPiece;
    Piece attackedPiece;
    Piece promotedPawnPiece;
    public PawnPromotionMove(int source, int destination , PIECENAME promotedPieceName) : base(source, destination)
    {
        MoveScore = 150;
        PromotedPieceName= promotedPieceName;
    }

    public override void DoMove()
    {
        if (Board.Instance.ChessBoard[Destination] != null)
        {

            attackedPiece = Board.Instance.ChessBoard[Destination];
            Board.Instance.RemoveActivePiece(attackedPiece);

        }

        promotedPawnPiece = Board.Instance.ChessBoard[Source];
        Board.Instance.ChessBoard[Source] = null;
        Board.Instance.RemoveActivePiece(promotedPawnPiece);

        if (PromotedPieceName == PIECENAME.QUEEN) promotedPiece = new Queen(promotedPawnPiece.playerColor, Destination);
        else if (PromotedPieceName == PIECENAME.ROOK) promotedPiece = new Rook(promotedPawnPiece.playerColor, Destination);
        else if (PromotedPieceName == PIECENAME.KNIGHT) promotedPiece = new Knight(promotedPawnPiece.playerColor, Destination);
        else if (PromotedPieceName == PIECENAME.BISHOP) promotedPiece = new Bishop(promotedPawnPiece.playerColor, Destination);

        Board.Instance.ChessBoard[Destination] = promotedPiece;
        Board.Instance.AddActivePiece(promotedPiece);
    }

    public override void UndoMove()
    {
        Board.Instance.ChessBoard[Source] = promotedPawnPiece;
        if (attackedPiece != null)
        {

            Board.Instance.AddActivePiece(attackedPiece);
            Board.Instance.RemoveActivePiece(Board.Instance.ChessBoard[Destination]);

        }
        List<Piece> activePieces = (Board.Instance.ChessBoard[Destination].playerColor == PlayerColor.WHITE) ? Board.Instance.WhiteActivePieces : Board.Instance.BlackActivePieces;
        activePieces.Remove(Board.Instance.ChessBoard[Destination]);
        Board.Instance.ChessBoard[Source] = promotedPawnPiece;
        activePieces.Add(promotedPawnPiece);
        Board.Instance.ChessBoard[Destination] = attackedPiece;
    }
}
