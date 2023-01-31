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

    public override void ChangePosition(int index) 
    {

        base.ChangePosition(index);

        if ((((int)(index / 8) == 7 && this.playerColor == PlayerColor.WHITE)) ||
            (((int)(index / 8) == 0 && this.playerColor == PlayerColor.BLACK))
            )
        {

        }

        MoveCoroute = false;
    }

    public override IEnumerator Move(int index)
    {
        Vector3 forward =  CalculateLocalPosition(index) - this.transform.localPosition;
        float cof;

        while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index))> 0.01f &&
               Vector3.Dot(CalculateLocalPosition(index) - this.transform.localPosition, forward) >= 0  ) 
        {

            cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));
            cof = (cof > 1.5f) ? 1.5f : cof;
            this.transform.Translate(forward * Time.deltaTime * cof * speed);
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
    void CheckSingleDoubleUpWhite() {

        if (Index + 8 <= 63 && Board.Instance.ChessBoard[Index + 8] == null)
        {

            if ((int)(Index + 8) / 8 == 7)
            {

                AddIndex(Index + 8, MOVETYPE.PAWN_PROMOTION);

            }
            else {

                AddIndex(Index + 8, MOVETYPE.FREE);

            }
            
            if (isFirstMove) DoubleUpWhite();

        }

    }

    void CheckSingleDoubleUpBlack() {


        if (Index - 8 >=0 && Board.Instance.ChessBoard[Index - 8] == null)
        {

            if ((int)((Index - 8) / 8) == 0)
            {

                AddIndex(Index - 8, MOVETYPE.PAWN_PROMOTION);

            }
            else {

                AddIndex(Index - 8, MOVETYPE.FREE);

            }
            
            if (isFirstMove) DoubleUpBlack();

        }

    }

    void DoubleUpWhite() {

        if (Index + 16 <= 63 && Board.Instance.ChessBoard[Index + 16] == null) {

            AddIndex(Index + 16, MOVETYPE.FREE);
        } 
    
    }
    void DoubleUpBlack() {

        if (Index - 16 >= 0 && Board.Instance.ChessBoard[Index - 16] == null) {

            AddIndex(Index - 16 , MOVETYPE.FREE);

        }
        
    
    }

    void checkEleminateCrossMoveWhite()
    {

        if (Index + 7 <= 63 &&
            Board.Instance.ChessBoard[Index + 7] != null &&
            (int)((Index + 7) / 8) == (int)(Index / 8) + 1 &&
            Board.Instance.ChessBoard[Index + 7].playerColor != playerColor 
            )
        {
            AddIndex(Index + 7 , MOVETYPE.ATTACKING);
        }
        //ValidMoves.Add(Index + 7);
        if (Index + 9 <= 63 &&
            Board.Instance.ChessBoard[Index + 9] != null &&
            (int)((Index + 9) / 8) == (int)(Index / 8) + 1 &&
            Board.Instance.ChessBoard[Index + 9].playerColor != playerColor
            )
        {
            AddIndex(Index + 9 , MOVETYPE.ATTACKING);
        }

    }

    void checkEleminateCrossMoveBlack() {

        if (Index - 7 >= 0 &&
            Board.Instance.ChessBoard[Index - 7] != null &&
            (int)((Index - 7) / 8) + 1 == (int)(Index / 8) &&
             Board.Instance.ChessBoard[Index - 7].playerColor != this.playerColor)
        {

            AddIndex(Index - 7 , MOVETYPE.ATTACKING);

        }
        //ValidMoves.Add(Index - 7);
        if (Index - 9 >=0 &&
            Board.Instance.ChessBoard[Index - 9] != null &&
            (int)((Index - 9) / 8) + 1 == (int)(Index / 8) &&
            Board.Instance.ChessBoard[Index - 9].playerColor != this.playerColor)
        {
            AddIndex(Index - 9 , MOVETYPE.ATTACKING);
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
