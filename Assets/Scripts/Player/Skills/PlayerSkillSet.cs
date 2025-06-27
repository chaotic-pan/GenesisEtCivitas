using System;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Player.Skills
{
    public class PlayerSkillSet
    {
        private PlayerModel _playerModel;

        public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;
        public class OnSkillUnlockedEventArgs : EventArgs
        {
            public Skill skill;
        }
        public enum Skill
        {
            None,
            WaterOne,
            WaterTwo,
            DeathOne,
            DeathTwo
        }

        public PlayerSkillSet(PlayerModel playerModel)
        {
            _playerModel = playerModel;
            _unlockedSkills = new List<Skill>();
        }


        private List<Skill> _unlockedSkills;

        private void UnlockSkill(Skill skill)
        {
            if (!IsSkillUnlocked(skill))
            {
                _unlockedSkills.Add(skill);
                OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { skill = skill });
            }
        }

        public bool IsSkillUnlocked(Skill skill)
        {
            return _unlockedSkills.Contains(skill);
        }

        public override string ToString()
        {
            return $"Unlocked Skills: {string.Join(", ", _unlockedSkills)}";
        }

        public Skill GetSkillRequirement(Skill skill)
        {
            switch (skill)
            {
                case Skill.WaterTwo: return Skill.WaterOne;
                case Skill.DeathTwo: return Skill.DeathOne;
            }
            return Skill.None;
        }

        public int GetSkillCosts(Skill skill)
        {
            switch (skill)
            {
                case Skill.WaterOne: return 1;
                case Skill.WaterTwo: return 2;
                case Skill.DeathOne: return 1;
                case Skill.DeathTwo: return 2;
            }
            return 0;
        }

        public bool TryUnlockSkill(Skill skill)
        {
            Skill skillRequirement = GetSkillRequirement(skill);
            int skillCost = GetSkillCosts(skill);

            if (IsSkillUnlocked(skill))
            {
                Debug.LogError("Already Unlocked!");
                return false;
            }

            if (skillRequirement != Skill.None && !IsSkillUnlocked(skillRequirement))
            {
                Debug.LogError("Not Available Yet!");
                return false;
            }

            if (skillCost > _playerModel.virtuePoints)
            {
                Debug.LogError("Not Enough Virtue Points!");
                return false;
            }

            UnlockSkill(skill);
            _playerModel.virtuePoints -= skillCost;
            return true;
        }
    }
}