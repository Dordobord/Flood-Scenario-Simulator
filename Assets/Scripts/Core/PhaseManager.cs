using UnityEngine;

namespace StormWaits
{
    public enum GamePhase
    {
        Preparation,
        Transitioning,
        Flood
    }

    public class PhaseManager : MonoBehaviour
    {
        public static PhaseManager Instance { get; private set; }

        public static bool IsTransitionActive { get; private set; }

        [SerializeField] private Transform floodSpawnPoint;

        private float timeRemaining;

        public GamePhase CurrentPhase { get; private set; }

        public float TimeRemaining
        {
            get { return timeRemaining; }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            IsTransitionActive = false;
        }

        private void Start()
        {
            timeRemaining = GameSettings.Current.preparationSeconds;
            CurrentPhase = GamePhase.Preparation;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                IsTransitionActive = false;
            }
        }

        private void Update()
        {
            if (CurrentPhase != GamePhase.Preparation) return;

            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                BeginTransition();
            }
        }

        private void BeginTransition()
        {
            CurrentPhase = GamePhase.Transitioning;
            IsTransitionActive = true;

            if (TransitionUI.Instance != null)
            {
                TransitionUI.Instance.Play(TeleportPlayer, FinishTransition);
            }
            else
            {
                TeleportPlayer();
                FinishTransition();
            }
        }

        private void TeleportPlayer()
        {
            if (floodSpawnPoint == null)
            {
                Debug.LogWarning("PhaseManager: no Flood Spawn Point assigned. The player will stay where they are.");
                return;
            }

            FirstPersonController player = FindFirstObjectByType<FirstPersonController>();
            if (player == null) return;

            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.SetPositionAndRotation(floodSpawnPoint.position, floodSpawnPoint.rotation);

            if (controller != null) controller.enabled = true;
        }

        private void FinishTransition()
        {
            CurrentPhase = GamePhase.Flood;
            IsTransitionActive = false;
            GameEvents.RaiseMessage(GameSettings.Current.floodStartedMessage);
        }
    }
}
