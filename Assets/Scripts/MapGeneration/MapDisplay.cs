// MADE BY GENESIS

using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class MapDisplay : MonoBehaviour
    {
        public GameObject meshRendererPrefab;
        public void DrawMeshes(Dictionary<Vector2,MeshData> meshDataDict, Dictionary<Vector2,Texture2D> textures)
        {
            foreach (var chunkCoordinate in meshDataDict.Keys)
            {
                var newMesh = Instantiate(meshRendererPrefab, new Vector3(2390 * chunkCoordinate.x, 0, -2390 * chunkCoordinate.y), Quaternion.identity);
                
                var meshData = meshDataDict[chunkCoordinate];
                newMesh.GetComponent<MeshFilter>().sharedMesh = meshData.CreateMesh();
                var meshRenderer = newMesh.GetComponent<MeshRenderer>();
                var material = new Material(meshRenderer.sharedMaterial)
                {
                    mainTexture = textures[chunkCoordinate]
                };
                meshRenderer.sharedMaterial = material;
            }
        }
    }
}