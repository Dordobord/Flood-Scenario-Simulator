using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HierarchyOrganizer : EditorWindow
{
    [System.Serializable]
    public class GroupRule
    {
        public string namePrefix;
        public string parentPath;
    }

    [Header("Grouping")]
    public List<GroupRule> rules = new List<GroupRule>();

    [Header("Root Order")]
    public List<string> rootOrder = new List<string>();

    [Header("Group Order")]
    public string orderedParent = "Neighborhood";
    public List<string> groupOrder = new List<string>();

    SerializedObject serializedWindow;
    Vector2 scroll;

    [MenuItem("Tools/Scenarios/Hierarchy Organizer")]
    static void Open()
    {
        GetWindow<HierarchyOrganizer>("Hierarchy Organizer");
    }

    void OnEnable()
    {
        FillDefaults();
        serializedWindow = new SerializedObject(this);
    }

    void FillDefaults()
    {
        if (rules.Count == 0)
        {
            rules.Add(new GroupRule { namePrefix = "House_Flooring", parentPath = "Neighborhood/Ground" });
            rules.Add(new GroupRule { namePrefix = "Frontyard_Flooring", parentPath = "Neighborhood/Ground" });
            rules.Add(new GroupRule { namePrefix = "Ground", parentPath = "Neighborhood/Ground" });
            rules.Add(new GroupRule { namePrefix = "Road", parentPath = "Neighborhood/Roads" });
            rules.Add(new GroupRule { namePrefix = "Complete_Road", parentPath = "Neighborhood/Roads" });
            rules.Add(new GroupRule { namePrefix = "Corner_Road", parentPath = "Neighborhood/Roads" });
            rules.Add(new GroupRule { namePrefix = "Edge_Road", parentPath = "Neighborhood/Roads" });
            rules.Add(new GroupRule { namePrefix = "House_", parentPath = "Neighborhood/Houses" });
            rules.Add(new GroupRule { namePrefix = "SlopeCover", parentPath = "Neighborhood/Houses" });
            rules.Add(new GroupRule { namePrefix = "Houses", parentPath = "Neighborhood/Houses" });
            rules.Add(new GroupRule { namePrefix = "Barricade", parentPath = "Neighborhood/Street Props/Barriers" });
            rules.Add(new GroupRule { namePrefix = "Barrier", parentPath = "Neighborhood/Street Props/Barriers" });
            rules.Add(new GroupRule { namePrefix = "Cone", parentPath = "Neighborhood/Street Props/Barriers" });
            rules.Add(new GroupRule { namePrefix = "Bollard", parentPath = "Neighborhood/Street Props/Barriers" });
            rules.Add(new GroupRule { namePrefix = "JerseyBarrier", parentPath = "Neighborhood/Street Props/Barriers" });
            rules.Add(new GroupRule { namePrefix = "Fence", parentPath = "Neighborhood/StreetEdges" });
            rules.Add(new GroupRule { namePrefix = "InvisibleWall", parentPath = "Neighborhood/Street Props/Barriers" });
            rules.Add(new GroupRule { namePrefix = "Wall", parentPath = "Neighborhood/StreetEdges" });
            rules.Add(new GroupRule { namePrefix = "street_light", parentPath = "Neighborhood/Street Props/Lights" });
            rules.Add(new GroupRule { namePrefix = "traffic_light", parentPath = "Neighborhood/Street Props/Lights" });
            rules.Add(new GroupRule { namePrefix = "trash", parentPath = "Neighborhood/Street Props/Clutter" });
            rules.Add(new GroupRule { namePrefix = "metaltrashcan", parentPath = "Neighborhood/Street Props/Clutter" });
            rules.Add(new GroupRule { namePrefix = "dumpster", parentPath = "Neighborhood/Street Props/Clutter" });
            rules.Add(new GroupRule { namePrefix = "busstop", parentPath = "Neighborhood/Street Props/Clutter" });
            rules.Add(new GroupRule { namePrefix = "Water", parentPath = "Neighborhood/Water" });
            rules.Add(new GroupRule { namePrefix = "River", parentPath = "Neighborhood/Water" });
            rules.Add(new GroupRule { namePrefix = "Evacuation", parentPath = "Neighborhood/EvacuationCenter" });
            rules.Add(new GroupRule { namePrefix = "Boundar", parentPath = "Neighborhood/Boundaries" });
            rules.Add(new GroupRule { namePrefix = "Pothole", parentPath = "Neighborhood/Hazards" });
            rules.Add(new GroupRule { namePrefix = "Hazards", parentPath = "Neighborhood/Hazards" });
            rules.Add(new GroupRule { namePrefix = "FloodedArea", parentPath = "Neighborhood/Hazards" });
            rules.Add(new GroupRule { namePrefix = "FloodedRoute", parentPath = "Neighborhood/Hazards" });
            rules.Add(new GroupRule { namePrefix = "FloodedStreet", parentPath = "Neighborhood/Hazards" });
            rules.Add(new GroupRule { namePrefix = "EvacuationMarker", parentPath = "Neighborhood/EvacuationCenter" });
            rules.Add(new GroupRule { namePrefix = "BackWall", parentPath = "Neighborhood/BackWalls" });
            rules.Add(new GroupRule { namePrefix = "Fence_Slab", parentPath = "Neighborhood/BackWalls" });
            rules.Add(new GroupRule { namePrefix = "StreetEdges", parentPath = "Neighborhood/StreetEdges" });
            rules.Add(new GroupRule { namePrefix = "Street Props", parentPath = "Neighborhood/Street Props" });
            rules.Add(new GroupRule { namePrefix = "Cube", parentPath = "Neighborhood/Placeholder Buildings" });
            rules.Add(new GroupRule { namePrefix = "Placeholder Buildings", parentPath = "Neighborhood/Placeholder Buildings" });
            rules.Add(new GroupRule { namePrefix = "Plane", parentPath = "Neighborhood/Ground" });
            rules.Add(new GroupRule { namePrefix = "EvacuationSpawnPoint", parentPath = "Neighborhood" });
        }

        if (rootOrder.Count == 0)
        {
            rootOrder.AddRange(new[]
            {
                "Lighting", "Sun", "PSX Look", "EventSystem", "GameManager", "Player", "WeatherSystem", "UI",
                "HazardMapAlert", "Environment", "Tasks_Debug_Minigames", "Neighborhood",
            });
        }

        if (groupOrder.Count == 0)
        {
            groupOrder.AddRange(new[]
            {
                "Ground", "Roads", "Houses", "BackWalls", "StreetEdges", "Street Props", "Placeholder Buildings",
                "Water", "Hazards", "Boundaries", "EvacuationCenter", "EvacuationSpawnPoint",
            });
        }
    }

    void OnGUI()
    {
        serializedWindow.Update();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.HelpBox("Works on the open scene. Moves loose objects into their groups, fixes strays inside the groups and sorts them A to Z. Nothing is deleted except empty groups that got merged. Ctrl+Z undoes it, Ctrl+S saves it.", MessageType.Info);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("rules"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("rootOrder"), true);
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("orderedParent"));
        EditorGUILayout.PropertyField(serializedWindow.FindProperty("groupOrder"), true);
        EditorGUILayout.EndScrollView();
        serializedWindow.ApplyModifiedProperties();

        if (GUILayout.Button("Reset Lists To Defaults"))
        {
            rules.Clear();
            rootOrder.Clear();
            groupOrder.Clear();
            FillDefaults();
            serializedWindow = new SerializedObject(this);
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Organize Open Scene", GUILayout.Height(32)))
            Organize(SceneManager.GetActiveScene());
    }

    void Organize(Scene scene)
    {
        Undo.SetCurrentGroupName("Organize Hierarchy");
        int group = Undo.GetCurrentGroup();
        int moved = 0;
        List<string> leftover = new List<string>();

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (rootOrder.Contains(root.name))
                continue;

            GroupRule rule = FindRule(root.name);
            if (rule == null)
            {
                leftover.Add(root.name);
                continue;
            }

            Transform parent = MakePath(scene, rule.parentPath);
            if (IsPlainGroup(root) && root.name == parent.name)
            {
                for (int i = root.transform.childCount - 1; i >= 0; i--)
                    Undo.SetTransformParent(root.transform.GetChild(i), parent, "Organize Hierarchy");
                Undo.DestroyObjectImmediate(root);
            }
            else
            {
                Undo.SetTransformParent(root.transform, parent, "Organize Hierarchy");
            }
            moved++;
        }

        for (int i = 0; i < rootOrder.Count; i++)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == rootOrder[i])
                    Undo.SetSiblingIndex(root.transform, Mathf.Min(i, scene.rootCount - 1), "Organize Hierarchy");
            }
        }

        List<string> groupPaths = GetGroupPaths();
        int strays = 0;
        foreach (string path in groupPaths)
        {
            Transform groupTransform = FindPath(scene, path);
            if (groupTransform == null || PrefabUtility.IsPartOfAnyPrefab(groupTransform.gameObject))
                continue;
            strays += MoveStrays(scene, groupTransform, path);
        }

        int sorted = 0;
        foreach (string path in groupPaths)
        {
            Transform groupTransform = FindPath(scene, path);
            if (groupTransform == null || PrefabUtility.IsPartOfAnyPrefab(groupTransform.gameObject))
                continue;
            SortChildren(groupTransform, path == orderedParent);
            sorted++;
        }

        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("HierarchyOrganizer: moved " + moved + " root object(s) and " + strays + " stray(s), sorted " + sorted + " group(s) in " + scene.name + ".");
        if (leftover.Count > 0)
            Debug.LogWarning("HierarchyOrganizer: no rule for " + string.Join(", ", leftover) + ". Add a rule or move them by hand.");
    }

    List<string> GetGroupPaths()
    {
        List<string> paths = new List<string>();
        foreach (GroupRule rule in rules)
        {
            if (string.IsNullOrEmpty(rule.parentPath))
                continue;

            string[] parts = rule.parentPath.Split('/');
            string path = "";
            foreach (string part in parts)
            {
                path = path.Length == 0 ? part : path + "/" + part;
                if (!paths.Contains(path))
                    paths.Add(path);
            }
        }
        return paths;
    }

    int MoveStrays(Scene scene, Transform groupTransform, string path)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in groupTransform)
            children.Add(child);

        int count = 0;
        foreach (Transform child in children)
        {
            GroupRule rule = FindRule(child.name);
            if (rule == null || rule.parentPath == path)
                continue;

            string childPath = path + "/" + child.name;
            if (rule.parentPath == childPath || rule.parentPath.StartsWith(childPath + "/"))
                continue;

            Undo.SetTransformParent(child, MakePath(scene, rule.parentPath), "Organize Hierarchy");
            count++;
        }
        return count;
    }

    void SortChildren(Transform groupTransform, bool useGroupOrder)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in groupTransform)
            children.Add(child);

        children.Sort((a, b) =>
        {
            if (useGroupOrder)
            {
                int orderA = OrderOf(a.name);
                int orderB = OrderOf(b.name);
                if (orderA != orderB)
                    return orderA.CompareTo(orderB);
            }
            else
            {
                bool groupA = IsPlainGroup(a.gameObject) && a.childCount > 0;
                bool groupB = IsPlainGroup(b.gameObject) && b.childCount > 0;
                if (groupA != groupB)
                    return groupA ? -1 : 1;
            }
            return EditorUtility.NaturalCompare(a.name, b.name);
        });

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].GetSiblingIndex() != i)
                Undo.SetSiblingIndex(children[i], i, "Organize Hierarchy");
        }
    }

    int OrderOf(string name)
    {
        int index = groupOrder.IndexOf(name);
        return index < 0 ? groupOrder.Count : index;
    }

    static Transform FindPath(Scene scene, string path)
    {
        Transform current = null;
        foreach (string part in path.Split('/'))
        {
            Transform next = null;
            if (current == null)
            {
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    if (root.name == part && IsPlainGroup(root))
                        next = root.transform;
                }
            }
            else
            {
                next = current.Find(part);
            }

            if (next == null)
                return null;
            current = next;
        }
        return current;
    }

    GroupRule FindRule(string name)
    {
        GroupRule best = null;
        foreach (GroupRule rule in rules)
        {
            if (string.IsNullOrEmpty(rule.namePrefix) || !name.StartsWith(rule.namePrefix))
                continue;
            if (best == null || rule.namePrefix.Length > best.namePrefix.Length)
                best = rule;
        }
        return best;
    }

    static bool IsPlainGroup(GameObject go)
    {
        return go.GetComponents<Component>().Length == 1 && !PrefabUtility.IsPartOfAnyPrefab(go);
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
                    if (root.name == part && IsPlainGroup(root))
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
                Undo.RegisterCreatedObjectUndo(made, "Organize Hierarchy");
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
