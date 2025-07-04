using UnityEngine;

namespace Player.Abilities
{
    public enum AbilityType
    {
        Rain,
        Earthquake,
        SendSaviour
    }
    
    public class PlayerAbility : MonoBehaviour
    {
        public virtual int Cost { get; set; }
        public virtual AbilityType Type { get; set; }
        public virtual int EffectDiameter { get; } = 1;

        public virtual void EnterAbility() {}
        public virtual void CastAbility(Vector3Int tileGridPos) {}
    }
}