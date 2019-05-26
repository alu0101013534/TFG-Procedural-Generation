using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePool : MonoBehaviour
{

    public static int numItems = 1000;
    public GameObject prefab;
    public GameObject Pineprefab;
    static GameObject[] trees;
    static GameObject[] pines;

    // Start is called before the first frame update
    void Start()
    {
        trees = new GameObject[numItems];
        pines = new GameObject[numItems];
        for (int i=0; i< numItems; i++) {
            trees[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            trees[i].SetActive(false);
            pines[i] = Instantiate(Pineprefab, Vector3.zero, Quaternion.identity);
            pines[i].SetActive(false);

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public static GameObject getTree() {

        for (int i = 0; i < numItems; i++)
        {
           if(!trees[i].activeSelf)
            { 
                return trees[i];
            }

        }
        return null;
    }

    public static GameObject getPine()
    {

        for (int i = 0; i < numItems; i++)
        {
            if (!pines[i].activeSelf)
            {
                return pines[i];
            }

        }
        return null;
    }
}
