using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace StormWaits
{
    
    
    
    
    
    
    
    
    
    
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
        [SerializeField] private DragToBagMinigame dragMinigame;
        [SerializeField] private GridPackMinigame gridMinigame;

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
            if (dragMinigame != null) dragMinigame.Finished += OnAttemptFinished;
            if (gridMinigame != null) gridMinigame.Finished += OnAttemptFinished;

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(Cancel);
            }

            if (cancelButton == null || dragMinigame == null || gridMinigame == null)
            {
                Debug.LogWarning("The MinigameWindow prefab is out of date. Run Storm Waits > 5. Rebuild UI Prefabs.");
            }

            HideAllMinigames();
            window.SetActive(false);
        }

        private void OnDestroy()
        {
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
            if (task.minigame.type == MinigameType.DragToBag) startFree = settings.freeCursorForDragToBag;
            if (task.minigame.type == MinigameType.GridPack) startFree = settings.freeCursorForGridPack;
            SetCursorFree(startFree);

            if (settings.logInput)
            {
                Debug.Log("[Minigame input] Opened '" + task.title + "'. Keyboard found: " + (Keyboard.current != null) + ". Cursor shown: " + startFree);
            }

            GameEvents.RaiseMinigameStarted(task);
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
            attemptText.text = string.Format(GameSettings.Current.attemptFormat, attemptNumber);
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
                case MinigameType.DragToBag:
                    if (dragMinigame == null)
                    {
                        
                        Debug.LogError("The MinigameWindow prefab has no Drag To Bag panel. Run Storm Waits > 5. Rebuild UI Prefabs. Using Select Correct for now.");
                        return selectMinigame;
                    }
                    return dragMinigame;
                case MinigameType.GridPack:
                    if (gridMinigame == null)
                    {
                        Debug.LogError("The MinigameWindow prefab has no Grid Pack panel. Run Storm Waits > 5. Rebuild UI Prefabs. Using Select Correct for now.");
                        return selectMinigame;
                    }
                    return gridMinigame;
                default:
                    return holdMinigame;
            }
        }

        private void HideAllMinigames()
        {
            holdMinigame.End();
            timingMinigame.End();
            selectMinigame.End();
            if (dragMinigame != null) dragMinigame.End();
            if (gridMinigame != null) gridMinigame.End();
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
            TaskData task = currentTask;
            onFinished = null;
            currentTask = null;

            GameEvents.RaiseMinigameEnded(task, success);
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
}
