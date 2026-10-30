using System.Collections.Generic;

public class PreparednessScore
{
    public List<DomainScore> domainScores = new List<DomainScore>();
    public List<PreparednessDomain> strongestDomains = new List<PreparednessDomain>();
    public List<PreparednessDomain> weakestDomains = new List<PreparednessDomain>();
    public Dictionary<TaskData, float> taskPercents = new Dictionary<TaskData, float>();
    public float totalPoints;
    public float maxPoints;
    public float evacuationSeconds;
}
