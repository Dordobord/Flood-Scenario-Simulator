using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormWaits
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text label;

        
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
    }
}
