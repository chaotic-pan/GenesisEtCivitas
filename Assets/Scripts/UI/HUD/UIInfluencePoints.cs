using System;
using TMPro;
using UI.Test;
using UnityEngine;

namespace UI.HUD
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIInfluencePoints : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void OnUpdateIP(InfluencePointsData data)
        {
            _text.text = data.InfluencePoints.ToString();
        }
    }
}
