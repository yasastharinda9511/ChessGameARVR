using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class AndroindPlayer : PlayerBase
{
    // Start is called before the first frame update
    [SerializeField]
    Camera camera;
    bool clicked;
    RaycastHit hit;
    void Start()
    {
        clicked = false;
    }

    // Update is called once per frame
    void Update()
    {
        clicked = (Input.touchCount > 0) ? true : false;
        if (clicked)
        {
            Ray ray = camera.ScreenPointToRay(Input.GetTouch(0).position);
            if(Physics.Raycast(ray, out hit)) base.ChangeState(GameManagerAI.Instance, clicked, hit.transform.gameObject);

        }

    }
}
