using Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tutorial
{
    public class Tutorial : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject[] tutorialScreens;
        
        private int _currentScreen;
    
        private bool _requiresNpcClick;
        private bool _requiresSkillBuy;
        private bool _requiresBaseSkillBuy;
        private bool _requiresSkillOpen;
        private bool _requiresSkillUse;

        public void Initialize()
        {
            if (!TutorialManager.HasTutorial) return;
            
            gameObject.SetActive(true);
        }
    
        private void Awake()
        {
            UIEvents.UIOpen.OnOpenNpcMenu += arg0 =>
            {
                if (_currentScreen != 2) return;
            
                _requiresNpcClick = false;
                Advance();
            };
        
            UIEvents.UIOpen.OnOpenSkillTree += arg0 =>
            {
                if (_currentScreen != 9) return;
            
                _requiresSkillOpen = false;
                Advance();
            };
        
            UIEvents.UIOpen.OnBuySkill += () =>
            {
                if (_currentScreen == 10)
                {
                    _requiresBaseSkillBuy = false;
                    Advance();
                } else if (_currentScreen == 11)
                {
                    _requiresSkillBuy = false;
                    Advance();
                }
            };
        
            UIEvents.UIOpen.OnUseSkill += () =>
            {
                if (_currentScreen != 12) return;
            
                _requiresSkillUse = false;
                Advance();
            };
        }

        private void OnDestroy()
        {
            UIEvents.UIOpen.OnOpenNpcMenu -= arg0 =>
            {
                if (_currentScreen != 2) return;
            
                _requiresNpcClick = false;
                Advance();
            };
        
            UIEvents.UIOpen.OnOpenSkillTree -= arg0 =>
            {
                if (_currentScreen != 9) return;
            
                _requiresSkillOpen = false;
                Advance();
            };
        
            UIEvents.UIOpen.OnBuySkill -= () =>
            {
                if (_currentScreen == 10)
                {
                    _requiresBaseSkillBuy = false;
                    Advance();
                } else if (_currentScreen == 11)
                {
                    _requiresSkillBuy = false;
                    Advance();
                }
            };
        
            UIEvents.UIOpen.OnUseSkill -= () =>
            {
                if (_currentScreen != 12) return;
            
                _requiresSkillUse = false;
                Advance();
            };
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Advance();
        }

        private void Advance()
        {
            if (_requiresNpcClick || _requiresSkillOpen || _requiresSkillBuy || _requiresSkillUse || _requiresBaseSkillBuy) return;
            if (_currentScreen > tutorialScreens.Length - 2)
            {
                gameObject.SetActive(false);
                return;
            }
        
            tutorialScreens[_currentScreen].SetActive(false);
            tutorialScreens[_currentScreen+1].SetActive(true);
        
            _currentScreen++;
        
            if (_currentScreen == 2)
                HandleNpcTutorial();

            if (_currentScreen == 9)
                HandleSkillOpenTutorial();
        
            if (_currentScreen == 10)
                HandleBaseSkillBuyTutorial();
            
            if (_currentScreen == 11)
                HandleSkillBuyTutorial();
        
            if (_currentScreen == 12)
                HandleSkillUseTutorial();
        }

        private void HandleSkillUseTutorial()
        {
            _requiresSkillUse = true;
        }

        private void HandleBaseSkillBuyTutorial()
        {
            GameEvents.InfluencePoints.GainInfluencePoints.Invoke(1000);
            _requiresBaseSkillBuy = true;
        }
        
        private void HandleSkillBuyTutorial()
        {
            _requiresSkillBuy = true;
        }

        private void HandleSkillOpenTutorial()
        {
            _requiresSkillOpen = true;
        }

        private void HandleNpcTutorial()
        {
            _requiresNpcClick = true;

            var randomCivi = GameObject.FindGameObjectsWithTag("Civi")[0];
            GameEvents.Camera.OnJumpToCiv.Invoke(randomCivi);
        }
    }
}
