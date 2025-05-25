using System.Collections.Generic;

namespace Player.Skills
{
    public class PlayerSkillSet
    {
        public enum Skill
        {
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

        public void UnlockSkill(Skill skill)
        {
            _unlockedSkills.Add(skill);
        }

        public bool IsSkillUnlocked(Skill skill)
        {
            return _unlockedSkills.Contains(skill);
        }
    }
}