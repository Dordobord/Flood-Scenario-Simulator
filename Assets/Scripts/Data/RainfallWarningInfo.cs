using System;
using UnityEngine;

[Serializable]
public class RainfallWarningInfo
{
    public RainfallWarningLevel level;
    public string label;
    public Color color = Color.white;
    public string action;

    public RainfallWarningInfo()
    {
    }

    public RainfallWarningInfo(RainfallWarningLevel level, string label, Color color, string action)
    {
        this.level = level;
        this.label = label;
        this.color = color;
        this.action = action;
    }
}
