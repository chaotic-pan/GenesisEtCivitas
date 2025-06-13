// MADE BY GENESIS

using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class MapDisplay : MonoBehaviour
    {
        public GameObject meshRendererPrefab;
        private const int size = 239;
        private int scale = 1;
        public void DrawMeshes(Dictionary<Vector2,MeshData> meshDataDict, Dictionary<Vector2,Texture2D> textures)
        {
            foreach (var chunkCoordinate in meshDataDict.Keys)
            {
                var newMesh = Instantiate(meshRendererPrefab, 
                    new Vector3(scale * size * chunkCoordinate.x, 0, -scale * size * chunkCoordinate.y), 
                    Quaternion.identity, 
                    transform);
                newMesh.transform.localScale = new Vector3(scale,scale,scale);
                
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