using System;

namespace StormWaits
{
    public static class GameEvents
    {
        public static event Action<TaskData> MinigameStarted;

        public static event Action<TaskData, bool> MinigameEnded;

        public static event Action<TaskResult> TaskCompleted;

        public static event Action<ItemData> ItemAdded;

        public static event Action<string> MessageRaised;

        public static void RaiseMinigameStarted(TaskData task)
        {
            if (MinigameStarted != null) MinigameStarted(task);
        }

        public static void RaiseMinigameEnded(TaskData task, bool success)
        {
            if (MinigameEnded != null) MinigameEnded(task, success);
        }

        public static void RaiseTaskCompleted(TaskResult result)
        {
            if (TaskCompleted != null) TaskCompleted(result);
        }

        public static void RaiseItemAdded(ItemData item)
        {
            if (ItemAdded != null) ItemAdded(item);
        }

        public static void RaiseMessage(string message)
        {
            if (MessageRaised != null) MessageRaised(message);
        }
    }
}
