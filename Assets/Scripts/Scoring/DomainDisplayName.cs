using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DomainDisplayName
{
    public PreparednessDomain domain;
    public string displayName;
    public string shortName;
    public Sprite icon;
    [TextArea] public string hint;
    [TextArea] public string greatMessage;
    [TextArea] public string goodMessage;
    [TextArea] public string weakMessage;
    public List<TaskData> mustDoTasks = new List<TaskData>();

    public DomainDisplayName()
    {
    }

    public DomainDisplayName(PreparednessDomain domain, string displayName, string hint)
    {
        this.domain = domain;
        this.displayName = displayName;
        this.hint = hint;
    }
}
