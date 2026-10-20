using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarInput : MonoBehaviour
{
    private void Update()
    {
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null || PhaseManager.IsPlayerLocked) return;
        if (MinigameManager.IsActive && !MinigameManager.IsPhoneOpen) return;

        GameSettings settings = GameSettings.Current;

        for (int i = 0; i < settings.hotbarKeys.Length && i < inventory.SlotCount; i++)
        {
            if (MinigameInput.KeyPressedThisFrame(settings.hotbarKeys[i]))
            {
                inventory.Select(i);
                return;
            }
        }

        if (!settings.hotbarScrollWheel || MinigameManager.IsPhoneOpen || Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) inventory.Select(inventory.SelectedIndex - 1);
        else if (scroll < 0f) inventory.Select(inventory.SelectedIndex + 1);
    }
}
