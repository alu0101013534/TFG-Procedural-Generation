using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{

    public GameObject canvas;

    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.isGameOver)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Pause();
            }
        }
    }

    public void Pause() {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            canvas.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            canvas.SetActive(true);
        }
    }




    public void Quit()
    {
        Application.LoadLevel(0);
    }
}
