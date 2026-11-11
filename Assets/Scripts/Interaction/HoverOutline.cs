using UnityEngine;

public class HoverOutline : MonoBehaviour
{
    private MeshRenderer[] parts;
    private int[] layers;
    private bool shown;

    public void SetShown(bool show)
    {
        if (show == shown) return;
        shown = show;

        if (parts == null)
        {
            parts = GetComponentsInChildren<MeshRenderer>(true);
            layers = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++) layers[i] = parts[i].gameObject.layer;
        }

        int outlineLayer = LayerMask.NameToLayer("Outline");

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null) parts[i].gameObject.layer = show ? outlineLayer : layers[i];
        }
    }

    private void OnDisable()
    {
        SetShown(false);
    }
}