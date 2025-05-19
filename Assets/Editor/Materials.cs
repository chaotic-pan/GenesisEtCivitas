using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;



public class MaterialReplacerWithMapping : EditorWindow
{
    private string fbxFolder = "Assets/Models";
    private string materialFolder = "Assets/Materials";
    private string prefabSaveFolder = "Assets/Prefabs"; // Where to save the final prefabs

    // Map FBX material names → your custom material names
    /*private Dictionary<string, string> naturePackMapping = new()
    {
        { "Snow", "_white" },
        { "Black", "_black" },
        { "White", "_white" },
        { "Rock.001", "_lightGray2" },
        { "Snow.001", "_white" },
        { "Green", "_green2" },
        { "Red", "_red3" },
        { "Wood", "_brown1" },
        { "LightOrange", "_orange0" },
        { "Rock", "_gray1" },
        { "LightWood", "_brown0" },
        { "DarkGreen", "_green4" },
        { "ColorLightGray_", "_lightGray1" },
        { "Yellow", "_yellow2" },
        { "Cyan", "_blueGreen2" },
        { "Leaves", "_brown0" },
        { "Pink", "_pink1" },
        { "Orange", "_orange2" }
    };*/

    private Dictionary<string, string> materialNameMapping = new()
    {
        { "Snow", "_white" },
        { "Black", "_black" },
        { "White", "_white" },
        { "Rock.001", "_lightGray2" },
        { "Snow.001", "_white" },
        { "Green", "_green2" },
        { "Red", "_red3" },
        { "Wood", "_brown1" },
        { "LightOrange", "_orange0" },
        { "Rock", "_gray1" },
        { "LightWood", "_brown0" },
        { "DarkGreen", "_green4" },
        { "ColorLightGray_", "_lightGray1" },
        { "Yellow", "_yellow2" },
        { "Cyan", "_blueGreen2" },
        { "Leaves", "_brown0" },
        { "Pink", "_pink1" },
        { "Orange", "_orange2" }
    };
    
    
    [MenuItem("Tools/Replace Materials And Overwrite Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<MaterialReplacerWithMapping>("Replace Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Replacer With Mapping", EditorStyles.boldLabel);
        fbxFolder = EditorGUILayout.TextField("FBX Folder", fbxFolder);
        materialFolder = EditorGUILayout.TextField("Material Folder", materialFolder);
        prefabSaveFolder = EditorGUILayout.TextField("Prefab Save Folder", prefabSaveFolder);

        if (GUILayout.Button("Replace Materials and Overwrite Prefabs"))
        {
            ReplaceMaterials();
        }
    }

    private void ReplaceMaterials()
    {
        // Load all custom materials into a dictionary
        string[] allCustomMatPaths = Directory.GetFiles(materialFolder, "*.mat", SearchOption.AllDirectories);
        Dictionary<string, Material> customMaterials = new Dictionary<string, Material>();

        foreach (string matPath in allCustomMatPaths)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat != null && !customMaterials.ContainsKey(mat.name))
                customMaterials.Add(mat.name, mat);
        }

        // Load all FBX files
        string[] fbxPaths = Directory.GetFiles(fbxFolder, "*.fbx", SearchOption.AllDirectories);

        foreach (string fbxPath in fbxPaths)
        {
            if (QualitySettings.antiAliasing < 1)
                QualitySettings.antiAliasing = 1;
            
            
            GameObject fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
            if (fbxAsset == null) continue;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset);
            if (instance == null) continue;

            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    Material oldMat = renderer.sharedMaterials[i];
                    if (oldMat == null)
                    {
                        newMaterials[i] = null;
                        continue;
                    }

                    if (materialNameMapping.TryGetValue(oldMat.name, out string newMatName) &&
                        customMaterials.TryGetValue(newMatName, out Material newMat))
                    {
                        newMaterials[i] = newMat;
                    }
                    else
                    {
                        newMaterials[i] = oldMat;
                    }
                }

                renderer.sharedMaterials = newMaterials;
            }

            // Build target prefab path
            string filename = Path.GetFileNameWithoutExtension(fbxPath);
            string prefabPath = Path.Combine(prefabSaveFolder, filename + ".prefab").Replace("\\", "/");

            // Ensure directory exists
            Directory.CreateDirectory(prefabSaveFolder);

            // Overwrite existing prefab
            PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabPath, InteractionMode.AutomatedAction);
            GameObject.DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ All prefabs created or overwritten successfully.");
    }
}