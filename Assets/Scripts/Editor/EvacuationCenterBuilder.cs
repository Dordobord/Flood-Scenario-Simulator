using UnityEditor;
using UnityEngine;

public class EvacuationCenterBuilder : EditorWindow
{
    [Header("Parts")]
    public GameObject buildingModel;
    public GameObject markerPrefab;

    [Header("Save")]
    public string prefabPath = "Assets/Prefabs/Gameplay/EvacuationCenter.prefab";

    [Header("Trigger")]
    public float triggerPadding = 1f;
    public float triggerHeight = 4f;

    [Header("Marker")]
    public float markerAboveRoof = 3f;

    SerializedObject serializedWindow;

    [MenuItem("Tools/Scenarios/Evacuation Center Builder")]
    static void Open()
    {
        GetWindow<EvacuationCenterBuilder>("Evacuation Center Builder");
    }

    void OnEnable()
    {
        FillDefaults();
        serializedWindow = new SerializedObject(this);
    }

    void FillDefaults()
    {
        if (buildingModel == null)
            buildingModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Multipurpose_Building.fbx");

        if (markerPrefab == null)
            markerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/EvacuationMarker.prefab");
    }

    void OnGUI()
    {
        serializedWindow.Update();
        EditorGUILayout.HelpBox("Makes one prefab file. No scene is changed. Drag the prefab into a scene afterwards.", MessageType.Info);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("buildingModel"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("markerPrefab"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("prefabPath"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("triggerPadding"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("triggerHeight"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("markerAboveRoof"));
        serializedWindow.ApplyModifiedProperties();

        GUI.enabled = buildingModel != null && markerPrefab != null;
        if (GUILayout.Button("Build Prefab", GUILayout.Height(30)))
        {
            Build();
        }
        GUI.enabled = true;
    }

    void Build()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            bool replace = EditorUtility.DisplayDialog("Evacuation Center Builder",
                prefabPath + " already exists. Replacing it removes any changes made to that prefab.", "Replace", "Cancel");
            if (!replace) return;
        }

        GameObject root = new GameObject("EvacuationCenter");

        GameObject building = (GameObject)PrefabUtility.InstantiatePrefab(buildingModel);
        building.transform.SetParent(root.transform, false);

        Bounds bounds = GetBounds(building);
        Vector3 offset = new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z);
        building.transform.localPosition += offset;
        bounds.center += offset;

        GameObject point = new GameObject("EvacuationPoint");
        point.transform.SetParent(root.transform, false);
        BoxCollider box = point.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(bounds.size.x + triggerPadding * 2f, triggerHeight, bounds.size.z + triggerPadding * 2f);
        box.center = new Vector3(0f, triggerHeight / 2f, 0f);
        point.AddComponent<EvacuationPoint>();

        GameObject marker = (GameObject)PrefabUtility.InstantiatePrefab(markerPrefab);
        marker.transform.SetParent(root.transform, false);
        marker.transform.localPosition = new Vector3(0f, bounds.max.y + markerAboveRoof, 0f);
        marker.name = markerPrefab.name;

        GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        DestroyImmediate(root);

        if (saved != null)
        {
            Selection.activeObject = saved;
            EditorGUIUtility.PingObject(saved);
            Debug.Log("Evacuation Center Builder: saved " + prefabPath + ". Building size " + bounds.size + ".");
        }
    }

    Bounds GetBounds(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(target.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer item in renderers)
        {
            bounds.Encapsulate(item.bounds);
        }
        return bounds;
    }
}
