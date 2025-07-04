using MapGeneration;
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
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Map"))
            {
                var mapGen = target as MapExtractor;
                var heatmapGen = mapGen.GetComponent<HeatmapGenerator>();
                var tilemapGen = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
                
                mapGen.GenerateMap();
                tilemapGen.InitializeTileData(mapGen);
                heatmapGen.GenerateAllHeatmapsAndWriteToDrive(tilemapGen);
            }
        }
    }
}