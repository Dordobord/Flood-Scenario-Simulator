using System;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreCalculator
{
    public static PreparednessScore Calculate(List<TaskData> tasks, TaskManager taskManager, float evacuationSeconds)
    {
        GameSettings settings = GameSettings.Current;
        PreparednessScore score = new PreparednessScore();
        score.evacuationSeconds = evacuationSeconds;

        Dictionary<PreparednessDomain, DomainScore> domainScores = new Dictionary<PreparednessDomain, DomainScore>();

        foreach (TaskData task in tasks)
        {
            if (task == null) continue;

            if (!domainScores.ContainsKey(task.domain))
            {
                domainScores[task.domain] = new DomainScore(task.domain, settings.domainMaxPoints);
            }

            float taskPercent = GetTaskPercent(task, taskManager, settings);
            float weight = GetTaskWeight(task, settings);

            DomainScore domainScore = domainScores[task.domain];
            domainScore.totalWeight += weight;
            domainScore.earnedWeight += weight * taskPercent;

            score.taskPercents[task] = taskPercent;
        }

        foreach (PreparednessDomain domain in Enum.GetValues(typeof(PreparednessDomain)))
        {
            if (!domainScores.ContainsKey(domain)) continue;

            DomainScore domainScore = domainScores[domain];
            score.domainScores.Add(domainScore);
            score.totalPoints += domainScore.Points;
            score.maxPoints += domainScore.maxPoints;
        }

        FindStrongestAndWeakestDomains(score);
        return score;
    }

    private static float GetTaskPercent(TaskData task, TaskManager taskManager, GameSettings settings)
    {
        TaskResult result;
        if (!taskManager.TryGetResult(task, out result)) return 0f;

        if (task.scoreMethod == ScoreMethod.Done) return 1f;

        int retries = Mathf.Max(0, result.attempts - 1);
        float percent = 1f - retries * settings.retryPenalty;

        if (task.scoreMethod == ScoreMethod.Procedure)
        {
            percent -= result.mistakes * settings.mistakePenalty;
        }

        return Mathf.Clamp01(percent);
    }

    private static float GetTaskWeight(TaskData task, GameSettings settings)
    {
        if (task.priority == TaskPriority.Critical) return settings.criticalWeight;
        return settings.importantWeight;
    }

    private static void FindStrongestAndWeakestDomains(PreparednessScore score)
    {
        float highestPoints = float.MinValue;
        float lowestPoints = float.MaxValue;

        foreach (DomainScore domainScore in score.domainScores)
        {
            if (domainScore.Points > highestPoints) highestPoints = domainScore.Points;
            if (domainScore.Points < lowestPoints) lowestPoints = domainScore.Points;
        }

        bool allTied = Mathf.Approximately(highestPoints, lowestPoints);

        foreach (DomainScore domainScore in score.domainScores)
        {
            bool isHighest = Mathf.Approximately(domainScore.Points, highestPoints);
            bool isLowest = Mathf.Approximately(domainScore.Points, lowestPoints);
            bool isPerfect = Mathf.Approximately(domainScore.Points, domainScore.maxPoints);

            if (isHighest && !allTied) score.strongestDomains.Add(domainScore.domain);
            if (isLowest && !isPerfect) score.weakestDomains.Add(domainScore.domain);
        }
    }
}
