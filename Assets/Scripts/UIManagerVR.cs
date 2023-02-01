using UnityEngine;

public class UIManagerVR : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject whitePawnToQueenDeck;
    [SerializeField]
    GameObject blackPawnToQueenDeck;

    public static UIManagerVR Instance;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        whitePawnToQueenDeck.SetActive(false);
        blackPawnToQueenDeck.SetActive(false);
    }
    public void WhiteDeckOn() { 
    
        whitePawnToQueenDeck.SetActive(true);

    }

    public void BlackDeckOn()
    {
        blackPawnToQueenDeck.SetActive(true);
    }

    public void BlackDeckOff()
    {
        Debug.Log("Set false set");
        blackPawnToQueenDeck.SetActive(false);
    } 
    
    public void WhiteDeckOff()
    {
        whitePawnToQueenDeck.SetActive(false);
    }
}
