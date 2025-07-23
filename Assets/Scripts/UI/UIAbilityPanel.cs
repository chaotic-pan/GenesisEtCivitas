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

        private void SpawnAbilityButton(String label, AbilityType ability)
        {
            var but = Instantiate(buttonPrefab, transform);
            but.GetComponentInChildren<TextMeshProUGUI>().text = label;
            but.GetComponent<Button>().onClick.AddListener(() => _playerController.callAbility(ability));
        }

        private void unlockRain()
        {
            SpawnAbilityButton("RAIN (100 IP)", AbilityType.Rain);
        }
        
        private void unlockEarthquake()
        {
            SpawnAbilityButton("EARTHQUAKE (200 IP)", AbilityType.Earthquake);
        }
        
        private void unlockPlantGrowth()
        {
            SpawnAbilityButton("PLANT GROWTH (200 IP)", AbilityType.PlantGrowth);
        }
    }
}