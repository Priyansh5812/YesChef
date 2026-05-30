using UnityEngine;

[System.Serializable]
public struct TimeSpan
{   
    [Range(0 , 59)] public int Mins;
    [Range(0 , 59)] public int Secs;

    public TimeSpan(int mins , int secs)
    {
        this.Mins = mins;
        this.Secs = secs;
    }

    public static int TimeSpanToSecs(TimeSpan span)
    {
        return span.Mins * 60 + span.Secs;
    }

    public static TimeSpan SecsToTimeSpan(int secs)
    {   
        return new TimeSpan(secs / 60 , secs % 60);
    }
}