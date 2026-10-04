using System;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreCalculator
{
    public static PreparednessScore Calculate(List<TaskData> tasks, TaskManager taskManager, float evacuationSeconds)
    {
        PreparednessScore score = new PreparednessScore();
        score.evacuationSeconds = evacuationSeconds;

        Dictionary<PreparednessDomain, DomainScore> scoresByDomain = new Dictionary<PreparednessDomain, DomainScore>();

        foreach (TaskData task in tasks)
        {
            if (task == null) continue;

            if (!Enum.IsDefined(typeof(PreparednessDomain), task.domain))
            {
                Debug.LogWarning("ScoreCalculator: the task '" + task.title + "' has no domain, so it is not scored. Pick a Domain on the task.");
                continue;
            }

            if (!scoresByDomain.ContainsKey(task.domain))
            {
                scoresByDomain[task.domain] = new DomainScore(task.domain);
            }

            DomainScore domainScore = scoresByDomain[task.domain];
            domainScore.total++;
            score.totalCount++;

            if (taskManager.IsCompleted(task))
            {
                domainScore.completed++;
                score.completedCount++;
                score.completedTasks.Add(task);
            }
            else
            {
                score.missedTasks.Add(task);
            }
        }

        foreach (PreparednessDomain domain in Enum.GetValues(typeof(PreparednessDomain)))
        {
            if (scoresByDomain.ContainsKey(domain)) score.domainScores.Add(scoresByDomain[domain]);
        }

        if (!score.IsPerfect)
        {
            FindWeakestDomains(score);
        }

        return score;
    }

    private static void FindWeakestDomains(PreparednessScore score)
    {
        if (score.domainScores.Count == 0) return;

        float lowestRatio = float.MaxValue;
        foreach (DomainScore domainScore in score.domainScores)
        {
            if (domainScore.Ratio < lowestRatio) lowestRatio = domainScore.Ratio;
        }

        foreach (DomainScore domainScore in score.domainScores)
        {
            if (Mathf.Approximately(domainScore.Ratio, lowestRatio)) score.weakestDomains.Add(domainScore.domain);
        }
    }
}
