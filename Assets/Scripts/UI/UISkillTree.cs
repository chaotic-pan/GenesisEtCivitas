using System;
using Player.Abilities;
using Player.Skills;
using UI;
using UnityEngine;

public class UISkillTree : MonoBehaviour
{

    private PlayerSkillSet playerSkillSet;
    [SerializeField] private UIAbilityPanel uiAbilityPanel;

    private bool UnlockSkill(PlayerSkillSet.Skill skill)
    {
        return playerSkillSet.TryUnlockSkill(skill);
    }
    
    // Water Skill set
    public void UnlockWaterOne()
    {
        if (UnlockSkill(PlayerSkillSet.Skill.WaterOne))
        {
            uiAbilityPanel.SpawnAbilityButton("RAIN", AbilityType.Rain);   
        }
    }

    public void UnlockWaterTwo()
    {
        UnlockSkill(PlayerSkillSet.Skill.WaterTwo);
    }

    //Death Skill set
    public void UnlockDeathOne()
    {
        if (UnlockSkill(PlayerSkillSet.Skill.DeathOne))
        {
            uiAbilityPanel.SpawnAbilityButton("EARTHQUAKE", AbilityType.Earthquake);   
        }
    }

    public void UnlockDeathTwo()
    {
        UnlockSkill(PlayerSkillSet.Skill.DeathTwo);
    }

    public void SetPlayerSkills(PlayerSkillSet playerSkillSet)
    {
        this.playerSkillSet = playerSkillSet;
    }


}
