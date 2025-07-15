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
    }
}