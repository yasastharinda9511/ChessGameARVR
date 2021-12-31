using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject RookButton;
    [SerializeField]
    GameObject BishopButton;
    [SerializeField]
    GameObject KnightButton;
    [SerializeField]
    GameObject QueenButton;
    [SerializeField]
    GameObject BoardStatusBanner;

    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null) {

            Instance = this;
        }
    }
    void Start()
    {

        this.InteractOff();
    }

    public void InteractOn()
    {

        RookButton.GetComponent<Button>().interactable =true;
        BishopButton.GetComponent<Button>().interactable =true;
        KnightButton.GetComponent<Button>().interactable =true;
        QueenButton.GetComponent<Button>().interactable =true;

    
    }

    public void InteractOff() 
    {
        RookButton.GetComponent<Button>().interactable = false;
        BishopButton.GetComponent<Button>().interactable = false;
        KnightButton.GetComponent<Button>().interactable = false;
        QueenButton.GetComponent<Button>().interactable = false;

    }

    public void UpdateBoardStatusBanner(string status) 
    {

        if (GameManager.Instance.BlackChecked && GameManager.Instance.status != BOARDSTATUS.BLACK_CHECKMATE) BoardStatusBanner.GetComponent<TextMeshPro>().text = "BLACK CHECK";
        else if (GameManager.Instance.WhiteChecked && GameManager.Instance.status != BOARDSTATUS.WHITE_CHECKMATE) BoardStatusBanner.GetComponent<TextMeshPro>().text = "WHITE CHECk";
        else BoardStatusBanner.GetComponent<TextMeshPro>().text = status;

    }

}
