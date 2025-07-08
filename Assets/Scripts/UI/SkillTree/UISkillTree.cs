using Events;
using Models;
using Player;
using Player.Skills;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UISkillTree : MonoBehaviour
{

    public PlayerSkillSet playerSkillSet;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costsText;
    public Button buyButton;
    public Skill startSkill;

    private CanvasGroup overlay;
    private bool active = false;
    private Skill displayedSkill;
    

    private void Awake()
    {
        UIEvents.UIOpen.OnOpenSkillItem += UpdateDisplay;
        UIEvents.UIUpdate.OnUpdatePlayerData += CostCheck;
    }

    private void OnDisable()
    { 
        UIEvents.UIOpen.OnOpenSkillItem -= UpdateDisplay;
        UIEvents.UIUpdate.OnUpdatePlayerData -= CostCheck;
    }

    private void Start()
    {
        playerSkillSet = PlayerController.instance._playerSkillSet;
        overlay = transform.GetChild(0).GetComponent<CanvasGroup>();
        setCanvasGroupActive(false);

        displayedSkill = startSkill;
        UnlockSkill();
        UIEvents.UIOpen.OnOpenSkillItem.Invoke(startSkill);
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

    public void UnlockSkill()
    {
        if (!playerSkillSet.TryUnlockSkill(displayedSkill)) return;
        
        displayedSkill.Unlock();
        UIEvents.UIOpen.OnOpenSkillTree.Invoke(playerSkillSet);
        UpdateDisplay(displayedSkill);
    }

    public void UpdateDisplay(Skill skill)
    {
        displayedSkill = skill;
        titleText.text = skill.title ;
        descriptionText.text = skill.description ;

        if (playerSkillSet.IsSkillUnlocked(skill))
        {
            // unlocked already
            buyButton.gameObject.SetActive(false);
        }
        else if (playerSkillSet.CheckSkillRequirement(skill))
        {
            // can unlock 
            buyButton.gameObject.SetActive(true);
            buyButton.interactable = true;
                            
            costsText.text = $"{skill.cost} IP";
            costsText.color = Color.white;
            
            if (!playerSkillSet.CheckSkillCost(skill))
            {
                // too expensive
                buyButton.interactable = false;
                costsText.color = Color.gray;
            }
        } 
        else 
        {
            // unavailable
            buyButton.gameObject.SetActive(true);
            buyButton.interactable = false;
            
            costsText.text = "unavailable";
            costsText.color = Color.gray;
        }
    }
    
    private void CostCheck(PlayerModel pm)
    {
        if (displayedSkill.cost > pm.InfluencePoints)
        {
            // too expensive
            buyButton.interactable = false;
            costsText.color = Color.gray;
        }
        else
        {
            buyButton.interactable = true;
            costsText.color = Color.white;
        }
    }

}
