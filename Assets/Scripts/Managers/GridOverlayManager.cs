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
    private HashSet<Vector3Int> currentAoeTiles = new HashSet<Vector3Int>();
    private MapDisplay.MapOverlay originalOverlay;

    private const int ChunkSize = 240;
    private const int HalfChunkSize = ChunkSize / 2;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowAoeOverlay(List<Vector3Int> aoeTiles)
    {
        if (HeatmapDisplay.Instance == null || MapDisplay.Instance == null) return;
        
        currentAoeTiles = new HashSet<Vector3Int>(aoeTiles);
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
        
        // Calculate chunk origin.
        float chunkOriginX = chunkCoord.x * (ChunkSize - 1) + 1;
        float chunkOriginZ = -(chunkCoord.y * (ChunkSize - 1));
        
        for (int y = 0; y < ChunkSize; y++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                // Calculate world position for this texel.
                Vector3 worldPos = new Vector3(
                    chunkOriginX + x - HalfChunkSize,
                    0,
                    chunkOriginZ + y - HalfChunkSize
                );
                
                // Convert to grid position.
                Vector3Int gridPos = TileManager.Instance.map.WorldToCell(worldPos);
                
                if (currentAoeTiles.Contains(gridPos))
                {
                    // Flip Y coordinate to match texture orientation.
                    int textureY = ChunkSize - 1 - y;
                    overlayTex.SetPixel(x, textureY, aoeHighlightColor);
                }
            }
        }
        
        overlayTex.Apply();
        return overlayTex;
    }
}