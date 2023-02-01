using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnModel : PieceModel
{
    public override IEnumerator MovePattern(int index)
    {
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
        yield return null;
    }

    public override void SelectChangeMaterial()
    {
        throw new System.NotImplementedException();
    }
}
