using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{

    public static int numItems = 1000;
    public List<GameObject> prefab;
    public List<GameObject> Pineprefab;
    public List<GameObject> BeachEnemies;
    public List<GameObject> WaterEnemies;
    public List<GameObject> ForestEnemies;
    public List<GameObject> MountainEnemies;
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

     public GameObject getGrassPrefabs() {

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

    public  GameObject getWoodsPrefabs()
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

    public GameObject getBeachEnemies()
    {

        GameObject enemy;
        if (BeachEnemies.Count != 0)
        {
            if (Random.Range(0, 10) > 4)
            {
                enemy = Instantiate(BeachEnemies[Random.Range(0, BeachEnemies.Count)], Vector3.zero, Quaternion.identity);

            }
            else
            {
                enemy = Instantiate(BeachEnemies[0], Vector3.zero, Quaternion.identity);

            }
            return enemy;
        }
        else return null;
    }
    public GameObject getWaterEnemies()
    {

        GameObject enemy;
        if (Random.Range(0, 10) > 4)
        {
            enemy = Instantiate(WaterEnemies[Random.Range(0, WaterEnemies.Count)], Vector3.zero, Quaternion.identity);

        }
        else
        {
            enemy = Instantiate(WaterEnemies[0], Vector3.zero, Quaternion.identity);

        }
        return enemy;
    }

    public GameObject getForestEnemies()
    {

        GameObject enemy;
        if (Random.Range(0, 10) > 4)
        {
            enemy = Instantiate(ForestEnemies[Random.Range(0, ForestEnemies.Count)], Vector3.zero, Quaternion.identity);

        }
        else
        {
            enemy = Instantiate(ForestEnemies[0], Vector3.zero, Quaternion.identity);

        }
        return enemy;
    }

    public GameObject getMountainEnemies()
    {

        GameObject enemy;
        if (Random.Range(0, 10) > 4)
        {
            enemy = Instantiate(MountainEnemies[Random.Range(0, MountainEnemies.Count)], Vector3.zero, Quaternion.identity);

        }
        else
        {
            enemy = Instantiate(MountainEnemies[0], Vector3.zero, Quaternion.identity);

        }
        return enemy;
    }

}

