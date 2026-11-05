using UnityEngine;
using UnityEngine.UI;

public class ResultSlider : MonoBehaviour
{
    [Header("Slides")]
    [SerializeField] private CanvasGroup[] slides;

    [Header("Buttons")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image[] dots;

    [Header("Motion")]
    [SerializeField] private float slideSpacing = 700f;
    [SerializeField, Range(0f, 1f)] private float sideScale = 0.76f;
    [SerializeField, Range(0f, 1f)] private float sideAlpha = 0.5f;
    [SerializeField] private float moveSpeed = 8f;

    [Header("Arrows")]
    [SerializeField] private float arrowMoveDistance = 4f;
    [SerializeField] private float arrowMoveSeconds = 0.5f;

    private int current;
    private RectTransform previousRect;
    private RectTransform nextRect;
    private Vector2 previousStart;
    private Vector2 nextStart;

    private void Awake()
    {
        previousRect = previousButton.GetComponent<RectTransform>();
        nextRect = nextButton.GetComponent<RectTransform>();
        previousStart = previousRect.anchoredPosition;
        nextStart = nextRect.anchoredPosition;

        previousButton.onClick.AddListener(() => ChangeSlide(-1));
        nextButton.onClick.AddListener(() => ChangeSlide(1));
    }

    private void Update()
    {
        float step = Mathf.Clamp01(moveSpeed * Time.unscaledDeltaTime);

        for (int i = 0; i < slides.Length; i++)
        {
            MoveSlide(i, step);
        }

        MoveArrows();
    }

    public void ShowFirst()
    {
        current = 0;
        Refresh();

        for (int i = 0; i < slides.Length; i++)
        {
            MoveSlide(i, 1f);
        }
    }

    private void ChangeSlide(int direction)
    {
        current = Mathf.Clamp(current + direction, 0, slides.Length - 1);
        Refresh();
    }

    private void Refresh()
    {
        previousButton.gameObject.SetActive(current > 0);
        nextButton.gameObject.SetActive(current < slides.Length - 1);

        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].interactable = i == current;
            slides[i].blocksRaycasts = i == current;
        }

        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].color = GameSettings.Current.GetStarColor(i == current);
        }

        slides[current].transform.SetAsLastSibling();
    }

    private void MoveSlide(int index, float step)
    {
        int distance = index - current;
        RectTransform rect = slides[index].GetComponent<RectTransform>();

        float scale = distance == 0 ? 1f : sideScale;
        float alpha = distance == 0 ? 1f : Mathf.Abs(distance) == 1 ? sideAlpha : 0f;

        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, new Vector2(distance * slideSpacing, 0f), step);
        rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one * scale, step);
        slides[index].alpha = Mathf.Lerp(slides[index].alpha, alpha, step);
    }

    private void MoveArrows()
    {
        bool isOut = Mathf.Repeat(Time.unscaledTime, arrowMoveSeconds * 2f) < arrowMoveSeconds;
        float offset = isOut ? arrowMoveDistance : 0f;

        previousRect.anchoredPosition = previousStart + Vector2.left * offset;
        nextRect.anchoredPosition = nextStart + Vector2.right * offset;
    }
}
