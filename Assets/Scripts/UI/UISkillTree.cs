using System;
using Player.Skills;
using UnityEngine;

public class UISkillTree : MonoBehaviour
{

    private PlayerSkillSet _playerSkillSet;

    // Water Skill set
    public void UnlockWaterOne()
    {
        if (!_playerSkillSet.TryUnlockSkill(PlayerSkillSet.Skill.WaterOne))
        {
            Debug.LogError("Cannot Unlock Yet!");
        }
    }

    public void UnlockWaterTwo()
    {
        if (!_playerSkillSet.TryUnlockSkill(PlayerSkillSet.Skill.WaterTwo))
        {
            Debug.LogError("Cannot Unlock Yet!");
        }
    }

    //Death Skill set
    public void UnlockDeathOne()
    {
        if (!_playerSkillSet.TryUnlockSkill(PlayerSkillSet.Skill.DeathOne))
        {
            Debug.LogError("Cannot Unlock Yet!");
        }
    }

    public void UnlockDeathTwo()
    {
         if (!_playerSkillSet.TryUnlockSkill(PlayerSkillSet.Skill.DeathTwo))
        {
            Debug.LogError("Cannot Unlock Yet!");
        }
    }

    public void SetPlayerSkills(PlayerSkillSet playerSkillSet)
    {
        this._playerSkillSet = playerSkillSet;
    }


}
