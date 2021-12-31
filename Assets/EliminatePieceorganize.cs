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
    int filledIndex = 0;

    public Vector3 GetLocalPosition(int index) 
    {

        return new Vector3(4.375f - (1.25f * (float)(index % 8)) , 1f, 0.5f - (2f * (int)(index / 8)));

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
