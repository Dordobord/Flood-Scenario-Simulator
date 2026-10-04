using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GoBagGridView : MonoBehaviour
{
    private RectTransform rectTransform;
    private GoBagSettings settings;
    private Image[,] cellImages;
    private int width;
    private int height;
    private float cellSize;

    private bool previewShown;
    private int previewX;
    private int previewY;
    private int previewWidth;
    private int previewHeight;
    private bool previewFits;

    public bool IsBuilt => cellImages != null;

    public void Build(GoBagSettings goBagSettings)
    {
        if (IsBuilt) return;

        settings = goBagSettings;
        width = settings.gridWidth;
        height = settings.gridHeight;
        cellSize = settings.cellSize;

        rectTransform = (RectTransform)transform;
        rectTransform.sizeDelta = new Vector2(width * cellSize, height * cellSize);

        cellImages = new Image[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cellObject = new GameObject("Cell_" + x + "_" + y, typeof(RectTransform), typeof(Image));
                cellObject.layer = gameObject.layer;

                RectTransform cellRect = (RectTransform)cellObject.transform;
                cellRect.SetParent(transform, false);
                cellRect.anchorMin = new Vector2(0f, 1f);
                cellRect.anchorMax = new Vector2(0f, 1f);
                cellRect.pivot = new Vector2(0.5f, 0.5f);
                cellRect.anchoredPosition = GetCellCenter(x, y);
                cellRect.sizeDelta = new Vector2(cellSize - settings.cellGap, cellSize - settings.cellGap);

                Image cellImage = cellObject.GetComponent<Image>();
                cellImage.color = settings.emptyCellColor;
                cellImage.raycastTarget = false;

                cellImages[x, y] = cellImage;
            }
        }
    }

    public Vector2 GetCellPosition(int x, int y)
    {
        return new Vector2(x * cellSize, -y * cellSize);
    }

    public Vector2 GetCellCenter(int x, int y)
    {
        return GetCellPosition(x, y) + new Vector2(cellSize, -cellSize) * 0.5f;
    }

    public bool TryGetCellFromScreen(Vector2 screenPoint, out int x, out int y)
    {
        if (!TryGetCellFromScreenUnclamped(screenPoint, out x, out y)
            || x < 0 || y < 0 || x >= width || y >= height)
        {
            x = -1;
            y = -1;
            return false;
        }
        return true;
    }

    public bool TryGetCellFromScreenUnclamped(Vector2 screenPoint, out int x, out int y)
    {
        x = 0;
        y = 0;
        if (!IsBuilt) return false;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out Vector2 localPoint))
        {
            return false;
        }

        x = Mathf.FloorToInt(localPoint.x / cellSize);
        y = Mathf.FloorToInt(-localPoint.y / cellSize);
        return true;
    }

    public void ShowPreview(int x, int y, int itemWidth, int itemHeight, bool fits)
    {
        if (!IsBuilt) return;

        if (previewShown && x == previewX && y == previewY
            && itemWidth == previewWidth && itemHeight == previewHeight && fits == previewFits)
        {
            return;
        }

        ResetAllCellColors();
        Color color = fits ? settings.fitsColor : settings.blockedColor;

        for (int i = 0; i < itemWidth; i++)
        {
            for (int j = 0; j < itemHeight; j++)
            {
                SetCellColor(x + i, y + j, color);
            }
        }

        previewShown = true;
        previewX = x;
        previewY = y;
        previewWidth = itemWidth;
        previewHeight = itemHeight;
        previewFits = fits;
    }

    public void HidePreview()
    {
        if (!previewShown) return;

        ResetAllCellColors();
        previewShown = false;
    }

    private void SetCellColor(int x, int y, Color color)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return;
        cellImages[x, y].color = color;
    }

    private void ResetAllCellColors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cellImages[x, y].color = settings.emptyCellColor;
            }
        }
    }
}
