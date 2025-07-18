using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Player.Abilities
{
    public enum AbilityType
    {
        Rain,
        Earthquake,
        SendSaviour,
        PlantGrowth
    }
    
    public class PlayerAbility : MonoBehaviour
    {
        public virtual int Cost { get; set; }
        public virtual AbilityType Type { get; set; }
        public virtual int EffectDiameter { get; } = 1;
        public virtual float InfluenceAmount { get; } = 10f;

        public virtual void EnterAbility() {}
        public virtual void CastAbility(Vector3Int tileGridPos) {}
        
        protected void SpawnEffectOnTiles(List<Vector3Int> tiles, string effectName, float heightOffset = 20f, float scaleMultiplier = 1.5f)
        {
            if (EffectManager.instance == null) return;

            foreach (var tilePos in tiles)
            {
                var worldPos = TileManager.Instance.map.GetCellCenterWorld(tilePos);
                var tileData = TileManager.Instance.getTileDataByGridCoords(tilePos);
                if (tileData == null) continue;

                // Position effect above the tile.
                var spawnPos = new Vector3(
                    worldPos.x,
                    tileData.height + heightOffset,
                    worldPos.z
                );

                var effect = EffectManager.instance.GetEffect(effectName);
                if (effect == null) continue;

                effect.transform.position = spawnPos;
        
                // Scale to cover the tile.
                var hexSize = TileManager.Instance.map.cellSize.x * 0.866f;
                var scale = hexSize * scaleMultiplier; 
                effect.transform.localScale = Vector3.one * scale;

                var ps = effect.GetComponent<ParticleSystem>();
                StartCoroutine(ReturnAfterDuration(effect, effectName, ps.main.duration));
            }
        }

        private IEnumerator ReturnAfterDuration(GameObject effect, string effectName, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (effect == null) yield break;
    
            effect.transform.localScale = Vector3.one;
            EffectManager.instance.ReturnEffect(effectName, effect);
        }
    }
}