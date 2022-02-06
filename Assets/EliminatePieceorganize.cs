using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminatePieceorganize : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject parentGameObject;
    [SerializeField] 
    PlayerColor color;
    [SerializeField]
    Vector3 initialPoint;
    int filledIndex = 0;
    [SerializeField]
    Vector3 cell;
    public Vector3 GetLocalPosition(int index) 
    {

        return initialPoint + new Vector3(index % 8 * cell.x , cell.y * (int)(index /8 ) , 0  );

    }

    public void PushEliminatePiece(GameObject eliminatePiece) {

        eliminatePiece.transform.parent = parentGameObject.transform;
        eliminatePiece.transform.localPosition = GetLocalPosition(filledIndex);
        //this.transform.TransformPoint(GetLocalPosition(filledIndex));

        Debug.Log("local coordinates are : " + eliminatePiece.transform.position);
        float cof; 

        Vector3 forward = parentGameObject.transform.TransformPoint(GetLocalPosition(filledIndex)) - this.transform.position;

        /*while (Vector3.Magnitude( this.transform.TransformPoint(GetLocalPosition(filledIndex)) - this.transform.position ) > 0.01f) 
        {

            cof = Vector3.Magnitude(this.transform.position - this.transform.TransformPoint(GetLocalPosition(filledIndex)));
            //Debug.Log("cof is : " + cof);
            eliminatePiece.transform.Translate(forward * Time.deltaTime * 10f * cof );
            yield return null;

        }*/
        this.filledIndex += 1 ;

        //yield return null;

        return;

    }

}
