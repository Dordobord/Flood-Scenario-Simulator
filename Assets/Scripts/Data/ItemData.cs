using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Storm Waits/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string displayName = "New Item";
    [TextArea] public string description;

    [Header("HUD")]
    public Sprite icon;
    public Color fallbackColor = Color.white;

    [Header("Future use")]
    public int cost;
    public string hhiIndicator;
}
