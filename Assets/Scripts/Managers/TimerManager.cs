using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{

    public Text Timer;
    private double time = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time = time + Time.deltaTime;
        string timeText = System.TimeSpan.FromSeconds(time).ToString("mm':'ss");
        Timer.text = timeText;
    }

    public void ResetTimer()
    {
        time = 0;
    }

    public double TakeTime()
    {
        var r = time;
        ResetTimer();
        return r;
    }
}


