using Player;
using Player.Abilities;
using Player.Skills;
using TMPro;
using UI;
using UnityEngine;

public class UISkillTree : MonoBehaviour
{

    public PlayerSkillSet playerSkillSet;
    [SerializeField] private UIAbilityPanel uiAbilityPanel;
    public TextMeshProUGUI descriptionText;

    private CanvasGroup overlay;
    private bool active = false;

    private void Start()
    {
        playerSkillSet = PlayerController.instance._playerSkillSet;
        overlay = transform.GetChild(0).GetComponent<CanvasGroup>();
        setCanvasGroupActive(false);
    }

    private void setCanvasGroupActive(bool isActive)
    {
        overlay.alpha = isActive ? 1 : 0;
        overlay.interactable = isActive;
        overlay.blocksRaycasts = isActive;
        active = isActive;
    }

    public void ToggleVisibility()
    {
        if (active) setCanvasGroupActive(false);
        else
        { 
            setCanvasGroupActive(true);
            UIEvents.UIOpen.OnOpenSkillTree.Invoke(playerSkillSet);
        }
    }

    public bool UnlockSkill(PlayerSkillSet.Skill skill)
    {
        if (playerSkillSet.TryUnlockSkill(skill))
        {
            uiAbilityPanel.SpawnAbilityButton("RAIN", AbilityType.Rain);
            UIEvents.UIOpen.OnOpenSkillTree.Invoke(playerSkillSet);
            return true;
        }

        return false;
    }

    public void Test(PlayerSkillSet.Skill skill)
    {
        descriptionText.text = $"this is the {skill} skill, it costs {playerSkillSet.GetSkillCosts(skill)} VP" ;
    }


}
