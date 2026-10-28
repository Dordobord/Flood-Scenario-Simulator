using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainHouseCopier : EditorWindow
{
    [System.Serializable]
    public class CopyEntry
    {
        public string sourcePath;
        public string targetParent;
    }

    struct Link
    {
        public string ownerPath;
        public string ownerType;
        public int ownerIndex;
        public string property;
        public string targetPath;
        public string targetType;
        public int targetIndex;
    }

    [Header("Source")]
    public SceneAsset sourceScene;

    [Header("House Prefab")]
    public GameObject housePrefab;
    public List<string> removeInTargets = new List<string>();

    [Header("What To Copy")]
    public List<CopyEntry> entries = new List<CopyEntry>();

    [Header("Target Scenes")]
    public List<SceneAsset> targetScenes = new List<SceneAsset>();

    [Header("Clean Up Source")]
    public List<string> deletePaths = new List<string>();
    public List<CopyEntry> movePaths = new List<CopyEntry>();
    public List<string> rootOrder = new List<string>();

    [Header("Backup")]
    public string backupFolder = "SceneBackups";

    SerializedObject serializedWindow;
    Vector2 scroll;

    [MenuItem("Tools/Scenarios/Main House Copier")]
    static void Open()
    {
        GetWindow<MainHouseCopier>("Main House Copier");
    }

    void OnEnable()
    {
        FillDefaults();
        serializedWindow = new SerializedObject(this);
    }

    void FillDefaults()
    {
        if (sourceScene == null)
            sourceScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Scenarios/UrbanScenario.unity");

        if (housePrefab == null)
            housePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/House/Environment.prefab");

        if (removeInTargets.Count == 0)
        {
            removeInTargets.Add("Environment");
            removeInTargets.Add("Neighborhood/StreetEdges/FrontYard");
        }

        if (entries.Count == 0)
        {
            entries.Add(new CopyEntry { sourcePath = "Tasks_Debug_Minigames", targetParent = "" });
            entries.Add(new CopyEntry { sourcePath = "Neighborhood/Ground/Frontyard_Flooring", targetParent = "Neighborhood/Ground" });
        }

        if (deletePaths.Count == 0)
        {
            deletePaths.AddRange(new[]
            {
                "Neighborhood/Fences",
                "Neighborhood/Barricades",
                "Neighborhood/Landmarks",
                "Neighborhood/EvacuationCenter/BarangayHall",
                "Neighborhood/Water/River",
                "Neighborhood/Water/Label_RIVER",
                "Neighborhood/Houses",
                "Neighborhood/Sidewalks",
                "Neighborhood/Roads",
                "Neighborhood/Ground",
                "Neighborhood/StreetEdges/Backdrops",
                "Neighborhood/StreetEdges/Riverbank",
                "Neighborhood/StreetEdges/RoadBlocks/Join_Riverbank_Wall",
                "Neighborhood/StreetEdges/RoadBlocks/Join_Riverbank_Wall (1)",
                "Houses/FilledHouses/Column_Gate_R",
                "Roads/RoadTiles/Complete_Road_Straight",
                "Roads/Edge_Road",
                "PSX Graphic",
                "Neighborhood/Water/StreetFlood/StreetFlood_HomeBlock_1",
                "Neighborhood/Water/StreetFlood/StreetFlood_HomeBlock_2",
                "Neighborhood/Water/StreetFlood/StreetFlood_Town_1",
                "Neighborhood/Water/StreetFlood/StreetFlood_Town_2",
                "Neighborhood/Water/StreetFlood/StreetFlood_Town_3",
            });
        }

        if (movePaths.Count == 0)
        {
            movePaths.Add(new CopyEntry { sourcePath = "Ground", targetParent = "Neighborhood" });
            movePaths.Add(new CopyEntry { sourcePath = "Roads", targetParent = "Neighborhood" });
            movePaths.Add(new CopyEntry { sourcePath = "Houses", targetParent = "Neighborhood" });
            movePaths.Add(new CopyEntry { sourcePath = "Street Props", targetParent = "Neighborhood" });
            movePaths.Add(new CopyEntry { sourcePath = "Hazards", targetParent = "Neighborhood" });
        }

        if (rootOrder.Count == 0)
        {
            rootOrder.AddRange(new[]
            {
                "Lighting", "EventSystem", "GameManager", "Player", "WeatherSystem", "UI",
                "Environment", "Tasks_Debug_Minigames", "Neighborhood",
            });
        }

        if (targetScenes.Count == 0 && sourceScene != null)
            FillTargets();
    }

    void FillTargets()
    {
        string sourcePath = AssetDatabase.GetAssetPath(sourceScene);
        string folder = Path.GetDirectoryName(sourcePath).Replace('\\', '/');
        targetScenes.Clear();
        foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { folder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path != sourcePath)
                targetScenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(path));
        }
    }

    void OnGUI()
    {
        serializedWindow.Update();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("sourceScene"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("housePrefab"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("removeInTargets"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("entries"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("targetScenes"), true);
        if (GUILayout.Button("Refill Targets From Source Folder"))
            FillTargets();
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("deletePaths"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("movePaths"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("rootOrder"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("backupFolder"));
        EditorGUILayout.EndScrollView();
        serializedWindow.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Reset Lists To Defaults"))
        {
            entries.Clear();
            removeInTargets.Clear();
            housePrefab = null;
            deletePaths.Clear();
            movePaths.Clear();
            rootOrder.Clear();
            targetScenes.Clear();
            FillDefaults();
            serializedWindow = new SerializedObject(this);
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Clean Up Source Scene", GUILayout.Height(28)))
            CleanUpSource();
        if (GUILayout.Button("Copy To Target Scenes", GUILayout.Height(32)))
            CopyToTargets();
    }

    void CleanUpSource()
    {
        if (sourceScene == null)
            return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        string list = "";
        foreach (string path in deletePaths)
            list += "\n- " + path;
        if (!EditorUtility.DisplayDialog("Clean up " + sourceScene.name,
                "This deletes:" + list + "\n\nmoves " + movePaths.Count + " group(s) into a new parent and sorts the root objects. " +
                sourceScene.name + " is backed up to " + backupFolder + " first, then saved.",
                "Clean Up", "Cancel"))
            return;

        string sourcePath = AssetDatabase.GetAssetPath(sourceScene);
        string stamp = System.DateTime.Now.ToString("MMdd_HHmm");
        Backup(sourcePath, stamp);
        Scene scene = EditorSceneManager.OpenScene(sourcePath, OpenSceneMode.Single);

        int deleted = 0;
        foreach (string path in deletePaths)
        {
            Transform found = FindPath(scene, path);
            if (found == null)
            {
                Debug.LogWarning("MainHouseCopier: " + path + " not found, skipped.");
                continue;
            }
            DestroyImmediate(found.gameObject);
            deleted++;
        }

        int moved = 0;
        foreach (CopyEntry entry in movePaths)
        {
            Transform found = FindPath(scene, entry.sourcePath);
            Transform parent = MakePath(scene, entry.targetParent);
            if (found == null || parent == null)
            {
                Debug.LogWarning("MainHouseCopier: could not move " + entry.sourcePath + ", skipped.");
                continue;
            }
            found.SetParent(parent, true);
            moved++;
        }

        for (int i = 0; i < rootOrder.Count; i++)
        {
            GameObject root = FindRoot(scene, rootOrder[i]);
            if (root != null)
                root.transform.SetSiblingIndex(i);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("MainHouseCopier: deleted " + deleted + ", moved " + moved + ". Backup is in " + backupFolder + "/" + stamp);
    }

    void CopyToTargets()
    {
        if (sourceScene == null || targetScenes.Count == 0)
        {
            Debug.LogError("MainHouseCopier: set a source scene and at least one target scene.");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        string sourcePath = AssetDatabase.GetAssetPath(sourceScene);
        string names = "";
        foreach (SceneAsset scene in targetScenes)
        {
            if (scene != null)
                names += "\n- " + scene.name;
        }
        if (!EditorUtility.DisplayDialog("Copy main house",
                "This removes " + string.Join(", ", removeInTargets) + " in the target scenes and places " +
                (housePrefab != null ? housePrefab.name + ".prefab" : "no prefab") + " instead, then copies these from " + sourceScene.name + ":" + EntryList() +
                "\n\nTargets:" + names + "\n\nNothing else in those scenes is touched. " + sourceScene.name +
                " is not saved. Each target is backed up to " + backupFolder + " first.",
                "Copy", "Cancel"))
            return;

        Scene source = EditorSceneManager.OpenScene(sourcePath, OpenSceneMode.Single);
        foreach (CopyEntry entry in entries)
        {
            if (FindPath(source, entry.sourcePath) == null)
            {
                Debug.LogError("MainHouseCopier: " + sourceScene.name + " has no " + entry.sourcePath + ". Nothing was changed.");
                return;
            }
        }

        string stamp = System.DateTime.Now.ToString("MMdd_HHmm");
        foreach (SceneAsset sceneAsset in targetScenes)
        {
            if (sceneAsset == null)
                continue;
            string path = AssetDatabase.GetAssetPath(sceneAsset);
            if (path == sourcePath)
                continue;

            Backup(path, stamp);
            Scene target = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            CopyInto(source, target);
            EditorSceneManager.SaveScene(target);
            EditorSceneManager.CloseScene(target, true);
        }

        EditorSceneManager.OpenScene(sourcePath, OpenSceneMode.Single);
        Debug.Log("MainHouseCopier: done. Backups are in " + backupFolder + "/" + stamp);
    }

    string EntryList()
    {
        string list = "";
        foreach (CopyEntry entry in entries)
            list += "\n- " + entry.sourcePath;
        return list;
    }

    void Backup(string scenePath, string stamp)
    {
        string folder = Path.Combine(backupFolder, stamp);
        Directory.CreateDirectory(folder);
        File.Copy(scenePath, Path.Combine(folder, Path.GetFileName(scenePath)), true);
    }

    void CopyInto(Scene source, Scene target)
    {
        List<GameObject> olds = new List<GameObject>();
        foreach (string path in removeInTargets)
        {
            Transform old = FindPath(target, path);
            if (old != null)
                olds.Add(old.gameObject);
        }
        foreach (CopyEntry entry in entries)
        {
            Transform old = FindPath(target, TargetPath(entry));
            if (old != null)
                olds.Add(old.gameObject);
        }

        List<Link> links = new List<Link>();
        foreach (GameObject root in target.GetRootGameObjects())
        {
            foreach (MonoBehaviour owner in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (owner != null && !IsInside(owner.transform, olds))
                    RecordLinks(owner, olds, links);
            }
        }

        int prefabIndex = -1;
        foreach (string path in removeInTargets)
        {
            Transform old = FindPath(target, path);
            if (old == null)
                continue;
            if (old.parent == null && prefabIndex < 0)
                prefabIndex = old.GetSiblingIndex();
            DestroyImmediate(old.gameObject);
        }

        int missing = 0;
        if (housePrefab != null)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(housePrefab, target);
            GameObject sourceInstance = FindRoot(source, housePrefab.name);
            if (sourceInstance != null)
            {
                instance.transform.SetPositionAndRotation(sourceInstance.transform.position, sourceInstance.transform.rotation);
                instance.transform.localScale = sourceInstance.transform.localScale;
                missing += CopySceneLinks(sourceInstance.transform, instance.transform, source, target);
            }
            if (prefabIndex >= 0)
                instance.transform.SetSiblingIndex(prefabIndex);
        }

        List<GameObject> copies = new List<GameObject>();
        foreach (CopyEntry entry in entries)
        {
            Transform original = FindPath(source, entry.sourcePath);
            Transform old = FindPath(target, TargetPath(entry));
            int index = old != null ? old.GetSiblingIndex() : -1;
            if (old != null)
                DestroyImmediate(old.gameObject);

            GameObject copy = Duplicate(original.gameObject);
            copy.name = original.name;
            copy.transform.SetParent(null, true);
            SceneManager.MoveGameObjectToScene(copy, target);

            Transform parent = MakePath(target, entry.targetParent);
            if (parent != null)
                copy.transform.SetParent(parent, true);
            if (index >= 0)
                copy.transform.SetSiblingIndex(index);
            copies.Add(copy);
        }

        foreach (GameObject copy in copies)
        {
            foreach (MonoBehaviour owner in copy.GetComponentsInChildren<MonoBehaviour>(true))
                missing += Repoint(owner, source, target);
        }

        foreach (Link link in links)
        {
            Component owner = FindComponent(target, link.ownerPath, link.ownerType, link.ownerIndex);
            if (owner == null)
                continue;
            Object value = FindObject(target, link.targetPath, link.targetType, link.targetIndex);
            if (value == null)
            {
                Debug.LogWarning(target.name + ": " + link.ownerPath + " (" + link.ownerType + ")." + link.property +
                                 " pointed at " + link.targetPath + ", which the copy does not have. Set it by hand.");
                missing++;
                continue;
            }
            SerializedObject serialized = new SerializedObject(owner);
            SerializedProperty property = serialized.FindProperty(link.property);
            if (property == null)
                continue;
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorSceneManager.MarkSceneDirty(target);
        Debug.Log(target.name + ": placed the house prefab and copied the tasks" +
                  (missing > 0 ? ", " + missing + " field(s) need a look (see warnings)." : "."));
    }

    int CopySceneLinks(Transform sourceRoot, Transform targetRoot, Scene source, Scene target)
    {
        int missing = 0;
        foreach (MonoBehaviour sourceOwner in sourceRoot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (sourceOwner == null)
                continue;
            string relative = PathOf(sourceOwner.transform).Substring(PathOf(sourceRoot).Length);
            Transform targetTransform = relative.Length == 0 ? targetRoot : targetRoot.Find(relative.Substring(1));
            if (targetTransform == null)
                continue;
            Component targetOwner = ComponentOn(targetTransform, sourceOwner.GetType().FullName, IndexOf(sourceOwner));
            if (targetOwner == null)
                continue;

            SerializedObject sourceSerialized = new SerializedObject(sourceOwner);
            SerializedObject targetSerialized = new SerializedObject(targetOwner);
            SerializedProperty property = sourceSerialized.GetIterator();
            bool changed = false;
            while (property.Next(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference || property.name == "m_Script")
                    continue;
                Object value = property.objectReferenceValue;
                Transform t = TransformOf(value);
                if (t == null || t.gameObject.scene != source || t.IsChildOf(sourceRoot))
                    continue;

                Object match = FindObject(target, PathOf(t), value.GetType().FullName, value is Component c ? IndexOf(c) : 0);
                if (match == null)
                {
                    Debug.LogWarning(target.name + ": " + PathOf(targetTransform) + " (" + sourceOwner.GetType().Name + ")." +
                                     property.propertyPath + " points at " + PathOf(t) + ", which this scene does not have. Set it by hand.");
                    missing++;
                    continue;
                }
                SerializedProperty targetProperty = targetSerialized.FindProperty(property.propertyPath);
                if (targetProperty == null)
                    continue;
                targetProperty.objectReferenceValue = match;
                changed = true;
            }
            if (changed)
                targetSerialized.ApplyModifiedPropertiesWithoutUndo();
        }
        return missing;
    }

    string TargetPath(CopyEntry entry)
    {
        string name = entry.sourcePath.Substring(entry.sourcePath.LastIndexOf('/') + 1);
        return string.IsNullOrEmpty(entry.targetParent) ? name : entry.targetParent + "/" + name;
    }

    static GameObject Duplicate(GameObject original)
    {
        Selection.objects = new Object[] { original };
        Unsupported.DuplicateGameObjectsUsingPasteboard();
        GameObject copy = Selection.activeGameObject;
        Selection.objects = new Object[0];
        return copy;
    }

    static bool IsInside(Transform t, List<GameObject> olds)
    {
        foreach (GameObject old in olds)
        {
            if (t.IsChildOf(old.transform))
                return true;
        }
        return false;
    }

    static void RecordLinks(MonoBehaviour owner, List<GameObject> olds, List<Link> links)
    {
        SerializedProperty property = new SerializedObject(owner).GetIterator();
        while (property.Next(true))
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference || property.name == "m_Script")
                continue;
            Transform target = TransformOf(property.objectReferenceValue);
            if (target == null || !IsInside(target, olds))
                continue;

            links.Add(new Link
            {
                ownerPath = PathOf(owner.transform),
                ownerType = owner.GetType().FullName,
                ownerIndex = IndexOf(owner),
                property = property.propertyPath,
                targetPath = PathOf(target),
                targetType = property.objectReferenceValue.GetType().FullName,
                targetIndex = property.objectReferenceValue is Component c ? IndexOf(c) : 0,
            });
        }
    }

    static int Repoint(MonoBehaviour owner, Scene source, Scene target)
    {
        if (owner == null)
            return 0;
        int missing = 0;
        SerializedObject serialized = new SerializedObject(owner);
        SerializedProperty property = serialized.GetIterator();
        while (property.Next(true))
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference || property.name == "m_Script")
                continue;
            Object value = property.objectReferenceValue;
            Transform t = TransformOf(value);
            if (t == null || t.gameObject.scene != source)
                continue;

            Object match = FindObject(target, PathOf(t), value.GetType().FullName, value is Component c ? IndexOf(c) : 0);
            if (match == null)
            {
                Debug.LogWarning(target.name + ": " + PathOf(owner.transform) + " (" + owner.GetType().Name + ")." +
                                 property.propertyPath + " points at " + PathOf(t) + ", which this scene does not have. Set it by hand.");
                missing++;
            }
            property.objectReferenceValue = match;
        }
        serialized.ApplyModifiedPropertiesWithoutUndo();
        return missing;
    }

    static Transform TransformOf(Object value)
    {
        if (value is GameObject go)
            return go.transform;
        if (value is Component component)
            return component.transform;
        return null;
    }

    static string PathOf(Transform t)
    {
        string path = t.name;
        for (Transform p = t.parent; p != null; p = p.parent)
            path = p.name + "/" + path;
        return path;
    }

    static int IndexOf(Component component)
    {
        Component[] same = component.GetComponents(component.GetType());
        return System.Array.IndexOf(same, component);
    }

    static GameObject FindRoot(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name)
                return root;
        }
        return null;
    }

    static Transform FindPath(Scene scene, string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        int slash = path.IndexOf('/');
        GameObject root = FindRoot(scene, slash < 0 ? path : path.Substring(0, slash));
        if (root == null)
            return null;
        return slash < 0 ? root.transform : root.transform.Find(path.Substring(slash + 1));
    }

    static Transform MakePath(Scene scene, string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        Transform current = null;
        foreach (string part in path.Split('/'))
        {
            Transform next = current == null ? FindRoot(scene, part)?.transform : current.Find(part);
            if (next == null)
            {
                GameObject made = new GameObject(part);
                SceneManager.MoveGameObjectToScene(made, scene);
                if (current != null)
                    made.transform.SetParent(current, false);
                next = made.transform;
            }
            current = next;
        }
        return current;
    }

    static Object FindObject(Scene scene, string path, string type, int index)
    {
        Transform t = FindPath(scene, path);
        if (t == null)
            return null;
        if (type == typeof(GameObject).FullName)
            return t.gameObject;
        return ComponentOn(t, type, index);
    }

    static Component FindComponent(Scene scene, string path, string type, int index)
    {
        Transform t = FindPath(scene, path);
        return t != null ? ComponentOn(t, type, index) : null;
    }

    static Component ComponentOn(Transform t, string type, int index)
    {
        int seen = 0;
        foreach (Component component in t.GetComponents<Component>())
        {
            if (component == null || component.GetType().FullName != type)
                continue;
            if (seen++ == index)
                return component;
        }
        return null;
    }
}
