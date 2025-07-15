using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Managers;
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
                if (tileData != null) tileData.waterValue += 20;
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
            
            SpawnRainEffect(centerTilePos, affectedTiles);
        }

        private void SpawnRainEffect(Vector3Int center, List<Vector3Int> tiles)
        {
            if (EffectManager.instance == null) return;

            foreach (var tilePos in tiles)
            {
                var worldPos = TileManager.Instance.map.GetCellCenterWorld(tilePos);
                var tileData = TileManager.Instance.getTileDataByGridCoords(tilePos);
                if (tileData == null) continue;

                // Position effect 20 units above THIS tile's terrain.
                var spawnPos = new Vector3(
                    worldPos.x,
                    tileData.height + 20f,
                    worldPos.z
                );

                var rain = EffectManager.instance.GetEffect("Rain");
                if (rain == null) continue;

                rain.transform.position = spawnPos;
        
                // Scale to cover just this tile.
                var hexSize = TileManager.Instance.map.cellSize.x * 0.866f;
                var scale = hexSize * 1.5f; 
                rain.transform.localScale = Vector3.one * scale;

                // Auto-return to pool.
                var ps = rain.GetComponent<ParticleSystem>();
                EffectManager.instance.StartCoroutine(ReturnAfterDuration(rain, ps.main.duration));
            }
        }

        private static IEnumerator ReturnAfterDuration(GameObject effect, float duration)
        {
            yield return new WaitForSeconds(duration);
            effect.transform.localScale = Vector3.one; // Reset scale.
            EffectManager.instance.ReturnEffect("Rain", effect);
        }
    }
}