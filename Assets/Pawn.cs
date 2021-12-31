using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override void ChangePosition(int index) 
    {

        base.ChangePosition(index);

        if ((((int)(index / 8) == 7 && this.playerColor == PlayerColor.WHITE)) ||
            (((int)(index / 8) == 0 && this.playerColor == PlayerColor.BLACK))
            )
        {

            GameManager.Instance.ActivePieceSelector(this.playerColor, index );

        }

        MoveCoroute = false;
    }

    public override IEnumerator Move(int index)
    {
        Vector3 forward =  CalculateLocalPosition(index) - this.transform.localPosition;

        float cof;

        while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index))> 0.01f ) {

            Debug.Log("Move index is : " + index);
            Debug.Log("move is caling" + this.pieceName + playerColor);

            cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));
            cof = (cof > 2.5f) ? 2.5f : cof;
            this.transform.Translate(forward * Time.deltaTime * cof * 12.5f);
            yield return null; 

        }

        this.transform.localPosition = CalculateLocalPosition(index);
        MoveCoroute = false;
        yield return null;
    }

    public override void SelectChangeMaterial()
    {
        throw new System.NotImplementedException();
    }

    void checkSingleDoubleUp()
    {
        if (playerColor == PlayerColor.WHITE && InsideTheBoard(Index + 8) && Board.ChessBoard[Index + 8] == null && pieceName == PIECENAME.PAWN)
        {

            //ValidMoves.Add(Index + 8);
            AddIndex(Index + 8);
            checkDoubleUp();

        }
        else if (playerColor == PlayerColor.BLACK && InsideTheBoard(Index - 8) && Board.ChessBoard[Index - 8] == null && pieceName == PIECENAME.PAWN)
        {

            //ValidMoves.Add(Index - 8);
            AddIndex(Index - 8);
            checkDoubleUp();

        }

    }
    void checkDoubleUp()
    {

        if (playerColor == PlayerColor.WHITE && InsideTheBoard(Index + 16) && Board.ChessBoard[Index + 16] == null && isFirstMove && pieceName == PIECENAME.PAWN)
        {

            //ValidMoves.Add(Index + 16);
            AddIndex(Index + 16);
        }
        else if (playerColor == PlayerColor.BLACK && InsideTheBoard(Index - 16) && Board.ChessBoard[Index - 16] == null && isFirstMove && pieceName == PIECENAME.PAWN)
        {

            //ValidMoves.Add(Index - 16);
            AddIndex(Index - 16);

        }

    }

    void checkEleminateCrossMove()
    {

        if (playerColor == PlayerColor.WHITE && pieceName == PIECENAME.PAWN)
        {
            if (InsideTheBoard(Index + 7) && Board.ChessBoard[Index + 7] != null && (int)((Index + 7) / 8) == (int)(Index / 8) + 1)
                if (Board.ChessBoard[Index + 7].playerColor != this.playerColor) AddIndex(Index + 7);//ValidMoves.Add(Index + 7);
            if (InsideTheBoard(Index + 9)  && Board.ChessBoard[Index + 9] != null && (int)((Index + 9) / 8) == (int)(Index / 8) + 1)
                if (Board.ChessBoard[Index + 9].playerColor != this.playerColor) AddIndex(Index + 9);//ValidMoves.Add(Index + 9);

        }
        else if (playerColor == PlayerColor.BLACK && pieceName == PIECENAME.PAWN)
        {

            if (InsideTheBoard(Index - 7) && Board.ChessBoard[Index - 7] != null && (int)((Index - 7) / 8) + 1 == (int)(Index / 8))
                if (Board.ChessBoard[Index - 7].playerColor != this.playerColor)  AddIndex(Index - 7);//ValidMoves.Add(Index - 7);
            if (InsideTheBoard(Index - 9) && Board.ChessBoard[Index - 9] != null && (int)((Index - 9) / 8) + 1 == (int)(Index / 8))
                if (Board.ChessBoard[Index - 9].playerColor != this.playerColor) AddIndex(Index - 9);//ValidMoves.Add(Index - 9);

        }

    }

    public override List<int> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkSingleDoubleUp();
        checkEleminateCrossMove();

        return ValidMoves;
    }
}
