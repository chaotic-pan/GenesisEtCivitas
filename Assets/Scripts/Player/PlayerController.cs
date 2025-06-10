using Models;
using Player.Abilities;
using Player.Skills;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel _playerModel;
        private PlayerSkillSet _playerSkillSet;

        private void Awake()
        {
            _playerModel = GetComponent<PlayerModel>();
            _playerSkillSet = new PlayerSkillSet();
            _playerSkillSet.OnSkillUnlocked += PlayerSkillSet_OnSkillUnlocked;
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

            if (_playerModel.InfluencePoints < ability.Cost)
            {
                Debug.Log("Too little CASH dumb ass");
                return;
            }
            _playerModel.InfluencePoints -= ability.Cost;

            Debug.Log("Cost: " + ability.Cost);

            Destroy(ability);
        }

        //Skills

        private void PlayerSkillSet_OnSkillUnlocked(object sender, PlayerSkillSet.OnSkillUnlockedEventArgs e)
        {
            switch (e.skill)
            {
                case PlayerSkillSet.Skill.WaterOne:
                    SetWaterOne();
                    break;
                case PlayerSkillSet.Skill.DeathOne:
                    SetDeathOne();
                    break;
                case PlayerSkillSet.Skill.WaterTwo:
                    SetWaterTwo();
                    break;
                case PlayerSkillSet.Skill.DeathTwo:
                    SetDeathTwo();
                    break;
            }
        }

        public PlayerSkillSet GetPlayerSkillSet()
        {
            return _playerSkillSet;
        }

        // Water Skills
        public bool CanUseWaterOne()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.WaterOne);
        }
        private void SetWaterOne()
        {
            Debug.Log("Water One Unlocked");
        }

        public bool CanUseWaterTwo()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.WaterTwo);
        }
        private void SetWaterTwo()
        {
            Debug.Log("Water Two Unlocked");
        }

        // Death Skill set
        public bool CanUseDeathOne()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.DeathTwo);
        }
        private void SetDeathOne()
        {
            Debug.Log("Death One Unlocked");
        }
        
         public bool CanUseDeathTwo()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.DeathTwo);
        }
        private void SetDeathTwo()
        {
            Debug.Log("Death Two Unlocked");
        }
    }
}