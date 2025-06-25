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
    private HashSet<Vector2> affectedChunks = new HashSet<Vector2>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowAoeOverlay(List<Vector3Int> aoeTiles)
    {
        if (HeatmapDisplay.Instance == null || MapDisplay.Instance == null) return;

        // Restore all previously modified textures first.
        RestoreOriginalTextures();
        
        currentAoeTiles = new HashSet<Vector3Int>(aoeTiles);
        originalOverlay = HeatmapDisplay.Instance._currentMapOverlay;
        
        var currentTextures = HeatmapDisplay.Instance._maps[originalOverlay];
        affectedChunks.Clear();

        // Find all chunks that need updating.
        foreach (var tile in currentAoeTiles)
        {
            var chunks = TileManager.Instance.getWorldPositionOfTile(tile);
            foreach (var chunk in chunks)
            {
                affectedChunks.Add(chunk);
            }
        }

        // Process only affected chunks.
        foreach (var chunk in affectedChunks)
        {
            if (!currentTextures.ContainsKey(chunk)) continue;
            
            // Store original if not already stored.
            if (!originalTextures.ContainsKey(chunk))
            {
                originalTextures[chunk] = currentTextures[chunk];
            }
            
            // Create or reuse overlay texture.
            if (!overlayTextures.ContainsKey(chunk))
            {
                overlayTextures[chunk] = new Texture2D(ChunkSize, ChunkSize, TextureFormat.ARGB32, false);
            }
            
            UpdateOverlayTexture(chunk);
        }
        
        MapDisplay.Instance.ReplaceTexture(overlayTextures);
    }
    
    private void RestoreOriginalTextures()
    {
        foreach (var kvp in originalTextures)
        {
            if (overlayTextures.ContainsKey(kvp.Key))
            {
                overlayTextures[kvp.Key].SetPixels(kvp.Value.GetPixels());
                overlayTextures[kvp.Key].Apply();
            }
        }
    }

    private void UpdateOverlayTexture(Vector2 chunkCoord)
    {
        var baseTexture = originalTextures[chunkCoord];
        var overlayTex = overlayTextures[chunkCoord];
        
        // Copy base texture.
        overlayTex.SetPixels(baseTexture.GetPixels());
        
        // Calculate chunk origin.
        float chunkOriginX = chunkCoord.x * (ChunkSize - 1) + 1;
        float chunkOriginZ = -(chunkCoord.y * (ChunkSize - 1));
        
        // Highlight affected tiles.
        for (int y = 0; y < ChunkSize; y++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                Vector3 worldPos = new Vector3(
                    chunkOriginX + x - HalfChunkSize,
                    0,
                    chunkOriginZ + y - HalfChunkSize
                );
                
                Vector3Int gridPos = TileManager.Instance.map.WorldToCell(worldPos);
                
                if (currentAoeTiles.Contains(gridPos))
                {
                    int textureY = ChunkSize - 1 - y;
                    overlayTex.SetPixel(x, textureY, aoeHighlightColor);
                }
            }
        }
        overlayTex.Apply();
    }

    public void HideAoeOverlay()
    {
        if (MapDisplay.Instance != null)
        {
            // Restore all original textures.
            RestoreOriginalTextures();
            MapDisplay.Instance.ReplaceTexture(originalTextures);
        }
        
        currentAoeTiles.Clear();
        originalTextures.Clear();
        overlayTextures.Clear();
        affectedChunks.Clear();
    }
}