using System.Collections.Generic;

public class PreparednessScore
{
    public List<DomainScore> domainScores = new List<DomainScore>();
    public List<PreparednessDomain> weakestDomains = new List<PreparednessDomain>();
    public List<TaskData> completedTasks = new List<TaskData>();
    public List<TaskData> missedTasks = new List<TaskData>();
    public List<TaskData> missedCriticalTasks = new List<TaskData>();
    public Dictionary<TaskData, float> taskPercents = new Dictionary<TaskData, float>();
    public int completedCount;
    public int totalCount;
    public float percent;
    public string rating;
    public bool ratingCapped;
    public float evacuationSeconds;

    public bool IsPerfect
    {
        get { return totalCount > 0 && completedCount == totalCount; }
    }
}
