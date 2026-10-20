using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;

    [Header("Selection")]
    [SerializeField] private Image background;
    [SerializeField] private Outline outline;
    [SerializeField] private TMP_Text numberText;

    public void SetItem(ItemData item)
    {
        if (item == null)
        {
            icon.enabled = false;
            label.text = "";
            return;
        }

        icon.enabled = true;

        if (item.icon != null)
        {
            icon.sprite = item.icon;
            icon.color = Color.white;
        }
        else
        {
            icon.sprite = null;
            icon.color = item.fallbackColor;
        }

        label.text = item.displayName;
    }

    public void SetNumber(int number)
    {
        if (numberText != null) numberText.text = number.ToString();
    }

    public void SetSelected(bool selected)
    {
        GameSettings settings = GameSettings.Current;

        if (background != null) background.color = selected ? settings.hotbarSelectedColor : settings.hotbarNormalColor;
        if (outline != null)
        {
            outline.enabled = selected;
            outline.effectColor = settings.hotbarOutlineColor;
        }
        transform.localScale = Vector3.one * (selected ? settings.hotbarSelectedScale : 1f);
    }
}
