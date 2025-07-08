using Player.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISkill : MonoBehaviour
    {
        [SerializeField] public Skill skill;
        public Sprite unlockedImg;
        public Sprite availableImg;
        public Sprite unavailableImg;
        public UISkillTree tree;

        private Image img;
        private bool clickable = true;

        private void Awake()
        {
            UIEvents.UIOpen.OnOpenSkillTree += UpdateSkillItem;
        }

        private void OnDisable()
        {
            UIEvents.UIOpen.OnOpenSkillTree -= UpdateSkillItem;
        }

        private void Start()
        {
            img = GetComponent<Image>();
            img.sprite = availableImg;
            img.color = skill.uiColor;
        }

        private void UpdateSkillItem(PlayerSkillSet skillSet)
        { 
            if (skillSet.IsSkillUnlocked(skill))
            {
                img.sprite = unlockedImg;
                clickable = false;
            }
            else if (skillSet.CheckSkillRequirement(skill))
            {
                img.sprite = availableImg;
                clickable = true;
            } 
            else 
            {
                img.sprite = unavailableImg;
                clickable = false;
            }
        }
        
        public void OnClick()
        {
            if (clickable) tree.UnlockSkill(skill);
        }

        public void OnHover()
        {
            tree.Test(skill);
        }
    }
}