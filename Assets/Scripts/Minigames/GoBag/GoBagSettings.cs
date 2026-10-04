using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "GoBagSettings", menuName = "Go Bag/Settings")]
public class GoBagSettings : ScriptableObject
{
    [Header("Bag")]
    [Min(1)] public int gridWidth = 4;
    [Min(1)] public int gridHeight = 4;
    [Min(1f)] public float cellSize = 100f;
    [Min(0f)] public float cellGap = 4f;

    [Header("Controls")]
    public Key rotateKey = Key.R;

    [Header("Colors")]
    public Color emptyCellColor = new Color(1f, 1f, 1f, 0.15f);
    public Color itemColor = new Color(0.85f, 0.7f, 0.4f, 1f);
    public Color fitsColor = new Color(0.3f, 0.9f, 0.3f, 0.6f);
    public Color blockedColor = new Color(0.9f, 0.25f, 0.25f, 0.6f);

    [Header("Item label")]
    public Color labelColor = Color.black;
    [Min(1f)] public float labelMinFontSize = 10f;
    [Min(1f)] public float labelMaxFontSize = 24f;
    [Min(0f)] public float labelPadding = 4f;

    [Header("Pile tile label")]
    [TextArea] public string tileLabelFormat = "{0}\n({1}x{2})";

    [Header("Transparency")]
    [Range(0.1f, 1f)] public float heldItemAlpha = 0.6f;
    [Range(0f, 1f)] public float emptyTileAlpha = 0.3f;
}
