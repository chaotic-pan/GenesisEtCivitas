using System;
using System.Collections.Generic;
using Terrain;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class UIHeatmaps : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
        }

        private void Start()
        {
            foreach (var overlay in Enum.GetValues(typeof(MapDisplay.MapOverlay)))
            {
                _dropdown.AddOptions(new List<string>() { overlay.ToString() });
            }
        }
    
        public void OnSelectedMap(int choice)
        {
            var overlay = (MapDisplay.MapOverlay) choice;
            UIEvents.UIMap.OnOpenHeatmap.Invoke(overlay);
        }
    }
}
