using UnityEngine;

[System.Serializable]
// simple time helper used by the game timer
public struct TimeSpan
{   
    [Range(0 , 59)] public int Mins;
    [Range(0 , 59)] public int Secs;

    public TimeSpan(int mins , int secs)
    {
        // stores the provided minute and second values
        this.Mins = mins;
        this.Secs = secs;
    }

    public static int TimeSpanToSecs(TimeSpan span)
    {
        // turns a time span into raw seconds
        return span.Mins * 60 + span.Secs;
    }

    public static TimeSpan SecsToTimeSpan(int secs)
    {   
        // turns raw seconds back into a time span
        return new TimeSpan(secs / 60 , secs % 60);
    }
}
