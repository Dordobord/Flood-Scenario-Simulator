using UnityEngine;
using UnityEngine.InputSystem;

namespace StormWaits
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Storm Waits/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Keys")]
        public Key interactKey = Key.E;
        public Key actionKey = Key.E;
        [Tooltip("Shows or hides the cursor while a minigame is open.")]
        public Key cursorToggleKey = Key.Escape;
        [Tooltip("Closes a minigame without finishing it.")]
        public Key cancelMinigameKey = Key.Backspace;
        [Tooltip("Left click counts as the action key while the cursor is hidden.")]
        public bool allowMouseClick = true;

        [Header("Cursor")]
        [Tooltip("If on, Select Correct opens with the cursor already visible, so you can click right away.")]
        public bool freeCursorForSelectCorrect = false;
        [Tooltip("If on, Drag To Bag opens with the cursor visible. Dragging needs the cursor, so this is on by default.")]
        public bool freeCursorForDragToBag = true;
        [Tooltip("If on, Grid Pack opens with the cursor visible. Needed, since placing pieces uses the mouse.")]
        public bool freeCursorForGridPack = true;

        [Header("Debug")]
        [Tooltip("Prints to the Console when minigame keys and clicks are detected.")]
        public bool logInput = false;

        [Header("Interaction")]
        public float interactRange = 3f;
        [Tooltip("{0} is the key name, {1} is the task title.")]
        public string promptFormat = "[{0}] {1}";

        [Header("Minigame timing")]
        public float successDelay = 0.9f;
        public float failDelay = 1.1f;
        [Tooltip("How fast the hold bar drains when the button is released.")]
        public float holdDrainMultiplier = 1.5f;
        [Tooltip("Keeps the timing bar target away from the edges.")]
        [Range(0f, 0.4f)] public float targetEdgeMargin = 0.1f;

        [Header("Phase Timer")]
        [Tooltip("How long the preparation phase lasts.")]
        public float preparationSeconds = 120f;
        [Tooltip("{0} is the time remaining as mm:ss.")]
        public string prepTimerFormat = "Prep: {0}";
        [Tooltip("Shown at the top left once the flood phase starts.")]
        public string floodTimerText = "FLOOD PHASE";
        [Tooltip("The timer turns red at or below this many seconds left.")]
        public float timerWarningSeconds = 30f;
        public Color timerNormalColor = Color.white;
        public Color timerWarningColor = new Color(1f, 0.3f, 0.3f);

        [Header("Phase Transition")]
        public string transitionText = "Heading to higher ground...";
        public float transitionFadeSeconds = 1f;
        public float transitionHoldSeconds = 1.5f;
        public string floodStartedMessage = "The flood phase has begun!";

        [Header("Grid Pack")]
        [Tooltip("Rotates the piece you are holding, swapping its width and height.")]
        public Key rotateKey = Key.R;
        [Tooltip("Columns in the bag grid. The same size is used for every Grid Pack task.")]
        [Min(1)] public int gridColumns = 6;
        [Tooltip("Rows in the bag grid.")]
        [Min(1)] public int gridRows = 4;
        [Tooltip("Size in pixels of one grid cell.")]
        public float gridCellSize = 80f;
        [Tooltip("Size in pixels of a piece's icon while it sits in the pool, before it is picked up.")]
        public float gridPoolIconSize = 100f;
        public string gridHintText = "Click a piece to pick it up. R to rotate. Click again to place it in the bag.";
        public Color emptyCellColor = new Color(1f, 1f, 1f, 0.22f);
        public Color previewValidColor = new Color(0.3f, 0.8f, 0.4f, 0.9f);
        public Color previewInvalidColor = new Color(0.85f, 0.25f, 0.25f, 0.9f);

        [Header("Drag To Bag")]
        [Tooltip("{0} is items packed, {1} is items to pack, {2} is mistakes.")]
        public string packedFormat = "Packed {0}/{1}     Mistakes {2}";
        [Tooltip("Used when the task has a Max Mistakes limit. {3} is the limit.")]
        public string packedLimitFormat = "Packed {0}/{1}     Mistakes {2}/{3}";
        public Color tileColor = new Color(0.25f, 0.3f, 0.4f);
        public Color packedTileColor = new Color(0.25f, 0.55f, 0.35f);
        public Color wrongDropColor = new Color(0.85f, 0.25f, 0.25f);
        [Tooltip("How long a tile stays red after a wrong drop.")]
        public float wrongFlashSeconds = 0.4f;
        [Tooltip("How see-through a tile is while it is being dragged.")]
        [Range(0.3f, 1f)] public float dragAlpha = 0.8f;

        [Header("HUD")]
        public float toastSeconds = 3f;
        [Tooltip("{0} is completed tasks, {1} is total tasks.")]
        public string taskHeaderFormat = "Tasks {0}/{1}";
        public string doneMark = "[x] ";
        public string todoMark = "[ ] ";

        [Header("Minigame text")]
        public string attemptFormat = "Attempt {0}";
        public string successMessage = "Success!";
        public string failMessage = "Not quite. Try again!";
        [Tooltip("{0} is hits so far, {1} is hits needed.")]
        public string hitsFormat = "Hits: {0}/{1}";
        public string selectedMark = "[x]  ";
        public string unselectedMark = "[ ]  ";
        [Tooltip("{0} is the action key name.")]
        public string holdHintFormat = "Hold {0}";
        [Tooltip("{0} is the action key name.")]
        public string timingHintFormat = "Press {0} when the marker is in the green zone";
        [Tooltip("{0} is the cursor toggle key, {1} is the cancel key.")]
        public string controlsHintFormat = "{0}: show or hide cursor     {1}: cancel";

        [Header("Messages")]
        public string inventoryFullMessage = "Your inventory is full.";
        public string collectedMessage = "Collected: ";
        public string taskCompleteMessage = "Task complete: ";

        [Header("Colors")]
        public Color taskDoneColor = new Color(0.49f, 0.99f, 0f);
        public Color successColor = new Color(0.4f, 0.95f, 0.5f);
        public Color failColor = new Color(1f, 0.45f, 0.4f);
        public Color choiceNormalColor = new Color(0.2f, 0.24f, 0.32f);
        public Color choiceSelectedColor = new Color(0.2f, 0.45f, 0.75f);

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
}
