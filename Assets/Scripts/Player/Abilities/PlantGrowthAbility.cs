using UnityEngine;
using System.Collections.Generic;
using Terrain;
using UI;

namespace Player.Abilities
{
    public class PlantGrowthAbility: PlayerAbility
    {
        public override int Cost => 80;
        public override AbilityType Type => AbilityType.PlantGrowth;
        public override int EffectDiameter => 5;

        public override void EnterAbility()
        {
            Debug.Log("Entered PG Ability");
            GridOverlayManager.Instance.aoeHighlightColor = new Color(0, 1f, 0.5f, 0.7f);
        }

        public override void CastAbility(Vector3Int centerTilePos)
        {
            // Get all tiles in AoE.
            int radius = (EffectDiameter - 1) / 2;
            List<Vector3Int> affectedTiles = TileManager.Instance.GetSpecificRange(centerTilePos, radius);
            
            // Apply fertility increase to each tile.
            foreach (Vector3Int tilePos in affectedTiles)
            {
                var tileData = TileManager.Instance.getTileDataByGridCoords(tilePos);
                if (tileData != null)
                {
                    tileData.landFertility += 10;
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
                    MapDisplay.MapOverlay.Fertility
                );
            }
        }
    }
}