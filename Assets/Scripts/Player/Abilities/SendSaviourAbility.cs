using UnityEngine;

namespace Player.Abilities
{
    public class SendSaviourAbility : PlayerAbility
    {
        public override int Cost => 0;
        public override AbilityType Type => AbilityType.SendSaviour;
        public override int EffectDiameter => 1;

        public override void EnterAbility()
        {
            GridOverlayManager.Instance.aoeHighlightColor = new Color(1, 0.75f, .25f, 0.7f);
        }

        public override void CastAbility(Vector3Int centerTilePos)
        {
            Messiah.SendMessiah?.Invoke(centerTilePos);
        }
    }
}