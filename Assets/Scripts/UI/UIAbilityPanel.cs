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

        private void Awake()
        {
            onUnlockRain.onUnlocked += unlockRain;
            onUnlockEarthquake.onUnlocked += unlockEarthquake;
        }

        private void OnDisable()
        {
            onUnlockRain.onUnlocked -= unlockRain;
            onUnlockEarthquake.onUnlocked -= unlockEarthquake;
        }

        private void SpawnAbilityButton(String label, AbilityType ability)
        {
            var but = Instantiate(buttonPrefab, transform);
            but.GetComponentInChildren<TextMeshProUGUI>().text = label;
            but.GetComponent<Button>().onClick.AddListener(() => _playerController.callAbility(ability));
        }

        private void unlockRain()
        {
            SpawnAbilityButton("RAIN", AbilityType.Rain);
        }
        
        private void unlockEarthquake()
        {
            SpawnAbilityButton("EARTHQUAKE", AbilityType.Earthquake);
        }
    }
}