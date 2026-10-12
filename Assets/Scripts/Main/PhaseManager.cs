using UnityEngine;
using UnityEngine.Serialization;

public enum GamePhase
{
    Preparation,
    Transitioning,
    Evacuation,
    Finished
}

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager Instance { get; private set; }

    public static bool IsTransitionActive { get; private set; }

    public static bool IsPlayerLocked
    {
        get { return IsTransitionActive || PauseMenuUI.IsPaused || (Instance != null && Instance.CurrentPhase == GamePhase.Finished); }
    }

    [FormerlySerializedAs("floodSpawnPoint")]
    [SerializeField] private Transform evacuationSpawnPoint;

    [SerializeField] private bool useTransitionScreen = false;

    private float timeRemaining;
    private float evacuationStartTime;

    public GamePhase CurrentPhase { get; private set; }

    public float TimeRemaining
    {
        get { return timeRemaining; }
    }

    public float EvacuationSeconds { get; private set; }

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
        timeRemaining = GetPreparationSeconds();
        CurrentPhase = GamePhase.Preparation;
    }

    private float GetPreparationSeconds()
    {
        if (TaskManager.Instance != null)
        {
            ScenarioData scenario = TaskManager.Instance.CurrentScenario;
            if (scenario != null && scenario.preparationSeconds > 0f) return scenario.preparationSeconds;
        }
        return GameSettings.Current.preparationSeconds;
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
        GameEvents.RaisePreparationEnded();

        if (useTransitionScreen && TransitionUI.Instance != null)
        {
            TransitionUI.Instance.Play(TeleportPlayer, StartEvacuation);
            return;
        }

        if (useTransitionScreen)
        {
            Debug.LogWarning("PhaseManager: Use Transition Screen is on, but there is no TransitionCanvas in the scene. Moving the player instantly.");
        }

        TeleportPlayer();
        StartEvacuation();
    }

    private void TeleportPlayer()
    {
        if (evacuationSpawnPoint == null)
        {
            Debug.LogWarning("PhaseManager: no Evacuation Spawn Point assigned. The player will stay where they are.");
            return;
        }

        FirstPersonController player = FindAnyObjectByType<FirstPersonController>();
        if (player == null) return;

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        player.transform.SetPositionAndRotation(evacuationSpawnPoint.position, evacuationSpawnPoint.rotation);

        if (controller != null) controller.enabled = true;
    }

    private void StartEvacuation()
    {
        CurrentPhase = GamePhase.Evacuation;
        IsTransitionActive = false;
        evacuationStartTime = Time.time;
        GameEvents.RaiseMessage(GameSettings.Current.evacuationStartedMessage);
    }

    public void ReachEvacuationPoint()
    {
        if (CurrentPhase != GamePhase.Evacuation) return;

        EvacuationSeconds = Time.time - evacuationStartTime;
        CurrentPhase = GamePhase.Finished;
        GameEvents.RaiseEvacuationCompleted();
    }
}
