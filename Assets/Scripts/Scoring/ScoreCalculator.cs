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
            float weight = GetWeight(task, settings);

            domainScore.total++;
            domainScore.possible += weight;
            score.totalCount++;

            TaskResult result;
            if (taskManager.TryGetResult(task, out result))
            {
                float taskScore = GetTaskScore(task, result, settings);

                domainScore.completed++;
                domainScore.earned += weight * taskScore;
                score.completedCount++;
                score.completedTasks.Add(task);
                score.taskPercents[task] = taskScore * 100f;
            }
            else
            {
                score.missedTasks.Add(task);
                if (task.priority == TaskPriority.Critical) score.missedCriticalTasks.Add(task);
            }
        }

        foreach (PreparednessDomain domain in Enum.GetValues(typeof(PreparednessDomain)))
        {
            if (scoresByDomain.ContainsKey(domain)) score.domainScores.Add(scoresByDomain[domain]);
        }

        score.percent = GetAveragePercent(score.domainScores);
        SetRating(score, settings);

        if (!score.IsPerfect)
        {
            FindWeakestDomains(score);
        }

        return score;
    }

    private static float GetWeight(TaskData task, GameSettings settings)
    {
        if (task.priority == TaskPriority.Critical) return settings.criticalWeight;
        return settings.importantWeight;
    }

    private static float GetTaskScore(TaskData task, TaskResult result, GameSettings settings)
    {
        if (task.scoreMethod == ScoreMethod.Done) return 1f;

        float retryLoss = settings.retryPenalty * Mathf.Max(0, result.attempts - 1);
        float taskScore;

        if (task.scoreMethod == ScoreMethod.Speed)
        {
            float timeUsed = settings.speedTimeLimit > 0f ? Mathf.Clamp01(result.secondsSpent / settings.speedTimeLimit) : 0f;
            taskScore = Mathf.Lerp(1f, settings.minSpeedScore, timeUsed) - retryLoss;
        }
        else if (task.scoreMethod == ScoreMethod.Knowledge)
        {
            taskScore = 1f - retryLoss;
        }
        else
        {
            taskScore = 1f - settings.mistakePenalty * result.mistakes - retryLoss;
        }

        return Mathf.Clamp(taskScore, settings.minTaskScore, 1f);
    }

    private static float GetAveragePercent(List<DomainScore> domainScores)
    {
        if (domainScores.Count == 0) return 0f;

        float total = 0f;
        foreach (DomainScore domainScore in domainScores)
        {
            total += domainScore.Percent;
        }
        return total / domainScores.Count;
    }

    private static void SetRating(PreparednessScore score, GameSettings settings)
    {
        if (score.percent >= settings.wellPreparedPercent)
        {
            score.rating = settings.wellPreparedRating;
        }
        else if (score.percent >= settings.partlyPreparedPercent)
        {
            score.rating = settings.partlyPreparedRating;
        }
        else
        {
            score.rating = settings.notPreparedRating;
        }

        if (score.missedCriticalTasks.Count > 0 && score.rating == settings.wellPreparedRating)
        {
            score.rating = settings.partlyPreparedRating;
            score.ratingCapped = true;
        }
    }

    private static void FindWeakestDomains(PreparednessScore score)
    {
        if (score.domainScores.Count == 0) return;

        float lowestPercent = float.MaxValue;
        foreach (DomainScore domainScore in score.domainScores)
        {
            if (domainScore.Percent < lowestPercent) lowestPercent = domainScore.Percent;
        }

        foreach (DomainScore domainScore in score.domainScores)
        {
            if (Mathf.Approximately(domainScore.Percent, lowestPercent)) score.weakestDomains.Add(domainScore.domain);
        }
    }
}
