using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int startHealth;
    private int currentHealth;
    public TextMeshProUGUI healthText;

    public float invenciblePeriod;
    public bool takingDamage;
    public float timer;

    public bool isGameOver;

    public GameObject gameOverCanvas;

    public AudioSource audiosource;
    public AudioClip hitsound;



    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startHealth;
        healthText.text = "HEALTH: "+startHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (takingDamage)
        {
            timer += Time.deltaTime;

            if (timer > invenciblePeriod)
            {
                timer = 0f;
                takingDamage = false;


            }
        }
        if (isGameOver) { 

            if (Input.GetKey(KeyCode.Q))
            {
                GameObject.FindObjectOfType<PauseControl>().Quit();
            }
        }
    }


    public void DamagePlayer()
    {
        if (!takingDamage)
        { 
            takingDamage = true;
            currentHealth--;
            audiosource.PlayOneShot(hitsound);
            healthText.text = "HEALTH: " + currentHealth.ToString();
            if (currentHealth <= 0)
                GameOver();
        }

    }


    public void GameOver() {

        isGameOver = true;
        Time.timeScale = 0;
        gameOverCanvas.SetActive(true);
    }

    public void Win() {

        isGameOver = true;
        Time.timeScale = 0;
    }

}
