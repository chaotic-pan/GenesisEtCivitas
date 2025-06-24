using System;
using Player;
using Player.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAbilityPanel : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        public PlayerController _playerController;

        public void SpawnAbilityButton(String label, AbilityType ability)
        {
            var but = Instantiate(buttonPrefab, transform);
            but.GetComponentInChildren<TextMeshProUGUI>().text = label;
            but.GetComponent<Button>().onClick.AddListener(() => _playerController.callAbility(ability));
        }
    }
}