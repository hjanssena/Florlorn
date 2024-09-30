using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LeaderBoardScreen : MonoBehaviour
{
    TMP_Text timeBoard;
    TMP_Text leaderBoard;

    void Start()
    {
        timeBoard = GameObject.Find("Content").GetComponent<TMP_Text>();
        timeBoard.text = "\n";

        TimeSpan total = new TimeSpan();
        int i = 1;
        foreach(TimeSpan time in TimeKeeping.timeList)
        {
            timeBoard.text += "Level "+ i + ": " + time.ToString("mm':'ss") + "\n";
            total += time;
            i++;
        }
        timeBoard.text += "TOTAL: " + total.ToString("mm':'ss");


        TimeKeeping.SaveTotalTime();
        TimeKeeping.totalTimes.Sort();

        i = 1;
        foreach (TimeSpan totalTime in TimeKeeping.totalTimes)
        {
            timeBoard.text += i + ":  "+ totalTime.ToString("mm':'ss") + "\n";
            i++;
        }
    }
}
