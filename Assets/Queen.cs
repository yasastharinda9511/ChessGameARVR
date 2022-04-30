using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{

    public Queen(PlayerColor playerColor , int index) : base(playerColor , index )
    {

        this.pieceName = PIECENAME.QUEEN;
        this.PieceValue = (playerColor == PlayerColor.WHITE) ? 90 : -90;
        this.AbsPieceValue = 90;
        this.PieceThreatCoef = 1;

    }
    public override IEnumerator Move(int index)
    {
        MoveCoroute = false;
        Vector3 forward = CalculateLocalPosition(index) - this.transform.localPosition;
        float cof;

        while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index)) > 0.01f &&
               Vector3.Dot(CalculateLocalPosition(index) - this.transform.localPosition, forward) >= 0
            )
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

    // Start is called before the first frame update

    void checkInfiniteAllDirection()
    {

        checkSquareInfinite();
        checkCrossInfinite();

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
                    }
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex))
                    {
                        AddIndex(index , MOVETYPE.FREE);
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
                    }
                    else if ((i == 9 || i == -9) && InsideTheBoard(index) && CheckAdjacentDiagonal(index, prevIndex))
                    {
                        AddIndex(index , MOVETYPE.ATTACKING);
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

    void checkSquareInfinite()
    {

        int[] directions = new int[4] { 1, -1, 8, -8 };
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

                    if ((i == 1 || i == -1) && CheckInSameRow(index, prevIndex) && InsideTheBoard(index))
                    {
                        AddIndex(index , MOVETYPE.FREE);
                    }
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, prevIndex) && InsideTheBoard(index))
                    {
                        AddIndex(index , MOVETYPE.FREE);
                    }
                    else
                    {
                        break;
                    }

                }
                else if ((Board.Instance.ChessBoard[index] != null && Board.Instance.ChessBoard[index].playerColor != this.playerColor))
                {
                    if ((i == 1 || i == -1) && CheckInSameRow(index, prevIndex) && InsideTheBoard(index))
                    {
                        AddIndex(index , MOVETYPE.ATTACKING);
                    }
                    else if ((i == 8 || i == -8) && CheckInSameColumn(index, prevIndex) && InsideTheBoard(index))
                    {
                        AddIndex(index , MOVETYPE.ATTACKING);
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

    public override List<Moves> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkInfiniteAllDirection();
        return ValidMoves;
    }
}
