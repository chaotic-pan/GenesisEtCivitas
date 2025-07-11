// MADE BY GENESIS

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public struct TerrainType
    {
        public float height;
        public Color color;

        public TerrainType(float height, Color color)
        {
            this.height = height;
            this.color = color;
        }
    }

    public static class TextureGenerator
    {
        
        public static Texture2D TextureFromColorMap(Color[] colorMap, int width)
        {
            var texture = new Texture2D(width, width)
            {
                filterMode = FilterMode.Point
            };
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        public static Dictionary<Vector2, Texture2D> TextureFromColorMaps(Dictionary<Vector2, Color[]> colorMaps, int size)
        {
            var dict = new Dictionary<Vector2, Texture2D>();
            foreach (var entry in colorMaps)
            {
                dict.Add(entry.Key, TextureFromColorMap(entry.Value, size));
            }

            return dict;
        }

        
    }
}