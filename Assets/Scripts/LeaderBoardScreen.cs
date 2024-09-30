using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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

        leaderBoard = GameObject.Find("Leaderboard").GetComponent<TMP_Text>();
        leaderBoard.text = "\n";

        i = 1;
        foreach (TimeSpan totalTime in TimeKeeping.totalTimes)
        {
            leaderBoard.text += i + ":  "+ totalTime.ToString("mm':'ss") + "\n";
            i++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            try
            {
                SceneManager.MoveGameObjectToScene(GameObject.Find("MusicPlayer"), SceneManager.GetActiveScene());
            }
            catch
            {

            }
            SceneManager.LoadScene("MainMenu");
        }
    }
}
