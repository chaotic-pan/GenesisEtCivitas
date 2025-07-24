using System;
using Player;
using Player.Abilities;
using Player.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAbilityPanel : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        public PlayerController _playerController;
        [SerializeField] private Skill onUnlockRain;
        [SerializeField] private Skill onUnlockEarthquake;
        [SerializeField] private Skill onUnlockPlantGrowth;

        private void Awake()
        {
            onUnlockRain.onUnlocked += unlockRain;
            onUnlockEarthquake.onUnlocked += unlockEarthquake;
            onUnlockPlantGrowth.onUnlocked += unlockPlantGrowth;
        }

        private void OnDisable()
        {
            onUnlockRain.onUnlocked -= unlockRain;
            onUnlockEarthquake.onUnlocked -= unlockEarthquake;
            onUnlockPlantGrowth.onUnlocked -= unlockPlantGrowth;
        }

        private void SpawnAbilityButton(int cost, Sprite image,  AbilityType ability)
        {
            var but = Instantiate(buttonPrefab, transform);
            var icon = but.transform.GetChild(0).GetComponent<Image>();
            icon.sprite = image;
            var costTxt = but.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            costTxt.text = $"{cost} IP";
            but.GetComponent<Button>().onClick.AddListener(() => _playerController.callAbility(ability));
            var abilityButton = but.GetComponent<UiAbilityButton>();
            abilityButton.cost = cost;
            abilityButton.CostCheck(_playerController._playerModel);
            
        }

        private void unlockRain()
        {
            SpawnAbilityButton(100, onUnlockRain.icon, AbilityType.Rain);
        }
        
        private void unlockEarthquake()
        {
            SpawnAbilityButton(200, onUnlockEarthquake.icon, AbilityType.Earthquake);
        }
        
        private void unlockPlantGrowth()
        {
            SpawnAbilityButton(200, onUnlockPlantGrowth.icon, AbilityType.PlantGrowth);
        }
    }
}