using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class HumanMobilePlayer : PlayerBase
{
    // Start is called before the first frame update

    [SerializeField]
    Camera camera;

    RaycastHit hit;
    bool clicked;
    void Start()
    {
        hit = new RaycastHit();
        clicked= false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        //Debug.Log(Camera.main.name);
        clicked= (Input.GetMouseButtonDown(0)) ? true : false;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            base.ChangeState(GameManagerAI.Instance, clicked , hit.transform.gameObject);
        }
    }
}
