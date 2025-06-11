using UnityEngine;

namespace Player.Abilities
{
    public class EarthquakeAbility : PlayerAbility
    {
        public override int Cost => 100;

        public override AbilityType Type => AbilityType.Earthquake;
        
        public int effectDiameter = 5;

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