using Terrain;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace EditorScripts
{
    [CustomEditor(typeof(MapExtractor))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var mapGen = target as MapExtractor;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Map"))
                mapGen.GenerateMap();
        }
    }
}