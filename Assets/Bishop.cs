using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<int> CalculateValidMoves()
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

        while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index)) > 0.01f)
        {

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

                if (Board.ChessBoard[index] == null)
                {

                    if ((i == 7 || i == -7) && CheckAdjacentDiagonal(index, prevIndex) && InsideTheBoard(index))
                    {
                        AddIndex(index);
                        //ValidMoves.Add(index);
                    }
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex))
                    {
                        AddIndex(index);
                        //ValidMoves.Add(index);
                    }
                    else
                    {
                        break;
                    }

                }
                else if ((Board.ChessBoard[index] != null && Board.ChessBoard[index].playerColor != this.playerColor))
                {
                    if ((i == 7 || i == -7) && CheckAdjacentDiagonal(index, prevIndex) && InsideTheBoard(index))
                    {
                        AddIndex(index);
                        //ValidMoves.Add(index);
                    }
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex))
                    {
                        AddIndex(index);
                        //ValidMoves.Add(index);
                    }
                    break;
                }
                else if ((Board.ChessBoard[index] != null && Board.ChessBoard[index].playerColor == this.playerColor))
                {
                    break;
                }

                prevIndex = index;
                index += i;

            }

        }
    }
}
