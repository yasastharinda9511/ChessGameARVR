using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<int> CalculateValidMoves()
    {
        ValidMoves.Clear();
        checkLShape();
        return ValidMoves;
    }

    public override IEnumerator Move(int index)
    {
        bool[] checkPath = CheckObstacle(index);
        Vector3 forward;
        float cof;
        int index2;
        

        if (checkPath[0])
        {

            index2 = prevIndex + 8 * RowCountFromCurrentPosition(index);
            forward = CalculateLocalPosition(index2) - this.transform.localPosition;

            Debug.Log("#@$ path1");
            while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index2)) > 0.01f && 
                   Vector3.Dot(CalculateLocalPosition(index2) - this.transform.localPosition , forward) >= 0)
            {
                cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));
                cof = (cof > 1.5f) ? 1.5f : cof;
                this.transform.Translate(forward * Time.deltaTime * cof * speed);
                yield return null;

            }

            this.transform.localPosition = CalculateLocalPosition(index2);

            index2 = prevIndex + ColumnCountFromCurrentPosition(index) + 8 * RowCountFromCurrentPosition(index);

            forward = CalculateLocalPosition(index2) - this.transform.localPosition;

            while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index2)) > 0.01f  &&
                Vector3.Dot(CalculateLocalPosition(index2) - this.transform.localPosition, forward) >= 0)
            {
                
                cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));
                cof = (cof >1.5f) ? 1.5f : cof;
                this.transform.Translate(forward * Time.deltaTime * cof * speed);
                yield return null;


            }

            this.transform.localPosition = CalculateLocalPosition(index2);
        }
        else if (checkPath[1]) // should be modified
        {
            Debug.Log("#@$ path2");
            index2 = prevIndex + ColumnCountFromCurrentPosition(index);
            forward = CalculateLocalPosition(index2) - this.transform.localPosition;

            while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index2)) > 0.01f && 
                Vector3.Dot(CalculateLocalPosition(index2) - this.transform.localPosition, forward) >= 0)
            {

                cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));
                cof = (cof > 1.5f) ? 1.5f : cof;
                this.transform.Translate(forward * Time.deltaTime * cof * speed);
                yield return null;


            }

            this.transform.localPosition = CalculateLocalPosition(index2);

            index2 = prevIndex + ColumnCountFromCurrentPosition(index) + 8 * RowCountFromCurrentPosition(index);

            forward = CalculateLocalPosition(index2) - this.transform.localPosition;

            while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index2)) > 0.01f &&
                Vector3.Dot(CalculateLocalPosition(index2) - this.transform.localPosition, forward) >= 0)
            {

                cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));
                cof = (cof > 1.5f) ? 1.5f : cof;
                this.transform.Translate(forward * Time.deltaTime * cof * speed);
                yield return null;


            }

            this.transform.localPosition = CalculateLocalPosition(index2);
        }
        else {

            Vector3 distanceVector = this.transform.localPosition;

            forward = Vector3.Normalize(CalculateLocalPosition(index) - CalculateLocalPosition(prevIndex));

            float magnitude = Vector3.Magnitude(forward);

            float altitude;

            while (Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index)) > 0.01f  && 
                   Vector3.Dot(CalculateLocalPosition(index) - distanceVector, forward) >= 0) {

                cof = Vector3.Magnitude(this.transform.localPosition - CalculateLocalPosition(index));

                distanceVector = this.transform.localPosition + forward * Time.deltaTime * 5f * cof;

                distanceVector = new Vector3(distanceVector.x , 0 , distanceVector.z);

                altitude = Mathf.Abs(Vector3.Dot(distanceVector - CalculateLocalPosition(prevIndex), distanceVector - CalculateLocalPosition(index)));

                this.transform.localPosition = new Vector3(distanceVector.x, (altitude), distanceVector.z);

                yield return null;

            }

            this.transform.localPosition = CalculateLocalPosition(index);
        }

        MoveCoroute = false;
        yield return null;
    }

    public override void SelectChangeMaterial()
    {
        throw new System.NotImplementedException();
    }

    void checkLShape()
    {

        if (CheckInSameRow(Index + 1, Index) && Index + 17 <= 63)
        {

            if (!CheckSameColorPiece(Index + 17)) AddIndex(Index + 17);//ValidMoves.Add(Index + 17);

        }

        if (CheckInSameRow(Index - 1, Index) && Index + 15 <= 63)
        {

            if (!CheckSameColorPiece(Index + 15)) AddIndex(Index + 15);//ValidMoves.Add(Index + 15);

        }

        if (CheckInSameRow(Index + 1, Index) && Index - 15 >= 0)
        {

            if (!CheckSameColorPiece(Index - 15)) AddIndex(Index - 15);//ValidMoves.Add(Index - 15);

        }

        if (CheckInSameRow(Index - 1, Index) && Index - 17 >= 0)
        {

            if (!CheckSameColorPiece(Index - 17)) AddIndex(Index - 17);//ValidMoves.Add(Index - 17);
        }

        if (CheckInSameRow(Index + 2, Index) && Index + 10 <= 63)
        {

            if (!CheckSameColorPiece(Index + 10)) AddIndex(Index + 10);//ValidMoves.Add(Index + 10);
                    
        }

        if (CheckInSameRow(Index - 2, Index) && Index + 6 <= 63 && !CheckInSameRow(Index + 6, Index))
        {

            if (!CheckSameColorPiece(Index + 6)) AddIndex(Index + 6); //ValidMoves.Add(Index + 6);

        }

        if (CheckInSameRow(Index + 2, Index) && Index - 6 >= 0 && !CheckInSameRow(Index - 6, Index))
        {

            if (!CheckSameColorPiece(Index - 6)) AddIndex(Index - 6);//ValidMoves.Add(Index - 6);
        }

        if (CheckInSameRow(Index - 2, Index) && Index - 10 >= 0)
        {

            if (!CheckSameColorPiece(Index - 10)) AddIndex(Index - 10);//ValidMoves.Add(Index - 10);
        }

    }

    bool[] CheckObstacle(int index) {

        int rows = RowCountFromCurrentPosition(index);
        int columns = ColumnCountFromCurrentPosition(index);

        int i = (rows > 0) ? -1 : +1;
        int r = (columns > 0) ? -1 : +1;

        bool PathOne = true;
        bool Pathtwo = true;

        while (columns != 0 || rows != 0) {

            if (Board.Instance.ChessBoard[prevIndex + rows * 8 + columns] != null && (prevIndex + rows * 8 + columns != index)) {

                PathOne = false;
                Debug.Log("Path OneIndex false : " + (Index + rows * 8 + columns));
                break;
            } 

            if (columns != 0)
            {
                columns += r;
            }
            else
            {
                rows += i;
            }

        } 
        
        rows = RowCountFromCurrentPosition(index);
        columns = ColumnCountFromCurrentPosition(index);

        while (columns != 0 || rows != 0) {

            if (Board.Instance.ChessBoard[prevIndex + rows * 8 + columns] != null && (prevIndex + rows * 8 + columns != index)) {

                Pathtwo = false;
                Debug.Log("Path Two Index false : " + (prevIndex + rows * 8 + columns));
                break;

            }
            

            if (rows != 0)
            {
                rows += i;
            }
            else
            {
                columns += r;
            }
        }

        Debug.Log("PrevPath :" + PathOne + "Pathtwo :" + Pathtwo );
        return new bool[2] { PathOne , Pathtwo} ;

    }

    protected int ColumnCountFromCurrentPosition(int index1)
    {

        return ((int)(index1 % 8) - (int)(prevIndex % 8));

    }
    protected int RowCountFromCurrentPosition(int index1)
    {

        return ((int)(index1 / 8) - (int)(prevIndex / 8));

    }

}
