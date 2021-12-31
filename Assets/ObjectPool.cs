using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Start is called before the first frame update

    public static ObjectPool instance;

    private Stack<GameObject> inactivePooledValidObjects = new Stack<GameObject>();
    private Stack<GameObject> activePooledValidObjects = new Stack<GameObject>();

    [SerializeField]
    private GameObject validBox;
    [SerializeField]
    private GameObject parentBoard;

    int amoutToPool  = 28;

    private void Awake()
    {
        if (instance == null) {

            instance = this;
        
        }
    }
    void Start()
    {
        for (int i =0; i < amoutToPool; i++) {

            GameObject obj = Instantiate(validBox,parentBoard.transform);
            obj.SetActive(false);
            inactivePooledValidObjects.Push(obj);
        }
    }

    // Update is called once per frame
    public GameObject GetPooledObject()
    {

        GameObject obj = inactivePooledValidObjects.Pop();

        if (obj != null) {
            obj.SetActive(true);
            activePooledValidObjects.Push(obj);
            return obj;        
        }

        return null;
    
    }

    public void InactiveAllActive() 
    {

        while (activePooledValidObjects.Count != 0) {

            GameObject obj = activePooledValidObjects.Pop();
            obj.SetActive(false);
            inactivePooledValidObjects.Push(obj);

        }
    
    }
}
