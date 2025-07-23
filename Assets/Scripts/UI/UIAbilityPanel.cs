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

        private void SpawnAbilityButton(String label, Sprite image,  AbilityType ability)
        {
            var but = Instantiate(buttonPrefab, transform);
            var icon = but.transform.GetChild(0).GetComponent<Image>();
            icon.sprite = image;
            var cost = but.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            cost.text = label;
            but.GetComponent<Button>().onClick.AddListener(() => _playerController.callAbility(ability));
        }

        private void unlockRain()
        {
            SpawnAbilityButton("100 IP", onUnlockRain.icon, AbilityType.Rain);
        }
        
        private void unlockEarthquake()
        {
            SpawnAbilityButton("200 IP", onUnlockEarthquake.icon, AbilityType.Earthquake);
        }
        
        private void unlockPlantGrowth()
        {
            SpawnAbilityButton("200 IP", onUnlockPlantGrowth.icon, AbilityType.PlantGrowth);
        }
    }
}