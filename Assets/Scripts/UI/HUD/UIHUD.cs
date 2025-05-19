using UnityEngine;

namespace UI.HUD
{
    public class HUD : MonoBehaviour
    {
        private int _testIP;
        
        public void OnGetIP()
        {
            _testIP += 50;
            GameEvents.InfluencePoints.OnChangeInfluencePoints.Invoke(_testIP);
        }
    }
}
