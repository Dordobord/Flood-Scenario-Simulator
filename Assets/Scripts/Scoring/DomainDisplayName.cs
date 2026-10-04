using System;

[Serializable]
public class DomainDisplayName
{
    public PreparednessDomain domain;
    public string displayName;

    public DomainDisplayName()
    {
    }

    public DomainDisplayName(PreparednessDomain domain, string displayName)
    {
        this.domain = domain;
        this.displayName = displayName;
    }
}
