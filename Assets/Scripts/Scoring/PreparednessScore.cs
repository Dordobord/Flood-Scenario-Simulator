using System.Collections.Generic;

public class PreparednessScore
{
    public List<DomainScore> domainScores = new List<DomainScore>();
    public List<PreparednessDomain> weakestDomains = new List<PreparednessDomain>();
    public List<TaskData> completedTasks = new List<TaskData>();
    public List<TaskData> missedTasks = new List<TaskData>();
    public int completedCount;
    public int totalCount;
    public float evacuationSeconds;

    public float Percentage
    {
        get { return totalCount > 0 ? completedCount * 100f / totalCount : 0f; }
    }

    public bool IsPerfect
    {
        get { return totalCount > 0 && completedCount == totalCount; }
    }
}
