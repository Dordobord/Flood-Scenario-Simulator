using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    public static bool IsActive { get; private set; }

    public static bool CursorFree { get; private set; }

    public static bool IsPhoneOpen
    {
        get { return Instance != null && IsActive && Instance.phoneDevice != null; }
    }

    [Header("Window")]
    [SerializeField] private GameObject window;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    [Header("Phone View")]
    [SerializeField] private List<Graphic> hiddenGraphicsForPhone = new List<Graphic>();
    [SerializeField] private List<GameObject> hiddenObjectsForPhone = new List<GameObject>();

    [Header("Minigames")]
    [SerializeField] private HoldProgressMinigame holdMinigame;
    [SerializeField] private GoBagMinigame goBagMinigame;
    [SerializeField] private FlashlightMinigame flashlightMinigame;
    [SerializeField] private TVMinigame tvMinigame;
    [SerializeField] private RadioMinigame radioMinigame;
    [SerializeField] private BreakerMinigame breakerMinigame;
    [SerializeField] private StoveMinigame stoveMinigame;
    [SerializeField] private PhoneMinigame phoneMinigame;
    [SerializeField] private MedicationMinigame medicationMinigame;
    [SerializeField] private FirstAidMinigame firstAidMinigame;

    private Dictionary<MinigameType, MinigameBase> minigames = new Dictionary<MinigameType, MinigameBase>();
    private MinigameBase currentMinigame;
    private TaskData currentTask;
    private Action<bool> onFinished;
    private Coroutine resultRoutine;
    private PhoneDevice phoneDevice;
    private bool phoneInWindow;
    private bool plainPageOpen;

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

        Register(MinigameType.HoldProgress, holdMinigame);
        Register(MinigameType.GoBag, goBagMinigame);
        Register(MinigameType.Flashlight, flashlightMinigame);
        Register(MinigameType.TV, tvMinigame);
        Register(MinigameType.Radio, radioMinigame);
        Register(MinigameType.Breaker, breakerMinigame);
        Register(MinigameType.Stove, stoveMinigame);
        Register(MinigameType.Medication, medicationMinigame);
        Register(MinigameType.FirstAid, firstAidMinigame);

        if (phoneMinigame == null) phoneMinigame = FindFirstObjectByType<PhoneMinigame>(FindObjectsInactive.Include);
        phoneInWindow = phoneMinigame != null && phoneMinigame.transform.IsChildOf(window.transform);

        if (phoneMinigame != null && phoneMinigame.HomeScreen == null)
        {
            Debug.LogWarning("MinigameManager: the phone has no Home Screen assigned.");
        }
        else if (phoneMinigame != null)
        {
            Register(MinigameType.Phone, phoneMinigame);
            phoneMinigame.HomeScreen.AppChosen += OnPhoneAppChosen;
            phoneMinigame.HomePressed += OnPhoneHomePressed;
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(Cancel);
        }
        else
        {
            Debug.LogWarning("MinigameManager: Cancel Button is not assigned.");
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }
        else
        {
            Debug.LogWarning("MinigameManager: Confirm Button is not assigned.");
        }

        GameEvents.PreparationEnded += OnPreparationEnded;

        HideAllMinigames();
        window.SetActive(false);
    }

    private void Register(MinigameType type, MinigameBase minigame)
    {
        if (minigame == null) return;

        minigames[type] = minigame;
        minigame.Finished += OnAttemptFinished;
        minigame.FeedbackShown += OnFeedbackShown;
        minigame.InstructionsChanged += OnInstructionsChanged;
    }

    private void OnFeedbackShown(string message)
    {
        feedbackText.color = GameSettings.Current.failColor;
        feedbackText.text = message;
        AudioManager.PlaySfx(SoundLibrary.Current.fail);
    }

    private void OnInstructionsChanged(string message)
    {
        promptText.text = message;
    }

    private void OnConfirmClicked()
    {
        if (!acceptingInput || currentMinigame == null) return;

        currentMinigame.Confirm();
    }

    private void SetConfirmVisible(bool visible)
    {
        if (confirmButton != null) confirmButton.gameObject.SetActive(visible);
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

        currentTask = task;
        onFinished = finishedCallback;
        lastAttemptSucceeded = false;

        IsActive = true;
        window.SetActive(true);
        SetPhoneView(false);
        titleText.text = task.title;

        SetCursorFree(true);

        StartAttempt();
        return true;
    }

    public bool Reopen(TaskData task)
    {
        if (IsActive || task == null) return false;

        MinigameBase minigame = GetMinigame(task.minigame.type);
        if (!minigame.CanReopen) return false;

        currentTask = task;
        onFinished = null;
        lastAttemptSucceeded = true;
        acceptingInput = false;

        IsActive = true;
        window.SetActive(true);
        SetPhoneView(false);
        HideAllMinigames();

        titleText.text = task.title;
        promptText.text = GameSettings.Current.reopenInstructions;
        feedbackText.text = "";
        SetConfirmVisible(false);
        SetCursorFree(true);

        currentMinigame = minigame;
        minigame.Reopen(task.minigame);
        return true;
    }

    public bool BeginPhone(PhoneDevice device)
    {
        if (IsActive || device == null) return false;

        if (!minigames.ContainsKey(MinigameType.Phone))
        {
            Debug.LogWarning("MinigameManager: Phone Minigame is not assigned.");
            return false;
        }

        phoneDevice = device;
        IsActive = true;
        window.SetActive(phoneInWindow);
        SetPhoneView(true);

        ShowPhoneHome();
        return true;
    }

    private void ShowPhoneHome()
    {
        GameSettings settings = GameSettings.Current;

        HideAllMinigames();
        currentMinigame = null;
        currentTask = null;
        onFinished = null;
        acceptingInput = false;
        plainPageOpen = false;

        titleText.text = settings.phoneTitle;
        promptText.text = "";
        feedbackText.text = "";
        SetConfirmVisible(false);
        SetCursorFree(true);

        phoneMinigame.ShowHome();
    }

    private void OnPhoneAppChosen(PhonePage page)
    {
        if (phoneDevice == null || page == null || currentTask != null)
        {
            Debug.LogWarning("MinigameManager: could not open " + (page != null ? page.name : "the app") + " because the phone is busy or the icon has no Page.");
            return;
        }

        if (page.Task == null)
        {
            plainPageOpen = true;
            phoneMinigame.ShowPage(page);
            return;
        }

        currentTask = page.Task;
        lastAttemptSucceeded = false;
        titleText.text = currentTask.title;
        StartAttempt();
    }

    private void OnPhoneHomePressed()
    {
        if (phoneDevice == null) return;

        if (currentTask != null) Cancel();
        else if (plainPageOpen) ShowPhoneHome();
    }

    private bool TryReturnToPhone(bool success)
    {
        if (phoneDevice == null || currentTask == null) return false;

        TaskData finishedTask = currentTask;
        currentTask = null;
        phoneDevice.OnAppFinished(finishedTask, success);
        ShowPhoneHome();
        return true;
    }

    private void Update()
    {
        if (!IsActive) return;

        GameSettings settings = GameSettings.Current;
        if (MinigameInput.KeyPressedThisFrame(settings.cancelMinigameKey))
        {
            Cancel();
            return;
        }

        if (!acceptingInput || currentMinigame == null) return;

        if (Time.frameCount <= startFrame) return;

        currentMinigame.Tick();
    }

    public void ClosePhone()
    {
        if (phoneDevice == null) return;

        phoneDevice = null;
        Cancel();
    }

    private void OnPreparationEnded()
    {
        phoneDevice = null;
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
        lastAttemptSucceeded = false;

        MinigameSettings minigameSettings = currentTask.minigame;
        if (minigameSettings.type == MinigameType.Phone)
        {
            promptText.text = "";
        }
        else if (string.IsNullOrEmpty(minigameSettings.prompt))
        {
            promptText.text = currentTask.description;
        }
        else
        {
            promptText.text = minigameSettings.prompt;
        }

        currentMinigame = GetMinigame(minigameSettings.type);
        SetConfirmVisible(currentMinigame.UsesConfirm);

        startFrame = Time.frameCount;
        attemptStartTime = Time.time;
        acceptingInput = true;
        currentMinigame.Begin(minigameSettings);
    }

    private MinigameBase GetMinigame(MinigameType type)
    {
        MinigameBase minigame;
        if (minigames.TryGetValue(type, out minigame)) return minigame;
        return holdMinigame;
    }

    private void HideAllMinigames()
    {
        foreach (MinigameBase minigame in minigames.Values)
        {
            minigame.End();
        }
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
            AudioManager.PlaySfx(SoundLibrary.Current.fail);
            yield return new WaitForSecondsRealtime(settings.failDelay);

            resultRoutine = null;
            StartAttempt();
        }
    }

    private void Close(bool success)
    {
        if (TryReturnToPhone(success)) return;

        phoneDevice = null;
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

    private void SetPhoneView(bool phoneView)
    {
        foreach (Graphic graphic in hiddenGraphicsForPhone)
        {
            if (graphic != null) graphic.enabled = !phoneView;
        }
        foreach (GameObject hidden in hiddenObjectsForPhone)
        {
            if (hidden != null) hidden.SetActive(!phoneView);
        }
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
}
