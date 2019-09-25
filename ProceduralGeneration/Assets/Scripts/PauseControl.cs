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
        Time.timeScale = 1;
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm== null || !gm.isGameOver)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Pause();
            }


            if (Input.GetKey(KeyCode.Q))
            {
                Quit();
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Application.LoadLevel(0);
       
    }
}
