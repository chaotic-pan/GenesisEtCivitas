using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RemapMaterials : EditorWindow
{
    string folderPath = "Assets/";

    // Dynamic list of mappings
    [System.Serializable]
    public class MaterialMapping
    {
        public string originalMaterialName;
        public Material newMaterial;
    }

    List<MaterialMapping> materialMappings = new List<MaterialMapping>();

    Vector2 scrollPos;
    
    [MenuItem("Tools/Batch Replace Materials In Folder")]
    public static void ShowWindow()
    {
        GetWindow<RemapMaterials>("Material Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("FBX Material Replacer", EditorStyles.boldLabel);

        // Folder input
        EditorGUILayout.BeginHorizontal();
        folderPath = EditorGUILayout.TextField("FBX Folder", folderPath);
        if (GUILayout.Button("Select", GUILayout.MaxWidth(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Folder with FBX Files", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                // Convert absolute path to relative project path
                if (selected.StartsWith(Application.dataPath))
                    folderPath = "Assets" + selected.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Mapping entries
        GUILayout.Label("Material Mappings", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

        for (int i = 0; i < materialMappings.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            materialMappings[i].originalMaterialName = EditorGUILayout.TextField(materialMappings[i].originalMaterialName);
            materialMappings[i].newMaterial = (Material)EditorGUILayout.ObjectField(materialMappings[i].newMaterial, typeof(Material), false);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                materialMappings.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Mapping"))
        {
            materialMappings.Add(new MaterialMapping());
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Replace Materials In Folder"))
        {
            ReplaceMaterialsInFolder();
        }
    }

    void ReplaceMaterialsInFolder()
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("Invalid folder: " + folderPath);
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null) continue;

            bool changed = false;
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                Material[] mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    string matName = mats[i].name;
                    foreach (var map in materialMappings)
                    {
                        if (!string.IsNullOrWhiteSpace(map.originalMaterialName) &&
                            matName.ToLower().Contains(map.originalMaterialName.ToLower()))
                        {
                            mats[i] = map.newMaterial;
                            changed = true;
                            break;
                        }
                    }
                }
                r.sharedMaterials = mats;
            }

            if (changed)
            {
                EditorUtility.SetDirty(prefab);
                Debug.Log("Updated: " + assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Finished replacing materials.");
    }
}