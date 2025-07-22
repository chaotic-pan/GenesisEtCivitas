using System;
using MapGeneration;
using Terrain;
using TMPro;
using UI;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class HeatmapKey : MonoBehaviour
{
    [SerializeField] private RawImage panel;
    [SerializeField] private TextMeshProUGUI minText;
    [SerializeField] private TextMeshProUGUI maxText;

    private Vector2Int textureSize = new(256, 1);
    
    private void Awake()
    {
        UIEvents.UIMap.OnOpenHeatmap += OnChangeMap;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UIEvents.UIMap.OnOpenHeatmap -= OnChangeMap;
    }
    
    private void OnChangeMap(MapDisplay.MapOverlay overlay)
    {
        if (overlay == MapDisplay.MapOverlay.Terrain)
            gameObject.SetActive(false);
        
        var heatmap = HeatmapDisplay.Instance._heatmapDict[overlay];

        var tex = new Texture2D(textureSize.x, textureSize.y);

        for (int x = 0; x < textureSize.x; x++)
        {
            var color = heatmap.gradient.Evaluate((float)x / (textureSize.x - 1));
            tex.SetPixel(x, 0, color);
        }
        
        tex.Apply();
        
        panel.texture = tex;
        
        minText.text = heatmap.min.ToString();
        maxText.text = heatmap.max.ToString();
        
        gameObject.SetActive(true);
    }
}
