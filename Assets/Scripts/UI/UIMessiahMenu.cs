using Models;
using System.Collections;
using Player;
using Player.Abilities;
using UnityEngine;

namespace UI
{
    public class UIMessiahMenu : UpdateableMenu<NPCModel>
    {
        private bool isSelectingCity;
        public PlayerController _playerController;

        private IEnumerator coroutine;
        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenMessiahMenu += OnOpenNpcMenu;
            UIEvents.UIOpen.OnSelectCityMessiahAction += SelectedCity;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += CleanupOnSceneChange;
        }
        
        private void CleanupOnSceneChange(UnityEngine.SceneManagement.Scene scene)
        {
            UIEvents.UIOpen.OnOpenMessiahMenu -= OnOpenNpcMenu;
            UIEvents.UIOpen.OnSelectCityMessiahAction -= SelectedCity;
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
            if (!isSelectingCity || civ == null) return;
            StopCoroutine(coroutine);
            isSelectingCity = false;
            Debug.Log("City found");
            InvokeMessiahAction(civ);
        }
        IEnumerator SelectCity(float timer)
        {
            isSelectingCity = true;
            yield return new WaitForSeconds(timer);
            isSelectingCity = false;
            Debug.Log("No city found");
            
        }
        private void InvokeMessiahAction(Civilization civ)
        {
            Debug.Log(civ + " is currently in InvokeMessiahAction im UIMessiahMenu");
            Messiah.UseMessiah.Invoke(civ);
        }
    }
}
