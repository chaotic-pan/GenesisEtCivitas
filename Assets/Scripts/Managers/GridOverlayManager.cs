using System.Collections.Generic;
using MapGeneration;
using Terrain;
using UnityEngine;

public class GridOverlayManager : MonoBehaviour
{
    public static GridOverlayManager Instance;
    public Color aoeHighlightColor = new Color(0, 0.5f, 1f, 0.7f);
    
    private Dictionary<Vector2, Texture2D> originalTextures = new Dictionary<Vector2, Texture2D>();
    private Dictionary<Vector2, Texture2D> overlayTextures = new Dictionary<Vector2, Texture2D>();
    private List<Vector3Int> currentAoeTiles = new List<Vector3Int>();
    private MapDisplay.MapOverlay originalOverlay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowAoeOverlay(List<Vector3Int> aoeTiles)
    {
        if (HeatmapDisplay.Instance == null || MapDisplay.Instance == null)
        {
            Debug.LogError("Missing MapDisplay or HeatmapDisplay reference");
            return;
        }
        
        currentAoeTiles = aoeTiles;
        originalOverlay = HeatmapDisplay.Instance._currentMapOverlay;
        
        var currentTextures = HeatmapDisplay.Instance._maps[originalOverlay];
        foreach (var kvp in currentTextures)
        {
            originalTextures[kvp.Key] = kvp.Value;
            overlayTextures[kvp.Key] = CreateOverlayTexture(kvp.Value, kvp.Key);
        }
        
        MapDisplay.Instance.ReplaceTexture(overlayTextures);
    }

    public void HideAoeOverlay()
    {
        if (MapDisplay.Instance != null)
        {
            MapDisplay.Instance.ReplaceTexture(originalTextures);
        }
        currentAoeTiles.Clear();
        originalTextures.Clear();
        overlayTextures.Clear();
    }

    private Texture2D CreateOverlayTexture(Texture2D baseTexture, Vector2 chunkCoord)
    {
        Texture2D overlayTex = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false);
        overlayTex.SetPixels(baseTexture.GetPixels());
        
        foreach (var tilePos in currentAoeTiles)
        {
            Vector2 tileChunk = TileManager.Instance.GetChunkForTile(tilePos);
            if (tileChunk != chunkCoord) continue;

            Vector3 worldPos = TileManager.Instance.map.CellToWorld(tilePos);
            
            // Calculate UV coordinates within chunk.
            float uvX = (worldPos.x % 239f) / 239f;
            float uvY = (-worldPos.z % 239f) / 239f;
            
            // Convert to texture coordinates.
            int texX = Mathf.FloorToInt(uvX * baseTexture.width);
            int texY = Mathf.FloorToInt(uvY * baseTexture.height);
            
            // Highlight the tile with a 5x5 pixel area.
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    int px = Mathf.Clamp(texX + i, 0, baseTexture.width - 1);
                    int py = Mathf.Clamp(texY + j, 0, baseTexture.height - 1);
                    overlayTex.SetPixel(px, py, aoeHighlightColor);
                }
            }
        }
        
        overlayTex.Apply();
        return overlayTex;
    }
}