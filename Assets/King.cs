using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{

    enum CASTLINGTYPE { 
        NONE,
        WHITE_LEFT,
        WHITE_RIGHT,
        BLACK_LEFT,
        BLACK_RIGHT
    }

    CASTLINGTYPE castling;
    public override List<int> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkSingleAllDirections();
        KingCastlingValid();
        return ValidMoves;
    }

    public override void ChangePosition(int index)
    {
        bool kingCastling = (Mathf.Abs(this.Index - index) == 2) ? true : false;

        if (kingCastling)
        {
            this.KingCastling();
        }
        else {

            base.ChangePosition(index);
        }
    }

    void KingCastling()
    {
        Debug.Log("KingCastling" + castling);

        if (castling == CASTLINGTYPE.WHITE_LEFT)
        {

            Board.Instance.ChessBoard[0].ChangePosition(3);
            base.ChangePosition(2);

        }
        else if (castling == CASTLINGTYPE.WHITE_RIGHT)
        {

            Board.Instance.ChessBoard[7].ChangePosition(5);
            base.ChangePosition(6);

        }
        else if (castling == CASTLINGTYPE.BLACK_LEFT)
        {
            Board.Instance.ChessBoard[56].ChangePosition(59);
            base.ChangePosition(58);
        }
        else if (castling == CASTLINGTYPE.BLACK_RIGHT)
        {

            Board.Instance.ChessBoard[63].ChangePosition(61);
            base.ChangePosition(62);
        }

    }
    public override IEnumerator Move(int index)
    {
        MoveCoroute = false;
        Vector3 forward = CalculateLocalPosition(index) - this.transform.localPosition;
        float cof;

        while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index)) > 0.01f && 
               Vector3.Dot(CalculateLocalPosition(index) - this.transform.localPosition, forward) >= 0)
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

    void checkSingleAllDirections()
    {

        int[] directions = new int[8] { 1, -1, 8, -8, 9, -9, 7, -7 };
        int newIndex;
        foreach (int i in directions)
        {
            newIndex = Index + i;

            if (!InsideTheBoard(newIndex)) continue;

            if (Board.Instance.ChessBoard[Index + i] == null)
            {
                if ((i == 1 || i == -1) && IsAdjacentColumn(Index + i, Index) && CheckInSameRow(Index + i, Index))
                {
                    AddIndex(Index + i);
                    // ValidMoves.Add(Index + i);

                }
                else if ((i == 8 || i == -8) && CheckInSameColumn(Index + i, Index) && IsAdjacentRow(Index + i, Index))
                {
                    AddIndex(Index + i);
                    //ValidMoves.Add(Index + i);

                }
                else if ((i == 7 || i == -7) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index))
                {
                    AddIndex(Index + i);
                    //ValidMoves.Add(Index + i);

                }
                else if ((i == 9 || i == -9) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index))
                {
                    AddIndex(Index + i);
                    //ValidMoves.Add(Index + i);

                }

            }
            else if (Board.Instance.ChessBoard[Index + i] != null && Board.Instance.ChessBoard[Index + i].playerColor != this.playerColor)
            {

                if ((i == 1 || i == -1) && IsAdjacentColumn(Index + i, Index) && CheckInSameRow(Index + i, Index))
                {
                    AddIndex(Index + i);
                    //ValidMoves.Add(Index + i);

                }
                else if ((i == 8 || i == -8) && CheckInSameColumn(Index + i, Index) && IsAdjacentRow(Index + i, Index))
                {
                    AddIndex(Index + i);
                    //ValidMoves.Add(Index + i);

                }
                else if ((i == 7 || i == -7) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index))
                {
                    AddIndex(Index + i);
                    //ValidMoves.Add(Index + i);

                }
                else if ((i == 9 || i == -9) && IsAdjacentRow(Index + i, Index) && IsAdjacentColumn(Index + i, Index))
                {
                    AddIndex(Index + i);
                    //ValidMoves.Add(Index + i);

                }

            }

        }

    }

    void KingCastlingValid() 
    {
        // left castling'
        castling = CASTLINGTYPE.NONE;

        if (this.playerColor == PlayerColor.WHITE &&
            this.Index == 4 && 
            Board.Instance.ChessBoard[4] != null &&
            Board.Instance.ChessBoard[4].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[0] != null &&
            Board.Instance.ChessBoard[0].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[0].playerColor == PlayerColor.WHITE &&
            Board.Instance.ChessBoard[0].isFirstMove &&
            Board.Instance.ChessBoard[1] == null &&
            Board.Instance.ChessBoard[2] == null &&
            Board.Instance.ChessBoard[3] == null &&
            !Board.Instance.WhiteChecked
            )
        {

            if (ValidMoves.Find(x => x == 3) == 3) {
                AddIndex(2);
                castling = CASTLINGTYPE.WHITE_LEFT;

            } 

        }
        
        if (this.playerColor == PlayerColor.WHITE &&
            this.Index == 4 &&
            Board.Instance.ChessBoard[4] != null &&
            Board.Instance.ChessBoard[4].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[7] != null &&
            Board.Instance.ChessBoard[7].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[7].playerColor == PlayerColor.WHITE &&
            Board.Instance.ChessBoard[7].isFirstMove &&
            Board.Instance.ChessBoard[5] == null &&
            Board.Instance.ChessBoard[6] == null &&
            !Board.Instance.WhiteChecked
            )
        {

            if (ValidMoves.Find(x => x == 5) == 5) {

                AddIndex(6);
                castling = CASTLINGTYPE.WHITE_RIGHT;
            }
            
        }



        if (this.playerColor == PlayerColor.BLACK &&
            this.Index == 60 &&
            Board.Instance.ChessBoard[60] != null &&
            Board.Instance.ChessBoard[60].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[56] != null && 
            Board.Instance.ChessBoard[56].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[56].playerColor == PlayerColor.BLACK &&
            Board.Instance.ChessBoard[56].isFirstMove &&
            Board.Instance.ChessBoard[57] == null &&
            Board.Instance.ChessBoard[58] == null &&
            Board.Instance.ChessBoard[59] == null &&
            !Board.Instance.BlackChecked
            )
        {

            if (ValidMoves.Find(x => x == 59) == 59) {

                AddIndex(58);
                castling = CASTLINGTYPE.BLACK_LEFT;
            }
            
        }

        if (this.playerColor == PlayerColor.BLACK &&
            this.Index == 60 &&
            Board.Instance.ChessBoard[60] != null &&
            Board.Instance.ChessBoard[60].pieceName == PIECENAME.KING &&
            isFirstMove &&
            Board.Instance.ChessBoard[63] != null &&
            Board.Instance.ChessBoard[63].pieceName == PIECENAME.ROOK &&
            Board.Instance.ChessBoard[63].playerColor == PlayerColor.BLACK &&
            Board.Instance.ChessBoard[63].isFirstMove &&
            Board.Instance.ChessBoard[61] == null &&
            Board.Instance.ChessBoard[62] == null &&
            !Board.Instance.BlackChecked
            )
        {
            if (ValidMoves.Find(x => x == 61) == 61) {

                AddIndex(62);
                castling = CASTLINGTYPE.BLACK_RIGHT;
            }

        }

    }

}