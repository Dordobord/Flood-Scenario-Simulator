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
    public Key cancelMinigameKey = Key.Backspace;
    public bool allowMouseClick = true;
    public Key moveForwardKey = Key.W;
    public Key moveBackKey = Key.S;
    public Key moveLeftKey = Key.A;
    public Key moveRightKey = Key.D;

    [Header("Debug")]
    public bool logInput = false;

    [Header("Interaction")]
    public float interactRange = 3f;
    public string promptFormat = "[{0}] {1}";
    public string holdPromptFormat = "Hold [{0}] {1}";
    public string placePromptFormat = "Place {0}";
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

    [Header("Minigame text")]
    public string successMessage = "Success!";
    public string failMessage = "Not quite. Try again!";
    public string hitsFormat = "Hits: {0}/{1}";
    public string selectedMark = "[x]  ";
    public string unselectedMark = "[ ]  ";
    public string actionWithClickFormat = "{0} or left click";
    public string holdHintFormat = "Hold {0}";
    public string timingHintFormat = "Press {0} when the marker is in the green zone";

    [Header("Breaker")]
    public string breakerMainLabel = "MAIN";
    public string breakerOnText = "ON";
    public string breakerOffText = "OFF";
    public Color breakerOnColor = new Color(0.75f, 0.25f, 0.25f);
    public Color breakerOffColor = new Color(0.25f, 0.6f, 0.35f);
    public string breakerCoverHint = "Click the cover to open the breaker panel.";
    public string breakerSwitchHint = "Switch off every circuit first, then MAIN.";
    public string breakerMainEarlyHint = "Spark! Switch off the other circuits before MAIN.";

    [Header("Stove")]
    public Color stoveOnColor = new Color(0.85f, 0.45f, 0.2f);
    public Color stoveOffColor = new Color(0.25f, 0.6f, 0.35f);
    public string stoveKnobHint = "Drag each burner knob around until it points to OFF.";
    public string stoveValveHint = "Now turn the LPG valve clockwise until it is closed.";
    public string stoveValveLockedHint = "Turn off every burner before closing the LPG valve.";

    [Header("Sandbags")]
    public string sandbagTakePrompt = "Take a sandbag";
    public string sandbagPlacePromptFormat = "Place Sandbag ({0}/{1})";
    public string sandbagPlacedMessageFormat = "Sandbag placed ({0}/{1}). Get another from the pile!";
    public string sandbagAlreadyCarryingMessage = "You are already carrying a sandbag.";

    [Header("Leave Early")]
    public string leavePrompt = "Leave now and evacuate";
    public string leaveConfirmPrompt = "Confirm: leave now";
    public string leaveConfirmMessageFormat = "Leave now? Unfinished tasks will not count. Press {0} again to confirm.";
    public float leaveConfirmSeconds = 3f;

    [Header("Hotbar")]
    public Key[] hotbarKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5 };
    public bool hotbarScrollWheel = true;
    public Color hotbarNormalColor = new Color(0f, 0f, 0f, 0.55f);
    public Color hotbarSelectedColor = new Color(0.25f, 0.28f, 0.32f, 0.85f);
    public Color hotbarOutlineColor = new Color(1f, 1f, 1f, 1f);
    public float hotbarSelectedScale = 1.1f;

    [Header("Phone")]
    public string phoneTitle = "Phone";
    public string phoneTimeFormat = "HH:mm";
    public string phoneDoneText = "Done";
    public Color phoneDoneIconColor = new Color(0.45f, 0.48f, 0.52f);

    [Header("Phone Contacts")]
    public Color contactSavedRowColor = new Color(0.12f, 0.165f, 0.21f);
    public Color contactMarkerSavedColor = new Color(0.216f, 0.541f, 0.867f);

    [Header("Phone Notes")]
    public Color noteDoneTextColor = new Color(0.45f, 0.48f, 0.52f);
    public Color noteBoxDoneColor = new Color(0.114f, 0.62f, 0.459f);

    [Header("Power Bank")]
    public string powerBankPlugPrompt = "Plug in the power bank";
    public string powerBankChargingFormat = "Charging power bank {0}%";
    public string powerBankTakePrompt = "Take the charged power bank";
    public string powerBankStillChargingMessage = "Still charging. Do other tasks while you wait.";
    public string powerBankPackedMessage = "The power bank is charged and packed in the Go Bag.";
    public string powerBankLabelFormat = "<b>Power Bank {0}%</b>";

    [Header("Unplug Appliances")]
    public string unplugPromptFormat = "Unplug the {0}";
    public string unpluggedMessageFormat = "Unplugged the {0} ({1}/{2})";
    public string unpluggedLabelFormat = "<b>{0} (unplugged)</b>";

    [Header("Windows")]
    public string windowClosePrompt = "Close the window";
    public string windowOpenPrompt = "Open the window";
    public string windowClosedMessageFormat = "{0} closed ({1}/{2}). Close the rest!";
    public string windowName = "Window";

    [Header("Doors")]
    public string doorOpenPrompt = "Open the door";
    public string doorClosePrompt = "Close the door";

    [Header("Power")]
    public string noPowerMessage = "No power. The breaker is turned off.";
    public string noPowerPrompt = "No power (breaker is off)";
    public string powerOnPrompt = "Turn the power back on";
    public string powerOnMessage = "The power is back on.";

    [Header("Evacuation Marker")]
    public string evacuationMarkerLabelFormat = "{0}\n{1} m";
    public Color evacuationMarkerColor = new Color(0.25f, 0.85f, 0.4f);

    [Header("Hazards")]
    [Range(0f, 1f)] public float potholeSpeedMultiplier = 0.4f;
    public float potholeDepth = 0.3f;
    public float potholeDropSpeed = 3f;
    public float hazardAlertDelay = 5f;
    public string hazardAlertMessage = "Barangay alert: your area is flood prone. Check the Hazard Map app on your phone.";
    [TextArea] public string hazardWarningText = "<color=#D63031><b>Flood warning</b></color>\n<b>Your area is prone to flooding soon.</b>\nAvoid potholes and evacuate immediately.";
    public string potholeCountFormat = "Potholes stepped in: {0}";

    [Header("Messages")]
    public string inventoryFullMessage = "Your inventory is full.";
    public string collectedMessage = "Collected: ";
    public string taskCompleteMessage = "Task complete: ";

    [Header("Scoring")]
    public float criticalWeight = 2f;
    public float importantWeight = 1f;
    public float speedTimeLimit = 30f;
    [Range(0f, 1f)] public float minSpeedScore = 0.5f;
    [Range(0f, 1f)] public float mistakePenalty = 0.2f;
    [Range(0f, 1f)] public float retryPenalty = 0.1f;
    [Range(0f, 1f)] public float minTaskScore = 0.4f;
    public float wellPreparedPercent = 85f;
    public float partlyPreparedPercent = 50f;
    public string wellPreparedRating = "Well Prepared";
    public string partlyPreparedRating = "Partially Prepared";
    public string notPreparedRating = "Not Prepared";

    [Header("Results")]
    public bool showScore = false;
    public string tasksDoneFormat = "Tasks done: {0}/{1}";
    public string missedTasksHeading = "Missed tasks:";
    public string scorePercentFormat = "Preparedness Score: {0:0}%";
    public string ratingFormat = "Rating: {0}";
    public string ratingCappedNote = "(capped because a critical task was missed)";
    public string domainPercentFormat = "{0}: {1:0}%";
    public string taskScoreFormat = "{0} ({1:0}%)";
    public string missedCriticalHeading = "Missed critical tasks:";
    public string allDoneMessage = "You completed every task. You are well prepared!";
    public string evacuationTimeFormat = "Evacuation Time: {0}";
    public string weakestDomainFormat = "Weakest: {0}";
    public string domainSeparator = ", ";
    public string listBullet = "- ";
    public string recommendationFormat = "{0}: {1}";
    public string noTasksCompletedMessage = "None";
    public List<DomainDisplayName> domainDisplayNames = new List<DomainDisplayName>
    {
        new DomainDisplayName(PreparednessDomain.Planning, "Planning"),
        new DomainDisplayName(PreparednessDomain.Training, "Training"),
        new DomainDisplayName(PreparednessDomain.MaterialPreparedness, "Material Preparedness"),
        new DomainDisplayName(PreparednessDomain.Information, "Information")
    };

    [Header("Medication")]
    public Color medicinePickedColor = new Color(0.18f, 0.31f, 0.48f);
    public string pouchItemFormat = "{0} ({1})";
    public string pouchEmptyText = "Click a medicine to pack it.";

    [Header("Rainfall Warning")]
    public string rainfallWarningTitleFormat = "PAGASA: {0} RAINFALL WARNING";
    public List<RainfallWarningInfo> rainfallWarnings = new List<RainfallWarningInfo>
    {
        new RainfallWarningInfo(RainfallWarningLevel.Yellow, "YELLOW", new Color(0.95f, 0.77f, 0.06f), "Be alert. Flooding is possible in low-lying areas."),
        new RainfallWarningInfo(RainfallWarningLevel.Orange, "ORANGE", new Color(0.9f, 0.49f, 0.13f), "Be prepared. Get ready to evacuate."),
        new RainfallWarningInfo(RainfallWarningLevel.Red, "RED", new Color(0.91f, 0.3f, 0.24f), "Take action. Evacuate now.")
    };

    [Header("Pause")]
    public Key pauseKey = Key.Escape;
    public string pauseTitle = "Paused";

    [Header("Prep Boundary")]
    public string leaveHouseMessage = "Finish preparing first. You will evacuate when the timer ends.";
    public float leaveHouseMessageCooldown = 3f;

    [Header("Weather")]
    public float maxRainDropsPerSecond = 4000f;
    public float rainRampSeconds = 4f;
    public float maxWetness = 1f;
    public Color clearFogColor = new Color(0.62f, 0.66f, 0.72f);
    public Color stormFogColor = new Color(0.3f, 0.33f, 0.38f);
    public float clearFogDensity = 0.002f;
    public float stormFogDensity = 0.03f;
    [Range(0f, 1f)] public float stormSunMultiplier = 0.35f;
    [Range(0f, 1f)] public float stormSkyMultiplier = 0.4f;
    [Range(0f, 1f)] public float rainVolume = 0.8f;
    [Range(0f, 1f)] public float indoorFogMultiplier = 0.15f;
    [Range(0f, 1f)] public float indoorRainVolumeMultiplier = 0.4f;
    public float indoorBlendSeconds = 0.5f;

    [Header("Main Menu")]
    public string mainMenuSceneName = "MainMenu";
    public string menuTitle = "The Storm Waits for Nobody";
    public string menuSubtitle = "Choose a flood scenario";
    public string noScenarioSelectedTitle = "No scenario selected";
    public string noScenarioSelectedMessage = "Pick a scenario on the left to see its details.";
    public string preparationTimeFormat = "Preparation time: {0}";
    public Color scenarioNormalColor = new Color(0.2f, 0.24f, 0.32f);
    public Color scenarioSelectedColor = new Color(0.2f, 0.45f, 0.75f);

    [Header("Colors")]
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

    public RainfallWarningInfo GetRainfallWarning(RainfallWarningLevel level)
    {
        foreach (RainfallWarningInfo warning in rainfallWarnings)
        {
            if (warning.level == level) return warning;
        }
        return null;
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
