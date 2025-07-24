using System;
using Models;
using System.Collections;
using Events;
using Player;
using Player.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIMessiahMenu : UpdateableMenu<NPCModel>
    {
        public PlayerController _playerController;
        [SerializeField] private TextMeshProUGUI description;
        
        [SerializeField] private Image PreachButton;
        private bool preachToggle = false;
        [SerializeField] private Image SendButton;
        private bool sendToggle = false;

        [SerializeField] private Sprite buttonSprite;
        [SerializeField] private Sprite toggledButtonSprite;
        public override void Initialize()
        {
            GameEvents.Civilization.OnPreach += blockButtons;
            GameEvents.Civilization.OnPreachEnd += deblockButtons;
            UIEvents.UIOpen.OnOpenMessiahMenu += OnOpenNpcMenu;
            UIEvents.UIOpen.OnSelectCityMessiahAction += SelectedCity;
            UIEvents.UIOpen.OnOpenNpcMenu += _ => OnClose();
            UIEvents.UIOpen.OnOpenSkillTree += _ => OnClose();
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += CleanupOnSceneChange;
        }
        
        private void CleanupOnSceneChange(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name != "WorldMap") return;
            
            GameEvents.Civilization.OnPreach -= blockButtons;
            GameEvents.Civilization.OnPreachEnd -= deblockButtons;
            UIEvents.UIOpen.OnOpenMessiahMenu -= OnOpenNpcMenu;
            UIEvents.UIOpen.OnSelectCityMessiahAction -= SelectedCity;
            UIEvents.UIOpen.OnOpenNpcMenu -= _ => OnClose();
            UIEvents.UIOpen.OnOpenSkillTree -= _ => OnClose();
        }

        private void blockButtons(GameObject arg0)
        {
            PreachButton.gameObject.GetComponent<Button>().interactable = false;
            SendButton.gameObject.GetComponent<Button>().interactable = false;
        }
        private void deblockButtons(GameObject arg0)
        {
            PreachButton.gameObject.GetComponent<Button>().interactable = true;
            SendButton.gameObject.GetComponent<Button>().interactable = true;
        }
        
        private void OnOpenNpcMenu(NPCModel npcModel)
        {
            OnOpen(npcModel);
            togglePreach(false);
            toggleSend(false);
        }
    
        protected override void UpdateData(NPCModel data)
        {
            // npcName.text = data.NPCName;
        }

        private void Update()
        {
            if ((sendToggle || preachToggle) && Input.GetMouseButtonDown(1))
            {
                togglePreach(false);
                toggleSend(false);
            }
        }

        public void ButtonPressSendSaviour()
        {
            sendToggle = !sendToggle;
            toggleSend(sendToggle);
            if (sendToggle)
            {
                _playerController.callAbility(AbilityType.SendSaviour);
            }
        }
        
        public void toggleSend(bool isActive)
        {
            sendToggle = isActive;
            description.text = isActive? "Send your saviour to a selected location." : "";
            SendButton.sprite = isActive? toggledButtonSprite : buttonSprite;
            if (!isActive)
            {
                _playerController.cancelAbility.Invoke();
            }
        }
        
        
        public void ButtonPressMessiahAction()
        {
            preachToggle = !preachToggle;
            togglePreach(preachToggle);
            if (preachToggle)
            {
                UIEvents.UIVar.isCastingSaviourAction = true;
            }
        }

        public void togglePreach(bool isActive)
        {
            preachToggle = isActive;
            description.text = isActive? "Send your saviour to a struggling city, to improve their needs." : "";
            PreachButton.sprite = isActive? toggledButtonSprite : buttonSprite;
            if (!isActive)
            { 
                UIEvents.UIVar.isCastingSaviourAction = false;
            }
        }
        
        private void SelectedCity(Civilization civ)
        {
            if (!UIEvents.UIVar.isCastingSaviourAction || civ == null) return;
            UIEvents.UIVar.isCastingSaviourAction = false;
            InvokeMessiahAction(civ);
        }
       
        private void InvokeMessiahAction(Civilization civ)
        {
            Messiah.UseMessiah.Invoke(civ);
            togglePreach(false);
        }
        
    }
}
