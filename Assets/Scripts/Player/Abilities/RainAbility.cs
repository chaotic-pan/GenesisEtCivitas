using UnityEngine;

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

        public override void CastAbility(Vector3Int tileGridPos)
        {
            // Debug.Log("Casted Rain Ability");
            var tileData = TileManager.Instance.getTileDataByGridCoords(tileGridPos);
            tileData.waterValue += 20;
        }
    }
}