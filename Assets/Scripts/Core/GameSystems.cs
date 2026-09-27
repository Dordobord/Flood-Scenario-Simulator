using UnityEngine;

namespace StormWaits
{
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(InventoryManager))]
    [RequireComponent(typeof(TaskManager))]
    [RequireComponent(typeof(PhaseManager))]
    public class GameSystems : MonoBehaviour
    {
        [SerializeField] private GameSettings settings;

        private void Awake()
        {
            if (settings != null)
            {
                GameSettings.Current = settings;
            }
        }
    }
}
