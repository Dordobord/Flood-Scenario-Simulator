using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HoverOutline : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private MeshRenderer[] renderers;

    private readonly List<MeshRenderer> sources = new List<MeshRenderer>();
    private readonly List<MeshRenderer> outlines = new List<MeshRenderer>();
    private bool built;
    private bool shown;

    public void SetShown(bool show)
    {
        if (show == shown) return;
        shown = show;

        if (show && !built) Build();

        for (int i = 0; i < outlines.Count; i++)
        {
            if (outlines[i] != null) outlines[i].enabled = show && sources[i] != null && sources[i].enabled && sources[i].gameObject.activeInHierarchy;
        }
    }

    private void OnDisable()
    {
        SetShown(false);
    }

    private void Build()
    {
        built = true;

        Material material = GameSettings.Current.hoverOutlineMaterial;
        if (material == null)
        {
            Debug.LogWarning("HoverOutline: assign Hover Outline Material in GameSettings.");
            return;
        }

        MeshRenderer[] targets = renderers != null && renderers.Length > 0 ? renderers : GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer source in targets)
        {
            if (source == null || source.sharedMaterial == material) continue;

            if (source.isPartOfStaticBatch)
            {
                Debug.LogWarning("HoverOutline: untick Batching Static on " + source.name + " so it can be outlined.");
                continue;
            }

            MeshFilter filter = source.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) continue;

            GameObject copy = new GameObject(source.name + " Outline");
            copy.layer = source.gameObject.layer;
            copy.transform.SetParent(source.transform, false);

            copy.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;

            MeshRenderer outline = copy.AddComponent<MeshRenderer>();
            Material[] materials = new Material[filter.sharedMesh.subMeshCount];
            for (int i = 0; i < materials.Length; i++) materials[i] = material;
            outline.sharedMaterials = materials;
            outline.shadowCastingMode = ShadowCastingMode.Off;
            outline.receiveShadows = false;
            outline.lightProbeUsage = LightProbeUsage.Off;
            outline.reflectionProbeUsage = ReflectionProbeUsage.Off;
            outline.enabled = false;

            sources.Add(source);
            outlines.Add(outline);
        }
    }
}
