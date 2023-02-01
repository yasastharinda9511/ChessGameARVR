using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugObject : EventTrigger
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnPointerClick(PointerEventData data)
    {
        Debug.Log("my Index is : " + this.GetComponent<Piece>().Index );
    }

    public override void OnPointerDown(PointerEventData data)
    {
        Debug.Log("OnPointerDown called.");
    }
}
