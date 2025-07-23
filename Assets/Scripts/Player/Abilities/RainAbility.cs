using UnityEngine;
using System.Collections.Generic;
using Terrain;
using UI;

namespace Player.Abilities
{
    public class RainAbility : PlayerAbility
    {
        public override int Cost => 100;
        public override AbilityType Type => AbilityType.Rain;
        public override int EffectDiameter => 5;

        public override void EnterAbility()
        {
            Debug.Log("Entered Rain Ability");
            GridOverlayManager.Instance.aoeHighlightColor = new Color(0, 0.5f, 1f, 0.7f);
        }

        public override void CastAbility(Vector3Int centerTilePos)
        {
            // Get affected tiles.
            var radius = (EffectDiameter - 1) / 2;
            var affectedTiles = TileManager.Instance.GetSpecificRange(centerTilePos, radius);
            
            // Apply water increase to tiles.
            foreach (var tilePos in affectedTiles)
            {
                var tileData = TileManager.Instance.getTileDataByGridCoords(tilePos);
                if (tileData != null) tileData.waterValue += 100;
            }
            
            // Update heatmaps.
            var affectedChunks = new HashSet<Vector2>();
            foreach (var tilePos in affectedTiles)
            {
                var chunks = TileManager.Instance.getWorldPositionOfTile(tilePos);
                foreach (var chunk in chunks) affectedChunks.Add(chunk);
            }
            foreach (var chunk in affectedChunks)
            {
                UIEvents.UIMap.OnUpdateHeatmapChunks.Invoke(
                    new List<Vector2> { chunk }, 
                    MapDisplay.MapOverlay.WaterValue
                );
            }
            SpawnEffectOnTiles(affectedTiles, "Rain", heightOffset: 20f, scaleMultiplier: 1.5f);
        }
    }
}