using UnityEngine;

namespace StormWaits
{
    public class Pickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private TaskData task;
        [SerializeField] private string promptOverride;
        [SerializeField] private bool hideWhenCompleted = true;

        public string Prompt
        {
            get
            {
                if (!string.IsNullOrEmpty(promptOverride)) return promptOverride;
                if (task != null) return task.title;
                return "Interact";
            }
        }

        public bool CanInteract
        {
            get
            {
                if (task == null) return false;
                if (TaskManager.Instance != null && TaskManager.Instance.IsCompleted(task)) return false;
                return true;
            }
        }

        public void Interact()
        {
            if (!CanInteract) return;

            InventoryManager inventory = InventoryManager.Instance;
            if (task.rewardItem != null && inventory != null && !inventory.CanAdd(task.rewardItem))
            {
                GameEvents.RaiseMessage(GameSettings.Current.inventoryFullMessage);
                return;
            }

            if (MinigameManager.Instance == null)
            {
                Debug.LogWarning("Pickup: no minigame window in the scene. Run Storm Waits > 3. Build UI.");
                return;
            }

            MinigameManager.Instance.Begin(task, OnMinigameFinished);
        }

        private void OnMinigameFinished(bool success)
        {
            if (!success) return;

            GameSettings settings = GameSettings.Current;
            InventoryManager inventory = InventoryManager.Instance;

            if (task.rewardItem != null && inventory != null && inventory.TryAdd(task.rewardItem))
            {
                GameEvents.RaiseMessage(settings.collectedMessage + task.rewardItem.displayName);
            }
            else
            {
                GameEvents.RaiseMessage(settings.taskCompleteMessage + task.title);
            }

            if (hideWhenCompleted) gameObject.SetActive(false);
        }
    }
}
