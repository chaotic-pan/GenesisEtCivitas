using UnityEngine;

namespace Player.Abilities
{
    public class EarthquakeAbility : PlayerAbility
    {
        public override int Cost => 100;

        public override AbilityType Type => AbilityType.Earthquake;

        public override void CastAbility()
        {
            Debug.Log("Casted Earthquake Ability");
        }
    }
}