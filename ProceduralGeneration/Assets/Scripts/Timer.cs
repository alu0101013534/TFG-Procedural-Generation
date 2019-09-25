using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI timerText;
    public float startTime=80f;
    public GameObject winCanvas;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t =  startTime - Time.timeSinceLevelLoad;
        string min = ((int)t / 60).ToString();
        string sec = (t % 60).ToString("f2");
        timerText.text="SURVIVE: "+min+":" + sec;
        if (t <= 0)
            Win();

    }

    void Win()
    {
        winCanvas.SetActive(true);
        GameObject.FindObjectOfType<GameManager>().Win();

    }

}
