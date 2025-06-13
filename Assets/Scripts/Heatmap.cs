using Terrain;
using UnityEngine;

[CreateAssetMenu(fileName = "Heatmap", menuName = "ScriptableObjects/Heatmap")]
public class Heatmap : ScriptableObject
{
    public MapDisplay.MapOverlay overlay;
    public Gradient gradient;
    public int min;
    public int max;
}