using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    public static bool IsActive { get; private set; }

    public static bool CursorFree { get; private set; }

    [Header("Window")]
    [SerializeField] private GameObject window;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text attemptText;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text controlsHintText;
    [SerializeField] private Button cancelButton;

    [Header("Minigames")]
    [SerializeField] private HoldProgressMinigame holdMinigame;
    [SerializeField] private TimingBarMinigame timingMinigame;
    [SerializeField] private SelectCorrectMinigame selectMinigame;
    [SerializeField] private GoBagMinigame goBagMinigame;
    [SerializeField] private FlashlightMinigame flashlightMinigame;
    [SerializeField] private TuningMinigame tuningMinigame;

    private MinigameBase currentMinigame;
    private TaskData currentTask;
    private Action<bool> onFinished;
    private Coroutine resultRoutine;

    private int attemptNumber;
    private float attemptStartTime;
    private bool acceptingInput;
    private bool lastAttemptSucceeded;
    private int startFrame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        IsActive = false;
        CursorFree = false;

        holdMinigame.Finished += OnAttemptFinished;
        timingMinigame.Finished += OnAttemptFinished;
        selectMinigame.Finished += OnAttemptFinished;
        if (goBagMinigame != null) goBagMinigame.Finished += OnAttemptFinished;
        if (flashlightMinigame != null) flashlightMinigame.Finished += OnAttemptFinished;
        if (tuningMinigame != null) tuningMinigame.Finished += OnAttemptFinished;

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(Cancel);
        }
        else
        {
            Debug.LogWarning("MinigameManager: Cancel Button is not assigned.");
        }

        GameEvents.PreparationEnded += OnPreparationEnded;

        HideAllMinigames();
        window.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.PreparationEnded -= OnPreparationEnded;

        if (Instance == this)
        {
            Instance = null;
            IsActive = false;
            CursorFree = false;
        }
    }

    public bool Begin(TaskData task, Action<bool> finishedCallback)
    {
        if (IsActive || task == null) return false;

        GameSettings settings = GameSettings.Current;

        currentTask = task;
        onFinished = finishedCallback;
        lastAttemptSucceeded = false;

        attemptNumber = 1;
        if (TaskManager.Instance != null)
        {
            attemptNumber = TaskManager.Instance.GetAttempts(task) + 1;
        }

        IsActive = true;
        window.SetActive(true);
        titleText.text = task.title;

        if (controlsHintText != null)
        {
            controlsHintText.text = string.Format(settings.controlsHintFormat, settings.cursorToggleKey, settings.cancelMinigameKey);
        }

        bool startFree = false;
        if (task.minigame.type == MinigameType.SelectCorrect) startFree = settings.freeCursorForSelectCorrect;
        if (task.minigame.type == MinigameType.GoBag) startFree = settings.freeCursorForGoBag;
        if (task.minigame.type == MinigameType.Flashlight) startFree = settings.freeCursorForFlashlight;
        if (task.minigame.type == MinigameType.Tuning) startFree = settings.freeCursorForTuning;
        SetCursorFree(startFree);

        if (settings.logInput)
        {
            Debug.Log("[Minigame input] Opened '" + task.title + "'. Keyboard found: " + (Keyboard.current != null) + ". Cursor shown: " + startFree);
        }

        StartAttempt();
        return true;
    }

    private void Update()
    {
        if (!IsActive) return;

        GameSettings settings = GameSettings.Current;
        if (settings.logInput) LogInput(settings);

        if (MinigameInput.KeyPressedThisFrame(settings.cursorToggleKey))
        {
            SetCursorFree(!CursorFree);
        }

        if (MinigameInput.KeyPressedThisFrame(settings.cancelMinigameKey))
        {
            Cancel();
            return;
        }

        if (!acceptingInput || currentMinigame == null) return;

        if (Time.frameCount <= startFrame) return;

        currentMinigame.Tick();
    }

    private void OnPreparationEnded()
    {
        Cancel();
    }

    public void Cancel()
    {
        if (!IsActive) return;

        if (resultRoutine != null)
        {
            StopCoroutine(resultRoutine);
            resultRoutine = null;
        }
        acceptingInput = false;

        Close(lastAttemptSucceeded);
    }

    private void StartAttempt()
    {
        HideAllMinigames();
        feedbackText.text = "";
        bool hideAttempt = currentTask.minigame.type == MinigameType.GoBag && !GameSettings.Current.showAttemptTextForGoBag;
        attemptText.text = hideAttempt ? "" : string.Format(GameSettings.Current.attemptFormat, attemptNumber);
        lastAttemptSucceeded = false;

        MinigameSettings minigameSettings = currentTask.minigame;
        if (string.IsNullOrEmpty(minigameSettings.prompt))
        {
            promptText.text = currentTask.description;
        }
        else
        {
            promptText.text = minigameSettings.prompt;
        }

        currentMinigame = GetMinigame(minigameSettings.type);

        startFrame = Time.frameCount;
        attemptStartTime = Time.time;
        acceptingInput = true;
        currentMinigame.Begin(minigameSettings);
    }

    private MinigameBase GetMinigame(MinigameType type)
    {
        switch (type)
        {
            case MinigameType.TimingBar:
                return timingMinigame;
            case MinigameType.SelectCorrect:
                return selectMinigame;
            case MinigameType.GoBag:
                if (goBagMinigame == null)
                {
                    Debug.LogError("MinigameManager: Go Bag Minigame is not assigned. Using Select Correct for now.");
                    return selectMinigame;
                }
                return goBagMinigame;
            case MinigameType.Flashlight:
                if (flashlightMinigame == null)
                {
                    Debug.LogError("MinigameManager: Flashlight Minigame is not assigned. Using Select Correct for now.");
                    return selectMinigame;
                }
                return flashlightMinigame;
            case MinigameType.Tuning:
                if (tuningMinigame == null)
                {
                    Debug.LogError("MinigameManager: Tuning Minigame is not assigned. Using Select Correct for now.");
                    return selectMinigame;
                }
                return tuningMinigame;
            default:
                return holdMinigame;
        }
    }

    private void HideAllMinigames()
    {
        holdMinigame.End();
        timingMinigame.End();
        selectMinigame.End();
        if (goBagMinigame != null) goBagMinigame.End();
        if (flashlightMinigame != null) flashlightMinigame.End();
        if (tuningMinigame != null) tuningMinigame.End();
    }

    private void OnAttemptFinished(bool success)
    {
        if (!acceptingInput) return;
        acceptingInput = false;
        lastAttemptSucceeded = success;

        float seconds = Time.time - attemptStartTime;
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.RecordAttempt(currentTask, success, seconds, currentMinigame.Mistakes);
        }

        if (resultRoutine != null) StopCoroutine(resultRoutine);
        resultRoutine = StartCoroutine(ShowResult(success));
    }

    private IEnumerator ShowResult(bool success)
    {
        GameSettings settings = GameSettings.Current;

        if (success)
        {
            feedbackText.color = settings.successColor;
            feedbackText.text = settings.successMessage;
            yield return new WaitForSecondsRealtime(settings.successDelay);

            resultRoutine = null;
            Close(true);
        }
        else
        {
            feedbackText.color = settings.failColor;
            feedbackText.text = settings.failMessage;
            yield return new WaitForSecondsRealtime(settings.failDelay);

            resultRoutine = null;
            attemptNumber++;
            StartAttempt();
        }
    }

    private void Close(bool success)
    {
        currentMinigame = null;
        HideAllMinigames();
        window.SetActive(false);
        IsActive = false;

        SetCursorFree(false);

        Action<bool> callback = onFinished;
        onFinished = null;
        currentTask = null;

        if (callback != null) callback(success);
    }

    private void SetCursorFree(bool free)
    {
        CursorFree = free;

        if (free)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = free;
    }

    private void LogInput(GameSettings settings)
    {
        if (MinigameInput.KeyPressedThisFrame(settings.actionKey))
        {
            Debug.Log("[Minigame input] Action key pressed.");
        }

        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log("[Minigame input] Left click. Counts as the action: " + (settings.allowMouseClick && !CursorFree));
        }

        if (MinigameInput.KeyPressedThisFrame(settings.cursorToggleKey))
        {
            Debug.Log("[Minigame input] Cursor toggle key pressed. Cursor shown before this press: " + CursorFree);
        }

        if (MinigameInput.KeyPressedThisFrame(settings.cancelMinigameKey))
        {
            Debug.Log("[Minigame input] Cancel key pressed.");
        }
    }
}
