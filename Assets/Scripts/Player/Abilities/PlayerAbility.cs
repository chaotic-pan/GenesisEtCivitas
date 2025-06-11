using UnityEngine;

namespace Player.Abilities
{
    public enum AbilityType
    {
        Rain,
        Earthquake
    }
    
    public class PlayerAbility : MonoBehaviour
    {
        public virtual int Cost { get; set; }

        public virtual AbilityType Type { get; set; }

        public virtual void EnterAbility() {}
        
        public virtual void CastAbility(Vector3Int tileGridPos) {}
    }
}