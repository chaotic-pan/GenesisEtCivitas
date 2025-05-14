using UnityEngine;

namespace Player.Abilities
{
    public class RainAbility : PlayerAbility
    {
        public override int Cost => 30;

        public override AbilityType Type => AbilityType.Rain;

        public override void CastAbility()
        {
            Debug.Log("Casted Rain Ability");
        }
    }
}