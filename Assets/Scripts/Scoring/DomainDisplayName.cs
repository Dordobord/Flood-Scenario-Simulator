using System;
using UnityEngine;

[Serializable]
public class DomainDisplayName
{
    public PreparednessDomain domain;
    public string displayName;
    [TextArea] public string hint;

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
