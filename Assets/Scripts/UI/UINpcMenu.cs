using System;
using System.Collections.Generic;
using System.Linq;
using CityStuff;
using Events;
using Models;
using Player.Skills;
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
        [SerializeField] private Button SaviourButton;

        [Header("Tabs")]
        [SerializeField] private List<GameObject> civObjects;
        [SerializeField] private List<GameObject> cityObjects;
        [SerializeField] private GameObject cityTab;
        [SerializeField] private GameObject civTab;
        [SerializeField] private Sprite tabActive;
        [SerializeField] private Sprite tabInactive;
        
        [Header("Locked Buildings")]
        [SerializeField] private List<GameObject> lockedBuilding;
        [SerializeField] private Skill onUnlockWell;
        
        [Header("Buttons")]
        [SerializeField] private GameObject BuildWellbutton;
        [SerializeField] private GameObject BuildChurchButton;
        
        private NPCModel model;
        
        public override void Initialize()
        {
           UIEvents.UIOpen.OnOpenNpcMenu += OnOpenNpcMenu;
           UIEvents.UIOpen.OnOpenMessiahMenu += _ => OnClose();
           UIEvents.UIOpen.OnOpenSkillTree += _ => OnClose();
           UnityEngine.SceneManagement.SceneManager.sceneUnloaded += CleanupOnSceneChange; // Cleanup because OnDestroy is not called if not enabled

           onUnlockWell.onUnlocked += UnlockWell;
           foreach (var build in lockedBuilding.Where(build => cityObjects.Contains(build)))
           {
               cityObjects.Remove(build);
           }
        }

        private void CleanupOnSceneChange(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name == "WorldMap")
            {
                UIEvents.UIOpen.OnOpenNpcMenu -= OnOpenNpcMenu;
                onUnlockWell.onUnlocked -= UnlockWell;
                UIEvents.UIOpen.OnOpenMessiahMenu -= _ => OnClose();
                UIEvents.UIOpen.OnOpenSkillTree -= _ => OnClose();
            }
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
            
            BuildChurchButton.SetActive(false);
        }
        
        public void OnBuildWell()
        {
            if (!model.City) return;
            model.City.BuildWell();
            
            BuildWellbutton.SetActive(false);
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

            SaviourButton.gameObject.SetActive(!UIEvents.UIVar.saviourExists);
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
                if (cityObject.name == "BuildChurch" && model.City._church != null)
                    continue;
                
                if (cityObject.name == "BuildWell" && model.City._well != null)
                    continue;
                    
                cityObject.SetActive(true);
            }
            
            npcName.text = model.City.CityName;
            
            civTab.GetComponent<Image>().sprite = tabInactive;
            cityTab.GetComponent<Image>().sprite = tabActive;
        }
        
        public void UnlockWell()
        {
            foreach (var build in lockedBuilding.Where(build => build.ToString().Contains("Well")))
            {
                cityObjects.Add(build);
                return;
            }
        }
    }
}
