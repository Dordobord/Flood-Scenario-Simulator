using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RiverWaterSetup : EditorWindow
{
    [System.Serializable]
    public class WaterSheet
    {
        public string name;
        public Vector2 center;
        public Vector2 size;
        public float height = 0.3f;
        public bool showFromStart = true;
    }

    [Header("Look")]
    public Material waterMaterial;

    [Header("Where")]
    public string parentPath = "Neighborhood/Water";
    public string groupName = "RiverFlood";

    [Header("Sheets")]
    public List<WaterSheet> sheets = new List<WaterSheet>();

    [Header("House")]
    public Vector2 houseCenter = new Vector2(0f, -1f);
    public Vector2 houseSize = new Vector2(6.3f, 12.3f);

    [Header("Evacuation Spot")]
    public Vector3 schoolSpot = new Vector3(108f, 0f, 63f);

    [Header("Low Street")]
    public Vector2 poolCenter = new Vector2(-140.5f, 15.5f);
    public Vector2 poolSize = new Vector2(46f, 25f);
    public float poolHeight = -6.9f;
    public string rampRoadName = "Complete_Road_Straight (5)";
    public float runoffLift = 0.15f;
    public float riseDepth = 0.8f;
    public float riseSeconds = 20f;

    [Header("Street Flood")]
    public Vector2 streetMin = new Vector2(-80f, -12f);
    public Vector2 streetMax = new Vector2(30.4f, 150f);
    public float streetHeight = 0.15f;
    public float houseMargin = 0.2f;

    [Header("Bridge")]
    public Vector2 bridgeCenter = new Vector2(58f, 63f);
    public Vector2 bridgeSize = new Vector2(54f, 16f);
    public float bridgeHeight = 0.15f;

    SerializedObject serializedWindow;
    Vector2 scroll;

    [MenuItem("Tools/Scenarios/River Water Setup")]
    static void Open()
    {
        GetWindow<RiverWaterSetup>("River Water Setup");
    }

    void OnEnable()
    {
        FillDefaults();
        serializedWindow = new SerializedObject(this);
    }

    void FillDefaults()
    {
        if (waterMaterial == null)
            waterMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Water/FloodWater_River.mat");

        if (sheets.Count == 0)
        {
            sheets.Add(new WaterSheet { name = "River_South", center = new Vector2(58f, 15f), size = new Vector2(54f, 80f), height = 0.25f });
            sheets.Add(new WaterSheet { name = "River_North", center = new Vector2(58f, 115.5f), size = new Vector2(54f, 89f), height = 0.25f });
            sheets.Add(new WaterSheet { name = "River_Zone1_South", center = new Vector2(21f, 27.5f), size = new Vector2(18f, 55f), height = 0.3f });
            sheets.Add(new WaterSheet { name = "River_Zone1_North", center = new Vector2(21f, 110.5f), size = new Vector2(18f, 79f), height = 0.3f });
            sheets.Add(new WaterSheet { name = "River_Zone2", center = new Vector2(-10f, 17.5f), size = new Vector2(44f, 21f), height = 0.3f, showFromStart = false });
        }
    }

    void OnGUI()
    {
        serializedWindow.Update();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.HelpBox("Adds water to the open scene. Nothing is deleted. Ctrl+Z removes it all, Ctrl+S keeps it.", MessageType.Info);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("waterMaterial"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("parentPath"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("groupName"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("sheets"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("houseCenter"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("houseSize"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("schoolSpot"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("poolCenter"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("poolSize"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("poolHeight"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("rampRoadName"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("runoffLift"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("riseDepth"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("riseSeconds"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("streetMin"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("streetMax"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("streetHeight"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("houseMargin"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("bridgeCenter"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("bridgeSize"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("bridgeHeight"));
        EditorGUILayout.EndScrollView();
        serializedWindow.ApplyModifiedProperties();

        if (GUILayout.Button("Reset Lists To Defaults"))
        {
            sheets.Clear();
            waterMaterial = null;
            FillDefaults();
            serializedWindow = new SerializedObject(this);
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Add Water To Open Scene", GUILayout.Height(32)))
            Build(SceneManager.GetActiveScene());
        if (GUILayout.Button("Add Low Street Water", GUILayout.Height(32)))
            AddLowStreet(SceneManager.GetActiveScene());
        if (GUILayout.Button("Add Street Flood", GUILayout.Height(32)))
            AddStreetFlood(SceneManager.GetActiveScene());
        if (GUILayout.Button("Add Bridge Water", GUILayout.Height(32)))
            AddBridge(SceneManager.GetActiveScene());
    }

    void AddBridge(Scene scene)
    {
        Transform group = MakePath(scene, parentPath + "/" + groupName);
        if (group.Find("River_Bridge") != null)
        {
            Debug.LogWarning("RiverWaterSetup: River_Bridge is already in " + scene.name + ". Delete it first to add it again.");
            return;
        }

        Undo.SetCurrentGroupName("Bridge Water");
        int undoGroup = Undo.GetCurrentGroup();
        GameObject bridge = MakeSheet("River_Bridge", bridgeCenter, bridgeSize, bridgeHeight, group);
        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = bridge;
        Debug.Log("RiverWaterSetup: added River_Bridge to " + scene.name + ".");
    }

    void AddStreetFlood(Scene scene)
    {
        Transform group = MakePath(scene, parentPath + "/" + groupName);
        if (group.Find("StreetFlood_West") != null)
        {
            Debug.LogWarning("RiverWaterSetup: StreetFlood is already in " + scene.name + ". Delete the StreetFlood sheets first to add them again.");
            return;
        }

        Undo.SetCurrentGroupName("Street Flood");
        int undoGroup = Undo.GetCurrentGroup();

        Vector2 holeMin = houseCenter - houseSize / 2f - Vector2.one * houseMargin;
        Vector2 holeMax = houseCenter + houseSize / 2f + Vector2.one * houseMargin;

        List<GameObject> added = new List<GameObject>();
        added.Add(MakeArea("StreetFlood_West", new Vector2(streetMin.x, streetMin.y), new Vector2(holeMin.x, streetMax.y), group));
        added.Add(MakeArea("StreetFlood_East", new Vector2(holeMax.x, streetMin.y), new Vector2(streetMax.x, streetMax.y), group));
        added.Add(MakeArea("StreetFlood_South", new Vector2(holeMin.x, streetMin.y), new Vector2(holeMax.x, holeMin.y), group));
        added.Add(MakeArea("StreetFlood_North", new Vector2(holeMin.x, holeMax.y), new Vector2(holeMax.x, streetMax.y), group));

        FloodWaterPhase phase = group.GetComponent<FloodWaterPhase>();
        if (phase == null)
            phase = Undo.AddComponent<FloodWaterPhase>(group.gameObject);
        SerializedObject serializedPhase = new SerializedObject(phase);
        SerializedProperty list = serializedPhase.FindProperty("waterObjects");
        foreach (GameObject water in added)
        {
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = water;
        }
        serializedPhase.ApplyModifiedProperties();

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = added[0];
        Debug.Log("RiverWaterSetup: added 4 StreetFlood sheets around the main house to " + scene.name + ".");
    }

    GameObject MakeArea(string sheetName, Vector2 min, Vector2 max, Transform parent)
    {
        return MakeSheet(sheetName, (min + max) / 2f, max - min, streetHeight, parent);
    }

    void AddLowStreet(Scene scene)
    {
        Transform group = MakePath(scene, parentPath + "/" + groupName);
        if (group.Find("LowStreet_Pool") != null)
        {
            Debug.LogWarning("RiverWaterSetup: LowStreet_Pool is already in " + scene.name + ". Delete it first to add it again.");
            return;
        }

        Undo.SetCurrentGroupName("Low Street Water");
        int undoGroup = Undo.GetCurrentGroup();

        List<GameObject> added = new List<GameObject>();
        added.Add(MakeSheet("LowStreet_Pool", poolCenter, poolSize, poolHeight, group));

        Transform ramp = FindRamp(scene);
        if (ramp != null)
            added.Add(MakeRunoff(ramp, group));

        FloodWaterPhase phase = group.GetComponent<FloodWaterPhase>();
        if (phase == null)
            phase = Undo.AddComponent<FloodWaterPhase>(group.gameObject);
        SerializedObject serializedPhase = new SerializedObject(phase);
        SerializedProperty list = serializedPhase.FindProperty("waterObjects");
        foreach (GameObject water in added)
        {
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = water;
        }
        serializedPhase.FindProperty("riseDepth").floatValue = riseDepth;
        serializedPhase.FindProperty("riseSeconds").floatValue = riseSeconds;
        serializedPhase.ApplyModifiedProperties();

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = added[0];
        Debug.Log("RiverWaterSetup: added LowStreet_Pool" + (ramp != null ? " and LowStreet_Runoff" : "") + " to " + scene.name + "." +
                  (ramp == null ? " No sloped road named " + rampRoadName + " found, so no runoff sheet." : ""));
    }

    Transform FindRamp(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == rampRoadName && Vector3.Angle(child.up, Vector3.up) > 1f)
                    return child;
            }
        }
        return null;
    }

    GameObject MakeRunoff(Transform ramp, Transform parent)
    {
        bool found = false;
        Bounds local = new Bounds();
        foreach (MeshFilter filter in ramp.GetComponentsInChildren<MeshFilter>())
        {
            if (filter.sharedMesh == null) continue;
            Bounds meshBounds = filter.sharedMesh.bounds;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = meshBounds.center + Vector3.Scale(meshBounds.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                Vector3 point = ramp.InverseTransformPoint(filter.transform.TransformPoint(corner));
                if (!found) { local = new Bounds(point, Vector3.zero); found = true; }
                else local.Encapsulate(point);
            }
        }

        GameObject sheet = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Undo.RegisterCreatedObjectUndo(sheet, "Low Street Water");
        sheet.name = "LowStreet_Runoff";
        DestroyImmediate(sheet.GetComponent<Collider>());
        sheet.transform.SetParent(parent, false);
        sheet.transform.position = ramp.TransformPoint(new Vector3(local.center.x, local.max.y, local.center.z)) + ramp.up * runoffLift;
        sheet.transform.rotation = ramp.rotation;
        Vector3 scale = ramp.lossyScale;
        sheet.transform.localScale = new Vector3(local.size.x * scale.x / 10f, 1f, local.size.z * scale.z / 10f);
        if (waterMaterial != null)
            sheet.GetComponent<MeshRenderer>().sharedMaterial = waterMaterial;
        return sheet;
    }

    void Build(Scene scene)
    {
        Undo.SetCurrentGroupName("River Water Setup");
        int undoGroup = Undo.GetCurrentGroup();

        Transform parent = MakePath(scene, parentPath);
        GameObject group = new GameObject(groupName);
        Undo.RegisterCreatedObjectUndo(group, "River Water Setup");
        SceneManager.MoveGameObjectToScene(group, scene);
        group.transform.SetParent(parent, false);

        List<GameObject> laterWater = new List<GameObject>();
        foreach (WaterSheet sheet in sheets)
        {
            GameObject made = MakeSheet(sheet.name, sheet.center, sheet.size, sheet.height, group.transform);
            if (!sheet.showFromStart)
                laterWater.Add(made);
        }

        GameObject houseWater = MakeSheet("HouseFlood", houseCenter, houseSize, 0f, group.transform);

        FloodWaterPhase phase = Undo.AddComponent<FloodWaterPhase>(group);
        SerializedObject serializedPhase = new SerializedObject(phase);
        SerializedProperty list = serializedPhase.FindProperty("waterObjects");
        list.arraySize = laterWater.Count;
        for (int i = 0; i < laterWater.Count; i++)
            list.GetArrayElementAtIndex(i).objectReferenceValue = laterWater[i];
        serializedPhase.FindProperty("houseWater").objectReferenceValue = houseWater.transform;
        SandbagWall wall = Object.FindFirstObjectByType<SandbagWall>(FindObjectsInactive.Include);
        if (wall != null && wall.gameObject.scene == scene)
            serializedPhase.FindProperty("sandbagWall").objectReferenceValue = wall;
        serializedPhase.ApplyModifiedPropertiesWithoutUndo();

        GameObject school = new GameObject("School_Spot");
        Undo.RegisterCreatedObjectUndo(school, "River Water Setup");
        school.transform.SetParent(MakePath(scene, "Neighborhood/EvacuationCenter"), false);
        school.transform.position = schoolSpot;

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = group;
        Debug.Log("RiverWaterSetup: added " + sheets.Count + " water sheet(s), HouseFlood and School_Spot to " + scene.name + "." +
                  (wall == null ? " No SandbagWall found, set it on FloodWaterPhase by hand." : ""));
    }

    GameObject MakeSheet(string sheetName, Vector2 center, Vector2 size, float height, Transform parent)
    {
        GameObject sheet = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Undo.RegisterCreatedObjectUndo(sheet, "River Water Setup");
        sheet.name = sheetName;
        DestroyImmediate(sheet.GetComponent<Collider>());
        sheet.transform.SetParent(parent, false);
        sheet.transform.position = new Vector3(center.x, height, center.y);
        sheet.transform.localScale = new Vector3(size.x / 10f, 1f, size.y / 10f);
        if (waterMaterial != null)
            sheet.GetComponent<MeshRenderer>().sharedMaterial = waterMaterial;
        return sheet;
    }

    static Transform MakePath(Scene scene, string path)
    {
        Transform current = null;
        foreach (string part in path.Split('/'))
        {
            Transform next = null;
            if (current == null)
            {
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    if (root.name == part)
                        next = root.transform;
                }
            }
            else
            {
                next = current.Find(part);
            }

            if (next == null)
            {
                GameObject made = new GameObject(part);
                Undo.RegisterCreatedObjectUndo(made, "River Water Setup");
                SceneManager.MoveGameObjectToScene(made, scene);
                if (current != null)
                    made.transform.SetParent(current, false);
                next = made.transform;
            }
            current = next;
        }
        return current;
    }
}
