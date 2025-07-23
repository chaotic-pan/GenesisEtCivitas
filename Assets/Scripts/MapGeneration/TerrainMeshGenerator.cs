// MADE BY GENESIS

using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Terrain
{
    public static class TerrainMeshGenerator
    {
        const int width = 240; //ChunkSize
        const int height = 240; //ChunkSize
                    
        const float topLeftX = (width - 1) / -2f;
        const float topLeftZ = (height - 1) / 2f;
        
        public static Dictionary<Vector2, MeshData> GenerateMesh(float[,] heightMap, float heightMultiplier, AnimationCurve meshHeightCurve, int nChunks, int points)
        {
           
            
            var dict = new Dictionary<Vector2, MeshData>();
            
            foreach (var chunkCoord in VectorUtils.GridCoordinates(nChunks, nChunks))
            {
                var chunkXOffset = chunkCoord.x * 239;
                var chunkYOffset = chunkCoord.y * 239;

                MeshData mesh;

                if (chunkCoord.x < 2 || chunkCoord.y < 2 || chunkCoord.y > nChunks - 3 || chunkCoord.x > nChunks - 3)
                    mesh = GenerateChunkMeshAtBorder(heightMap, heightMultiplier, chunkXOffset, chunkYOffset, points);
                else
                    mesh = GenerateChunk(heightMap, heightMultiplier, chunkXOffset, chunkYOffset);

                dict.Add(new Vector2(chunkCoord.x, chunkCoord.y), mesh);
            }
            return dict;
        }

        private static MeshData GenerateChunk(float[,] heightMap, float heightMultiplier, int chunkXOffset, int chunkYOffset)
        {
            var vertexIndex = 0;
            var meshData = new MeshData(width, height);
            foreach (var coord in VectorUtils.GridCoordinates(width, height))
            {
                var offsetX = coord.x + chunkXOffset;
                var offsetY = coord.y + chunkYOffset;

                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + coord.x, heightMap[offsetX, offsetY] * heightMultiplier, topLeftZ - coord.y);
                meshData.Uv[vertexIndex] = new Vector2(coord.x / (float)width, coord.y / (float)height);
                if (coord is var vector2Int && vector2Int.y < height - 1 && vector2Int.x < width - 1) // && heightMap[coord.x, coord.y] >= 0)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }

            return meshData;
        }
        
        private static MeshData GenerateChunkMeshAtBorder(float[,] heightMap, float heightMultiplier, int chunkXOffset, int chunkYOffset, int points)
        {
            
            var oceanRadius = points * 0.5f;
            var centerXY = (points - 1) * 0.5f;
            var oceanRadiusPow2 = Math.Pow(oceanRadius, 2);
            
            var vertexIndex = 0;
            var meshData = new MeshData(width, height);
            foreach (var coord in VectorUtils.GridCoordinates(width, height))
            {
                var offsetX = coord.x + chunkXOffset;
                var offsetY = coord.y + chunkYOffset;

                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + coord.x, heightMap[offsetX, offsetY] * heightMultiplier, topLeftZ - coord.y);
                meshData.Uv[vertexIndex] = new Vector2(coord.x / (float)width, coord.y / (float)height);
                
                var dx = chunkXOffset + coord.x - centerXY;
                var dy = chunkYOffset + coord.y - centerXY;
                var distanceFromCenterPow2 = Math.Pow(dx, 2) + Math.Pow(dy, 2);
                if (distanceFromCenterPow2 > oceanRadiusPow2)
                {
                    vertexIndex++;
                    continue;
                }
                
                if (coord is var vector2Int && vector2Int.y < height - 1 && vector2Int.x < width - 1) // && heightMap[coord.x, coord.y] >= 0)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }

            return meshData;
        }
    }
}