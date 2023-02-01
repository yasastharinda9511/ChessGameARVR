using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceModel : MonoBehaviour
{
    public Piece Piece { get; set; }

    public float speed = 0.05f;
    public abstract IEnumerator MovePattern(int index);
    public abstract void SelectChangeMaterial();
    public void Move(int index)
    {

        StartCoroutine(MovePattern(index));
    }

    protected Vector3 CalculateLocalPosition(int index)
    {

        return new Vector3(5.25f, 0, -5.25f) + new Vector3((int)(index / 8) * -1.5f, 0, (int)(index % 8) * 1.5f);

    }

    public void ShowValidMoves()
    {
        Piece.CalculateValidMoves();
        if (Piece.ValidMoves == null) return;

        foreach (var i in Piece.ValidMoves)
        {

            GameObject validBox = ObjectPool.instance.GetPooledObject();

            validBox.transform.localPosition = Board.Instance.CalculateLocalPosition(i.Destination);
            validBox.GetComponent<ValidBox>().ValidIndex = i.Destination;

        }

    }

    public void ChangePosition(int index) 
    {
        Piece.ChangePosition(index);
        Move(index);
    }

    public void SetLocalPosition(int index) 
    {
        transform.localPosition = CalculateLocalPosition(index);
    }

    public void Eleminate()
    {
        GameObject EleminationStage = (Piece.playerColor == PlayerColor.WHITE) ? GameObject.Find("WhiteEliminateStage") : GameObject.Find("BlackEliminateStage");
        EleminationStage.GetComponent<EliminatePieceorganize>().PushEliminatePiece(this.transform.gameObject);

    }

}
