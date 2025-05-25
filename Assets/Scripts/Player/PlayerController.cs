using Player.Abilities;
using Player.Skills;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel _playerModel;
        private PlayerSkillSet _playerSkillSet;

        private void Start()
        {
            _playerModel = GetComponent<PlayerModel>();
            _playerSkillSet = new PlayerSkillSet();
        }

        public void CastRainAbility()
        {
            CastAbility(AbilityType.Rain);
        }

        public void CastEarthquakeAbility()
        {
            CastAbility(AbilityType.Earthquake);
        }

        private void CastAbility(AbilityType type)
        {
            PlayerAbility ability = type switch
            {
                AbilityType.Rain => gameObject.AddComponent<RainAbility>(),
                AbilityType.Earthquake => gameObject.AddComponent<EarthquakeAbility>(),
                _ => null
            };
            if (!ability) return;

            ability.CastAbility();

            if (_playerModel.influencePoints < ability.Cost)
            {
                Debug.Log("Too little CASH dumb ass");
                return;
            }
            _playerModel.influencePoints -= ability.Cost;
            
            Debug.Log("Cost: " + ability.Cost);

            Destroy(ability);
        }

        public bool CanUseWaterOne()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.WaterOne);
        }
    }
}