using UnityEngine;

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
        }
        
        public override void CastAbility(Vector3Int tileGridPos)
        {
            // Debug.Log("Casted Earthquake Ability");
            
        }
    }
}