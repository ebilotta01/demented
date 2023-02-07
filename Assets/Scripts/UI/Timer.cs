using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timer;
    public float rawseconds;
    public int seconds;
    public int minutes;
    void Start()
    {
        rawseconds = 0;
        seconds = 0;
        minutes = 0;
    }

    void Update()
    {
        rawseconds += Time.deltaTime;
        seconds = (int)rawseconds;
        if(seconds >= 60) {
            rawseconds = 0;
            minutes++;
        }
        timer.text = FormatMMSS(minutes) + ":" + FormatMMSS(seconds);
    }
    string FormatMMSS(int value) {
        string ones = (value % 10).ToString();
        string tens = (value / 10).ToString();
        return tens + ones;
    }
}
