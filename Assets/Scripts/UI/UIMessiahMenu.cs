using Models;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMessiahMenu : UpdateableMenu<NPCModel>
    {
        private bool isSelectingCity;

        private IEnumerator coroutine;
        public override void Initialize()
        {
            UIEvents.UIOpen.OnOpenMessiahMenu += OnOpenNpcMenu;
            UIEvents.UIOpen.OnSelectCityMessiahAction += SelectedCity;
        }

        private void OnOpenNpcMenu(NPCModel npcModel)
        {
            OnOpen(npcModel);
        }
    
        protected override void UpdateData(NPCModel data)
        {
            // npcName.text = data.NPCName;
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
            NPC.UseMessiah.Invoke("GrantScoreImprovements", civ);
        }
    }
}
