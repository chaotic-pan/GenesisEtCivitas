using Models;
using System.Collections;
using Player;
using Player.Abilities;
using UnityEngine;

namespace UI
{
    public class UIMessiahMenu : UpdateableMenu<NPCModel>
    {
        public PlayerController _playerController;

        private IEnumerator coroutine;
        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenMessiahMenu += OnOpenNpcMenu;
            UIEvents.UIOpen.OnSelectCityMessiahAction += SelectedCity;
            UIEvents.UIOpen.OnOpenNpcMenu += _ => OnClose();
            UIEvents.UIOpen.OnOpenSkillTree += _ => OnClose();
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += CleanupOnSceneChange;
        }
        
        private void CleanupOnSceneChange(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name != "WorldMap") return;
            
            UIEvents.UIOpen.OnOpenMessiahMenu -= OnOpenNpcMenu;
            UIEvents.UIOpen.OnSelectCityMessiahAction -= SelectedCity;
            UIEvents.UIOpen.OnOpenNpcMenu -= _ => OnClose();
            UIEvents.UIOpen.OnOpenSkillTree -= _ => OnClose();
        }

        private void OnOpenNpcMenu(NPCModel npcModel)
        {
            OnOpen(npcModel);
        }
    
        protected override void UpdateData(NPCModel data)
        {
            // npcName.text = data.NPCName;
        }

        public void ButtonPressSendSaviour()
        {
            _playerController.callAbility(AbilityType.SendSaviour);
        }

        public void ButtonPressMessiahAction()
        {
            Debug.Log("Select a civilisation");

            coroutine = SelectCity(5);
            StartCoroutine(coroutine);
        }
        private void SelectedCity(Civilization civ)
        {
            if (!UIEvents.UIVar.isCastingSaviourAction || civ == null) return;
            StopCoroutine(coroutine);
            UIEvents.UIVar.isCastingSaviourAction = false;
            Debug.Log("City found");
            InvokeMessiahAction(civ);
        }
        IEnumerator SelectCity(float timer)
        {
            UIEvents.UIVar.isCastingSaviourAction = true;
            yield return new WaitForSeconds(timer);
            UIEvents.UIVar.isCastingSaviourAction = false;
            Debug.Log("No city found");
            
        }
        private void InvokeMessiahAction(Civilization civ)
        {
            Debug.Log(civ + " is currently in InvokeMessiahAction im UIMessiahMenu");
            Messiah.UseMessiah.Invoke(civ);
        }
    }
}
