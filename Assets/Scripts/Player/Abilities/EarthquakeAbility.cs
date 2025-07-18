using UnityEngine;
using System.Collections.Generic;
using Terrain;
using UI;

namespace Player.Abilities
{
    public class EarthquakeAbility : PlayerAbility
    {
        public override int Cost => 50;
        public override AbilityType Type => AbilityType.Earthquake;
        public override int EffectDiameter => 7;

        public override void EnterAbility()
        {
            Debug.Log("Entered Earthquake Ability");
            GridOverlayManager.Instance.aoeHighlightColor = new Color(1f, 0.4f, 0f, 0.7f);
        }

        public override void CastAbility(Vector3Int centerTilePos)
        {
            // Get all tiles in AoE.
            int radius = (EffectDiameter - 1) / 2;
            List<Vector3Int> affectedTiles = TileManager.Instance.GetSpecificRange(centerTilePos, radius);
            
            // Apply firmness reduction to each tile.
            foreach (Vector3Int tilePos in affectedTiles)
            {
                var tileData = TileManager.Instance.getTileDataByGridCoords(tilePos);
                if (tileData != null)
                {
                    tileData.firmness = Mathf.Max(tileData.firmness - 3, 0);
                }
            }
            
            // Update heatmaps for all affected chunks.
            HashSet<Vector2> affectedChunks = new HashSet<Vector2>();
            foreach (Vector3Int tilePos in affectedTiles)
            {
                var chunks = TileManager.Instance.getWorldPositionOfTile(tilePos);
                foreach (Vector2 chunk in chunks)
                {
                    affectedChunks.Add(chunk);
                }
            }
            
            // Trigger heatmap updates.
            foreach (Vector2 chunk in affectedChunks)
            {
                UIEvents.UIMap.OnUpdateHeatmapChunks.Invoke(
                    new List<Vector2> { chunk }, 
                    MapDisplay.MapOverlay.Firmness
                );
            }
            SpawnEffectOnTiles(affectedTiles, "Earthquake", heightOffset: 5f, scaleMultiplier: 1.2f);
        }
    }
}