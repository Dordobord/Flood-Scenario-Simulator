using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoastalWaterSetup : EditorWindow
{
    [Header("Look")]
    public Material waterMaterial;

    [Header("Where")]
    public string parentPath = "Neighborhood/Water";
    public string groupName = "CoastalFlood";

    [Header("House")]
    public Vector2 houseCenter = new Vector2(0f, -1f);
    public Vector2 houseSize = new Vector2(6.3f, 12.3f);
    public float houseMargin = 0.2f;

    [Header("Street Flood")]
    public Vector2 streetMin = new Vector2(-130f, -15f);
    public Vector2 streetMax = new Vector2(80f, 55f);
    public float streetHeight = 0.3f;

    [Header("Sea")]
    public Vector2 seaCenter = new Vector2(0f, -370f);
    public Vector2 seaSize = new Vector2(800f, 670f);
    public float normalSeaHeight = -16.5f;
    public float surgeSeaHeight = -14.5f;

    [Header("Slope Water")]
    public List<string> slopeRoadNames = new List<string>();
    public float runoffLift = 0.15f;

    [Header("Spawn Point")]
    public string spawnParentPath = "Neighborhood";
    public string spawnName = "EvacuationSpawnPoint";
    public Vector3 spawnPosition = new Vector3(0f, 0.05f, 8.5f);

    [Header("Alert")]
    public string alertName = "HazardMapAlert";

    SerializedObject serializedWindow;
    Vector2 scroll;

    [MenuItem("Tools/Scenarios/Coastal Water Setup")]
    static void Open()
    {
        GetWindow<CoastalWaterSetup>("Coastal Water Setup");
    }

    void OnEnable()
    {
        FillDefaults();
        serializedWindow = new SerializedObject(this);
    }

    void FillDefaults()
    {
        if (waterMaterial == null)
            waterMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Water/FloodWater_Sea.mat");

        if (slopeRoadNames.Count == 0)
        {
            slopeRoadNames.Add("Complete_Road_Straight (3)");
            slopeRoadNames.Add("Complete_Road_Straight (4)");
        }
    }

    void OnGUI()
    {
        serializedWindow.Update();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.HelpBox("Works on the open scene. Nothing is deleted. Ctrl+Z removes it, Ctrl+S keeps it.", MessageType.Info);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("waterMaterial"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("parentPath"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("groupName"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("houseCenter"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("houseSize"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("houseMargin"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("streetMin"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("streetMax"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("streetHeight"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("seaCenter"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("seaSize"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("normalSeaHeight"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("surgeSeaHeight"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("slopeRoadNames"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("runoffLift"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("spawnParentPath"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("spawnName"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("spawnPosition"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("alertName"));
        EditorGUILayout.EndScrollView();
        serializedWindow.ApplyModifiedProperties();

        if (GUILayout.Button("Add Water To Open Scene", GUILayout.Height(32)))
            AddWater(SceneManager.GetActiveScene());
        if (GUILayout.Button("Add Slope Water", GUILayout.Height(32)))
            AddSlopeWater(SceneManager.GetActiveScene());
        if (GUILayout.Button("Add Spawn Point And Turn On Alert", GUILayout.Height(32)))
            AddSpawnPoint(SceneManager.GetActiveScene());
    }

    void AddWater(Scene scene)
    {
        Transform parent = MakePath(scene, parentPath);
        if (parent.Find(groupName) != null)
        {
            Debug.LogWarning("CoastalWaterSetup: " + groupName + " is already in " + scene.name + ". Delete it first to add it again.");
            return;
        }

        Undo.SetCurrentGroupName("Coastal Water Setup");
        int undoGroup = Undo.GetCurrentGroup();

        GameObject group = new GameObject(groupName);
        Undo.RegisterCreatedObjectUndo(group, "Coastal Water Setup");
        SceneManager.MoveGameObjectToScene(group, scene);
        group.transform.SetParent(parent, false);

        Vector2 holeMin = houseCenter - houseSize / 2f - Vector2.one * houseMargin;
        Vector2 holeMax = houseCenter + houseSize / 2f + Vector2.one * houseMargin;

        List<GameObject> laterWater = new List<GameObject>();
        laterWater.Add(MakeArea("StreetFlood_West", new Vector2(streetMin.x, streetMin.y), new Vector2(holeMin.x, streetMax.y), group.transform));
        laterWater.Add(MakeArea("StreetFlood_East", new Vector2(holeMax.x, streetMin.y), new Vector2(streetMax.x, streetMax.y), group.transform));
        laterWater.Add(MakeArea("StreetFlood_South", new Vector2(holeMin.x, streetMin.y), new Vector2(holeMax.x, holeMin.y), group.transform));
        laterWater.Add(MakeArea("StreetFlood_North", new Vector2(holeMin.x, holeMax.y), new Vector2(holeMax.x, streetMax.y), group.transform));
        laterWater.Add(MakeSheet("Sea_Surge", seaCenter, seaSize, surgeSeaHeight, group.transform));

        MakeSheet("Sea_Normal", seaCenter, seaSize, normalSeaHeight, group.transform);
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

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = group;
        Debug.Log("CoastalWaterSetup: added street flood, Sea_Normal, Sea_Surge and HouseFlood to " + scene.name + "." +
                  (wall == null ? " No SandbagWall found, set it on FloodWaterPhase by hand." : ""));
    }

    void AddSlopeWater(Scene scene)
    {
        Transform group = MakePath(scene, parentPath).Find(groupName);
        FloodWaterPhase phase = group != null ? group.GetComponent<FloodWaterPhase>() : null;
        if (phase == null)
        {
            Debug.LogWarning("CoastalWaterSetup: press Add Water To Open Scene first.");
            return;
        }

        Undo.SetCurrentGroupName("Coastal Slope Water");
        int undoGroup = Undo.GetCurrentGroup();

        List<GameObject> added = new List<GameObject>();
        List<string> missing = new List<string>();
        foreach (string roadName in slopeRoadNames)
        {
            string sheetName = "SlopeFlood_" + roadName;
            if (group.Find(sheetName) != null) continue;

            Transform road = FindSlope(scene, roadName);
            if (road == null)
            {
                missing.Add(roadName);
                continue;
            }
            GameObject sheet = MakeRunoff(road, group);
            sheet.name = sheetName;
            added.Add(sheet);
        }

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
        if (added.Count > 0)
            Selection.activeGameObject = added[0];
        Debug.Log("CoastalWaterSetup: added " + added.Count + " slope water sheet(s) to " + scene.name + "." +
                  (missing.Count > 0 ? " No sloped road named " + string.Join(", ", missing) + "." : ""));
    }

    Transform FindSlope(Scene scene, string roadName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == roadName && Vector3.Angle(child.up, Vector3.up) > 1f)
                    return child;
            }
        }
        return null;
    }

    GameObject MakeRunoff(Transform road, Transform parent)
    {
        bool found = false;
        Bounds local = new Bounds();
        foreach (MeshFilter filter in road.GetComponentsInChildren<MeshFilter>())
        {
            if (filter.sharedMesh == null) continue;
            Bounds meshBounds = filter.sharedMesh.bounds;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = meshBounds.center + Vector3.Scale(meshBounds.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                Vector3 point = road.InverseTransformPoint(filter.transform.TransformPoint(corner));
                if (!found) { local = new Bounds(point, Vector3.zero); found = true; }
                else local.Encapsulate(point);
            }
        }

        GameObject sheet = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Undo.RegisterCreatedObjectUndo(sheet, "Coastal Slope Water");
        DestroyImmediate(sheet.GetComponent<Collider>());
        sheet.transform.SetParent(parent, false);
        sheet.transform.position = road.TransformPoint(new Vector3(local.center.x, local.max.y, local.center.z)) + road.up * runoffLift;
        sheet.transform.rotation = road.rotation;
        Vector3 scale = road.lossyScale;
        sheet.transform.localScale = new Vector3(local.size.x * scale.x / 10f, 1f, local.size.z * scale.z / 10f);
        if (waterMaterial != null)
            sheet.GetComponent<MeshRenderer>().sharedMaterial = waterMaterial;
        return sheet;
    }

    void AddSpawnPoint(Scene scene)
    {
        Undo.SetCurrentGroupName("Coastal Spawn Point");
        int undoGroup = Undo.GetCurrentGroup();

        Transform parent = MakePath(scene, spawnParentPath);
        Transform spawn = parent.Find(spawnName);
        if (spawn == null)
        {
            GameObject made = new GameObject(spawnName);
            Undo.RegisterCreatedObjectUndo(made, "Coastal Spawn Point");
            made.transform.SetParent(parent, false);
            made.transform.position = spawnPosition;
            made.transform.rotation = Quaternion.identity;
            spawn = made.transform;
        }

        PhaseManager phaseManager = Object.FindFirstObjectByType<PhaseManager>(FindObjectsInactive.Include);
        bool linked = phaseManager != null && phaseManager.gameObject.scene == scene;
        if (linked)
        {
            SerializedObject serializedManager = new SerializedObject(phaseManager);
            serializedManager.FindProperty("evacuationSpawnPoint").objectReferenceValue = spawn;
            serializedManager.ApplyModifiedProperties();
        }

        bool alertOn = false;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name != alertName) continue;
            Undo.RecordObject(root, "Coastal Spawn Point");
            root.SetActive(true);
            alertOn = true;
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = spawn.gameObject;
        Debug.Log("CoastalWaterSetup: " + spawnName + " is ready in " + scene.name + "." +
                  (linked ? " Linked to PhaseManager." : " No PhaseManager found, link it by hand.") +
                  (alertOn ? " " + alertName + " turned on." : " No " + alertName + " found."));
    }

    GameObject MakeArea(string sheetName, Vector2 min, Vector2 max, Transform parent)
    {
        return MakeSheet(sheetName, (min + max) / 2f, max - min, streetHeight, parent);
    }

    GameObject MakeSheet(string sheetName, Vector2 center, Vector2 size, float height, Transform parent)
    {
        GameObject sheet = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Undo.RegisterCreatedObjectUndo(sheet, "Coastal Water Setup");
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
                Undo.RegisterCreatedObjectUndo(made, "Coastal Water Setup");
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
