using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Storm Waits/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Keys")]
    public Key interactKey = Key.E;
    public Key actionKey = Key.E;
    public Key cursorToggleKey = Key.Escape;
    public Key cancelMinigameKey = Key.Backspace;
    public bool allowMouseClick = true;
    public Key moveForwardKey = Key.W;
    public Key moveBackKey = Key.S;
    public Key moveLeftKey = Key.A;
    public Key moveRightKey = Key.D;

    [Header("Cursor")]
    public bool freeCursorForSelectCorrect = false;
    public bool freeCursorForGoBag = true;
    public bool freeCursorForFlashlight = true;
    public bool freeCursorForTuning = true;

    [Header("Debug")]
    public bool logInput = false;

    [Header("Interaction")]
    public float interactRange = 3f;
    public string promptFormat = "[{0}] {1}";
    public string defaultPrompt = "Interact";

    [Header("Minigame timing")]
    public float successDelay = 0.9f;
    public float failDelay = 1.1f;
    public float holdDrainMultiplier = 1.5f;
    [Range(0f, 0.4f)] public float targetEdgeMargin = 0.1f;

    [Header("Phase Timer")]
    public float preparationSeconds = 120f;
    public string prepTimerFormat = "Prep: {0}";
    [FormerlySerializedAs("floodTimerText")]
    public string evacuationTimerText = "EVACUATE";
    public float timerWarningSeconds = 30f;
    public Color timerNormalColor = Color.white;
    public Color timerWarningColor = new Color(1f, 0.3f, 0.3f);
    [FormerlySerializedAs("floodTimerColor")]
    public Color evacuationTimerColor = new Color(1f, 0.3f, 0.3f);

    [Header("Phase Transition")]
    public string transitionText = "Heading to higher ground...";
    public float transitionFadeSeconds = 1f;
    public float transitionHoldSeconds = 1.5f;
    [FormerlySerializedAs("floodStartedMessage")]
    public string evacuationStartedMessage = "Evacuate! Get to the evacuation center.";

    [Header("HUD")]
    public float toastSeconds = 3f;
    public string taskHeaderFormat = "Tasks {0}/{1}";
    public bool boldTaskHeader = true;
    public string doneMark = "[x] ";
    public string todoMark = "[ ] ";

    [Header("Minigame text")]
    public string attemptFormat = "Attempt {0}";
    public bool showAttemptTextForGoBag = false;
    public string successMessage = "Success!";
    public string failMessage = "Not quite. Try again!";
    public string hitsFormat = "Hits: {0}/{1}";
    public string selectedMark = "[x]  ";
    public string unselectedMark = "[ ]  ";
    public string actionWithClickFormat = "{0} or left click";
    public string holdHintFormat = "Hold {0}";
    public string timingHintFormat = "Press {0} when the marker is in the green zone";
    public string controlsHintFormat = "{0}: show or hide cursor     {1}: cancel";

    [Header("Messages")]
    public string inventoryFullMessage = "Your inventory is full.";
    public string collectedMessage = "Collected: ";
    public string taskCompleteMessage = "Task complete: ";

    [Header("Results")]
    public string scoreFormat = "Preparedness Score: {0:0.##}% ({1}/{2} tasks)";
    public string evacuationTimeFormat = "Evacuation Time: {0}";
    public string domainScoreFormat = "{0}: {1}/{2}";
    public string weakestDomainFormat = "Weakest: {0}";
    public string domainSeparator = ", ";
    public string listBullet = "- ";
    public string recommendationFormat = "{0}: {1}";
    public string noTasksCompletedMessage = "None";
    public string wellPreparedMessage = "You completed every task. You are well prepared!";
    public List<DomainDisplayName> domainDisplayNames = new List<DomainDisplayName>
    {
        new DomainDisplayName(PreparednessDomain.Planning, "Planning"),
        new DomainDisplayName(PreparednessDomain.Training, "Training"),
        new DomainDisplayName(PreparednessDomain.MaterialPreparedness, "Material Preparedness"),
        new DomainDisplayName(PreparednessDomain.Information, "Information")
    };

    [Header("Colors")]
    public Color taskDoneColor = new Color(0.49f, 0.99f, 0f);
    public Color successColor = new Color(0.4f, 0.95f, 0.5f);
    public Color failColor = new Color(1f, 0.45f, 0.4f);
    public Color choiceNormalColor = new Color(0.2f, 0.24f, 0.32f);
    public Color choiceSelectedColor = new Color(0.2f, 0.45f, 0.75f);

    public string GetDomainName(PreparednessDomain domain)
    {
        foreach (DomainDisplayName entry in domainDisplayNames)
        {
            if (entry.domain == domain && !string.IsNullOrEmpty(entry.displayName)) return entry.displayName;
        }
        return domain.ToString();
    }

    private static GameSettings current;

    public static GameSettings Current
    {
        get
        {
            if (current == null)
            {
                Debug.LogWarning("No GameSettings assigned on GameSystems. Using default values.");
                current = CreateInstance<GameSettings>();
            }
            return current;
        }
        set
        {
            current = value;
        }
    }
}
