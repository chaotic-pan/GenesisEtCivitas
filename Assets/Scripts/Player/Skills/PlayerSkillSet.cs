using System;
using System.Collections.Generic;

namespace Player.Skills
{
    public class PlayerSkillSet
    {

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

        public PlayerSkillSet()
        {
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

        public bool TryUnlockSkill(Skill skill)
        {
            Skill skillRequirement = GetSkillRequirement(skill);

            if (skillRequirement != Skill.None)
            {
                if (IsSkillUnlocked(skillRequirement))
                {
                    UnlockSkill(skill);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                UnlockSkill(skill);
                return true;
            }
        }
    }
}