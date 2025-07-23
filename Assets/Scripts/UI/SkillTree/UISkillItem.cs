using Player.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISkillItem : MonoBehaviour
    {
        [SerializeField] public Skill skill;
        public Sprite unlockedImg;
        public Sprite availableImg;
        public Sprite unavailableImg;

        private Image img;
        private UnityEngine.UI.Outline outline;

        private void Awake()
        {
            UIEvents.UIOpen.OnOpenSkillTree += UpdateSkillItem;
            UIEvents.UIOpen.OnOpenSkillItem += ShowSkillItem;
        }

        private void OnDisable()
        {
            UIEvents.UIOpen.OnOpenSkillTree -= UpdateSkillItem;
            UIEvents.UIOpen.OnOpenSkillItem -= ShowSkillItem;
        }

        private void Start()
        {
            img = GetComponent<Image>();
            outline = GetComponent<UnityEngine.UI.Outline>();
            outline.enabled = false;
            img.sprite = availableImg;
            img.color = skill.uiColor;
            var icon = transform.GetChild(0).GetComponent<Image>();
            icon.sprite = skill.icon;
        }

        private void UpdateSkillItem(PlayerSkillSet skillSet)
        { 
            if (skillSet.IsSkillUnlocked(skill))
            {
                img.sprite = unlockedImg;
            }
            else if (skillSet.CheckSkillRequirement(skill))
            {
                img.sprite = availableImg;
            } 
            else 
            {
                img.sprite = unavailableImg;
            }
        }

        private void ShowSkillItem(Skill skill)
        {
            outline.enabled = skill == this.skill;
        }
        
        public void OnClick()
        {
            UIEvents.UIOpen.OnOpenSkillItem.Invoke(skill);
        }
    }
}