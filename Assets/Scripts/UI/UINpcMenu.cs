using System;
using System.Collections.Generic;
using CityStuff;
using Events;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UINpcMenu : UpdateableMenu<NPCModel>
    {
        [Header("Textfields")]
        [SerializeField] private TextMeshProUGUI npcName;
        [SerializeField] private TextMeshProUGUI influenceText;
        [SerializeField] private TextMeshProUGUI foodText;
        [SerializeField] private TextMeshProUGUI waterText;
        [SerializeField] private TextMeshProUGUI safetyText;
        [SerializeField] private TextMeshProUGUI shelterText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI populationText;

        [Header("Tabs")]
        [SerializeField] private List<GameObject> civObjects;
        [SerializeField] private List<GameObject> cityObjects;
        [SerializeField] private GameObject cityTab;
        [SerializeField] private GameObject civTab;
        [SerializeField] private Sprite tabActive;
        [SerializeField] private Sprite tabInactive;
        
        private NPCModel model;
        
        
        public override void Initialize()
        {
           UIEvents.UIOpen.OnOpenNpcMenu += OnOpenNpcMenu;
           UnityEngine.SceneManagement.SceneManager.sceneUnloaded += CleanupOnSceneChange; // Cleanup because OnDestroy is not called if not enabled
        }
        
        private void CleanupOnSceneChange(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name == "WorldMap")
                UIEvents.UIOpen.OnOpenNpcMenu -= OnOpenNpcMenu;
        }

        private void OnOpenNpcMenu(NPCModel npcModel)
        {
            OnOpen(npcModel);
            model = npcModel;

            OnOpenCivTab();
        }

        public void SwitchToSaviour()
        {
            OnClose();
            UIEvents.UIOpen.OnOpenMessiahMenu.Invoke(model);
        }
    
        protected override void UpdateData(NPCModel data)
        {
            influenceText.text = ((int)Mathf.Round(data.Faith)).ToString();
            foodText.text = ((int)Mathf.Round(data.Food)).ToString();
            waterText.text = ((int)Mathf.Round(data.Water)).ToString();
            safetyText.text = ((int)Mathf.Round(data.Safety)).ToString();
            shelterText.text = ((int)Mathf.Round(data.Shelter)).ToString();
            energyText.text = ((int)Mathf.Round(data.Energy)).ToString();
            populationText.text = data.Population.ToString();
            
            if (this == null) Debug.LogError("This UINpcMenu reference is destroyed!");
            if (cityTab == null) Debug.LogError("cityTab is null (destroyed or never assigned).");
            
            cityTab.SetActive(data.City != null);
        }

        public void ButtonPressMessiah()
        {
            model.IsMessiah = true;
            NPC.CheckMessiah.Invoke();
        }
        public void OnBuildChurch()
        {
            if (!model.City) return;
            model.City.BuildChurch();
        }
        
        public void OnBuildWell()
        {
            if (!model.City) return;
            model.City.BuildWell();
        }

        public void OnJumpToCiv()
        {
            GameEvents.Camera.OnJumpToCiv(model.NPC);
        }

        public void OnOpenCivTab()
        {
            foreach (var civObject in civObjects)
            {
                civObject.SetActive(true);
            }
            
            foreach (var cityObject in cityObjects)
            {
                cityObject.SetActive(false);
            }

            npcName.text = model.NPCName;

            civTab.GetComponent<Image>().sprite = tabActive;
            cityTab.GetComponent<Image>().sprite = tabInactive;
        }
        
        public void OnOpenCityTab()
        {
            foreach (var civObject in civObjects)
            {
                civObject.SetActive(false);
            }
            
            foreach (var cityObject in cityObjects)
            {
                cityObject.SetActive(true);
            }
            
            npcName.text = model.City.CityName;
            
            civTab.GetComponent<Image>().sprite = tabInactive;
            cityTab.GetComponent<Image>().sprite = tabActive;
        }
    }
}
