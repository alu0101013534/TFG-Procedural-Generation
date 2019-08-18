using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    private GameObject player;
    Rigidbody rb;



    // Start is called before the first frame update
    void Start()
    {
        player=GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform);


        transform.position += transform.forward * speed * Time.deltaTime;



    }
}
