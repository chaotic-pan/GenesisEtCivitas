using UnityEngine;
using System.Collections.Generic;
using Terrain;
using UI;

namespace Player.Abilities
{
    public class RainAbility : PlayerAbility
    {
        public override int Cost => 30;
        public override AbilityType Type => AbilityType.Rain;
        public override int EffectDiameter => 5;

        public override void EnterAbility()
        {
            Debug.Log("Entered Rain Ability");
        }

        public override void CastAbility(Vector3Int centerTilePos)
        {
            // Get all tiles in AoE.
            int radius = (EffectDiameter - 1) / 2;
            List<Vector3Int> affectedTiles = TileManager.Instance.GetSpecificRange(centerTilePos, radius);
            
            // Apply water increase to each tile.
            foreach (Vector3Int tilePos in affectedTiles)
            {
                var tileData = TileManager.Instance.getTileDataByGridCoords(tilePos);
                if (tileData != null)
                {
                    tileData.waterValue += 20;
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
                    MapDisplay.MapOverlay.WaterValue
                );
            }
        }
    }
}