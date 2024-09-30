using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    float timerStart;
    TMP_Text tmp;
    TimeSpan time;

    void Start()
    {
        timerStart = Time.time;
        tmp = GetComponent<TMP_Text>();
    }

    void Update()
    {
        time = TimeSpan.FromSeconds(Time.time - timerStart);
        tmp.text = time.ToString("mm':'ss");
    }

    private void OnDestroy()
    {
        TimeKeeping.timeList.Add(time);
    }
}
