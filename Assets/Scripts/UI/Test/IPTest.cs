using UnityEngine;

namespace UI.Test
{
    public class IPTest : MonoBehaviour
    {
        public void OnGainIP()
        {
            GameEvents.InfluencePoints.GainInfluencePoints(50);
        }
    }
}