using System.Collections;
using System.Collections.Generic;
using System;

public static class TimeKeeping
{
    public static List<TimeSpan> timeList = new List<TimeSpan>();
    public static List<TimeSpan> totalTimes = new List<TimeSpan>();

    public static void SaveTotalTime()
    {
        TimeSpan total = new TimeSpan();
        foreach (TimeSpan time in timeList) 
        {
            total += time;
        }

        totalTimes.Add(total);
    }
}
