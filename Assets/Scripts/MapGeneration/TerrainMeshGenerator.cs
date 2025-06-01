// MADE BY GENESIS

using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Terrain
{
    public static class TerrainMeshGenerator
    {
        public static Dictionary<Vector2, MeshData> GenerateMesh(float[,] heightMap, float heightMultiplier, AnimationCurve meshHeightCurve, int nHorizontalChunks, int nVerticalChunks)
        {
            const int width = 240; //ChunkSize
            const int height = 240; //ChunkSize
                    
            const float topLeftX = (width - 1) / -2f;
            const float topLeftZ = (height - 1) / 2f;
            
            var dict = new Dictionary<Vector2, MeshData>();
            
            foreach (var chunkCoord in VectorUtils.GridCoordinates(nHorizontalChunks, nVerticalChunks))
            {
                var chunkXOffset = chunkCoord.x * 239;
                var chunkYOffset = chunkCoord.y * 239;

                var meshData = new MeshData(width, height);
                var vertexIndex = 0;
                foreach (var coord in VectorUtils.GridCoordinates(width, height))
                {
                    var offsetX = coord.x + chunkXOffset;
                    var offsetY = coord.y + chunkYOffset;
                    
                    meshData.Vertices[vertexIndex] = new Vector3(topLeftX + coord.x, meshHeightCurve.Evaluate(heightMap[offsetX,offsetY]) * heightMultiplier, topLeftZ - coord.y);
                    meshData.Uv[vertexIndex] = new Vector2(coord.x/(float)width, coord.y/(float)height);
                    if (coord is { x: < width - 1, y: < height - 1 })// && heightMap[coord.x, coord.y] >= 0)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }
                    
                    vertexIndex++;
                }

                dict.Add(new Vector2(chunkCoord.x, chunkCoord.y), meshData);
            }
            return dict;
        }
    }
}