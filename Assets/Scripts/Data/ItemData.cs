using UnityEngine;

namespace StormWaits
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Storm Waits/Item")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string displayName = "New Item";
        [TextArea] public string description;

        [Header("HUD")]
        public Sprite icon;
        [Tooltip("Shown in the hotbar when there is no icon.")]
        public Color fallbackColor = Color.white;

        [Header("Future use")]
        [Tooltip("Not used yet. Reserved for the purchasing feature.")]
        public int cost;
        [Tooltip("The HHI indicator this item stands for (reference only).")]
        public string hhiIndicator;
    }
}
