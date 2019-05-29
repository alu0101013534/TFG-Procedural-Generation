using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePool : MonoBehaviour
{

    public static int numItems = 1000;
    public List<GameObject> prefab;
    public List<GameObject> Pineprefab;
    static GameObject[] trees;
    static GameObject[] pines;

    // Start is called before the first frame update
    void Start()
    {
       // trees = new GameObject[numItems];
        //pines = new GameObject[numItems];
       /* for (int i=0; i< numItems; i++) {
            if (Random.Range(0, 10) > 4)
            {
                trees[i] = Instantiate(prefab[Random.Range(0, prefab.Count)], Vector3.zero, Quaternion.identity);
                pines[i] = Instantiate(Pineprefab[Random.Range(0, Pineprefab.Count)], Vector3.zero, Quaternion.identity);
            }
            else
            {
                trees[i] = Instantiate(prefab[0], Vector3.zero, Quaternion.identity);
                pines[i] = Instantiate(Pineprefab[0], Vector3.zero, Quaternion.identity);
            }

            trees[i].SetActive(false);
            pines[i].SetActive(false);

        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public GameObject getTree() {

        /* for (int i = 0; i < numItems; i++)
         {
            if(!trees[i].activeSelf)
             { 
                 return trees[i];
             }

         }
         return null;*/
        GameObject tree;
        if (Random.Range(0, 10) > 4)
        {
            tree= Instantiate(prefab[Random.Range(0, prefab.Count)], Vector3.zero, Quaternion.identity);
           
        }
        else
        {
            tree = Instantiate(prefab[0], Vector3.zero, Quaternion.identity);
             
        }
        return tree;
    }

    public  GameObject getPine()
    {

        GameObject tree;
        if (Random.Range(0, 10) > 4)
        {
            tree = Instantiate(Pineprefab[Random.Range(0, Pineprefab.Count)], Vector3.zero, Quaternion.identity);

        }
        else
        {
            tree = Instantiate(Pineprefab[0], Vector3.zero, Quaternion.identity);

        }
        return tree;
    }
}
