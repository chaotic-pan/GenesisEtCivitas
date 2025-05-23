using TMPro;
using UI.Test;
using UnityEngine;

namespace UI.HUD
{
    public class InfluencePointsController : MonoBehaviour
    {
        [SerializeField] private UIInfluencePoints influencePointsInputField;
        
        private readonly InfluencePointsData _influencePointsData = new() { InfluencePoints = 0 };

        private void Awake()
        {
            GameEvents.InfluencePoints.OnGetInfluencePoints += OnGetInfluencePoints;
        }
        
        private void OnGetInfluencePoints(int influencePoints)
        {
            _influencePointsData.InfluencePoints += influencePoints;
            influencePointsInputField.OnUpdateIP(_influencePointsData);
        }
    }
}