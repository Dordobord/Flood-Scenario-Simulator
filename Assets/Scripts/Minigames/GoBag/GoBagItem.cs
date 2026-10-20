using UnityEngine;

public class GoBagItem
{
    public SelectChoice Choice { get; private set; }
    public bool IsRotated { get; private set; }

    public string Name => Choice.label;
    public Sprite Icon => Choice.icon;

    public int BaseWidth => Mathf.Max(1, Choice.gridWidth);
    public int BaseHeight => Mathf.Max(1, Choice.gridHeight);

    public int Width => IsRotated ? BaseHeight : BaseWidth;
    public int Height => IsRotated ? BaseWidth : BaseHeight;

    public bool CanRotate => BaseWidth != BaseHeight;

    public GoBagItem(SelectChoice choice)
    {
        Choice = choice;
    }

    public void Rotate()
    {
        if (!CanRotate) return;
        IsRotated = !IsRotated;
    }
}
