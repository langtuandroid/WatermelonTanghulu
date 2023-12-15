using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TimeHelper
{
    public static void SetTime(TimeType time)
    {
        Time.timeScale = (int)time;
    }
}
