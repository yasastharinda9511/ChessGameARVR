using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override List<int> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkSquareInfinite();
        return ValidMoves;

    }

    public override IEnumerator Move(int index)
    {
        MoveCoroute = false;
        Vector3 forward = CalculateLocalPosition(index) - this.transform.localPosition;
        float cof;

        while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index)) > 0.01f)
        {

            cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));
            cof = (cof > 3) ? 3 : cof;
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

    void checkSquareInfinite()
    {
        int[] directions = new int[4] { 1, -1, 8, -8 };
        int index;
        int prevIndex;
        foreach (var i in directions)
        {
            Debug.Log("@" + "Rook " + this.Index);
            index = Index + i;
            prevIndex = Index;
            while (InsideTheBoard(index))
            {

                if (Board.ChessBoard[index] == null)
                {

                    if ((i == 1 || i == -1) && CheckInSameRow(index, prevIndex) && InsideTheBoard(index))
                    {
                        //ValidMoves.Add(index);
                        AddIndex(index);
                    }
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, prevIndex) && InsideTheBoard(index))
                    {
                        //ValidMoves.Add(index);
                        AddIndex(index);
                    }
                    else
                    {
                        break;
                    }

                }
                else if ((Board.ChessBoard[index] != null && Board.ChessBoard[index].playerColor != this.playerColor))
                {
                    if ((i == 1 || i == -1) && CheckInSameRow(index, prevIndex) && InsideTheBoard(index))
                    {
                        //ValidMoves.Add(index);
                        AddIndex(index);
                    }
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, prevIndex) && InsideTheBoard(index))
                    {
                        //ValidMoves.Add(index);
                        AddIndex(index);
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
