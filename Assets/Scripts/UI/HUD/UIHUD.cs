using System;
using UI.Test;
using UnityEngine;

namespace UI.HUD
{
    public class HUD : MonoBehaviour
    {
        public void OnGetInfluencePoints()
        {
            GameEvents.InfluencePoints.OnGetInfluencePoints.Invoke(50);
        }
    }
}
