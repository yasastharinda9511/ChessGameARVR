using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{

    private const int scoreValue = 30;
    public override List<Moves> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkCrossInfinite();
        return ValidMoves;
    }

    public override IEnumerator Move(int index)
    {
        Vector3 forward = CalculateLocalPosition(index) - this.transform.localPosition;
        float cof;

        if (this.isFirstMove) isFirstMove = false;

        while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index)) > 0.01f &&
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

    void checkCrossInfinite()
    {

        int[] directions = new int[4] { 7, -7, 9, -9 };
        int index;
        int prevIndex;
        foreach (var i in directions)
        {
            index = Index + i;
            prevIndex = Index;
            while (InsideTheBoard(index))
            {

                if (Board.Instance.ChessBoard[index] == null)
                {

                    if ((i == 7 || i == -7) && CheckAdjacentDiagonal(index, prevIndex) && InsideTheBoard(index))
                    {

                        AddIndex(index , MOVETYPE.FREE);
                        //ValidMoves.Add(index);
                    }
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex))
                    {
                        
                        AddIndex(index , MOVETYPE.FREE);
                        //ValidMoves.Add(index);
                    }
                    else
                    {
                        break;
                    }

                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor != this.playerColor))
                {
                    if ((i == 7 || i == -7) && CheckAdjacentDiagonal(index, prevIndex) && InsideTheBoard(index))
                    {
                        AddIndex(index , MOVETYPE.ATTACKING);
                        //ValidMoves.Add(index);
                    }
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex))
                    {
                        AddIndex(index , MOVETYPE.ATTACKING);
                        //ValidMoves.Add(index);
                    }
                    break;
                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor == this.playerColor))
                {
                    DefendingMovesScore += Board.Instance.ChessBoard[index].PieceThreatCoef;
                    break;
                }

                prevIndex = index;
                index += i;

            }

        }
    }
}
