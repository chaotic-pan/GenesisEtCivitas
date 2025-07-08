using System;
using System.Collections.Generic;
using Models;

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
        
        public bool TryUnlockSkill(Skill skill)
        {
            if (!CheckSkillRequirement(skill)) return false;
            
            int skillCost = skill.cost;
            if (skillCost > _playerModel.virtuePoints) return false;
            
            UnlockSkill(skill);
            _playerModel.virtuePoints -= skillCost;
            return true;

        }

        public bool CheckSkillRequirement(Skill skill)
        {
            Skill skillRequirement = skill.requirement;
            return skillRequirement == null || IsSkillUnlocked(skillRequirement);
        }
    }
}