using System;
using TMPro;
using UnityEngine;

namespace UI.HUD
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIInfluencePoints : MonoBehaviour
    {
        
        private TextMeshProUGUI _text;
        
        private void Awake()
        {
            GameEvents.InfluencePoints.OnChangeInfluencePoints += OnUpdateIP;
            _text = gameObject.GetComponent<TextMeshProUGUI>();
        }

        private void OnUpdateIP(int influencePoints)
        {
            _text.text = influencePoints.ToString();
        }
    }
}
