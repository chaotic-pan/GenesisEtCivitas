using Events;
using Models;
using UnityEngine;

namespace CityStuff
{
    public class Church : MonoBehaviour
    {
        [SerializeField] private int influencePoints;
        [SerializeField] private int intervalInInGameMinutes;

        private double _nextMinuteToHandle;
        
        public void Initialize()
        {
            GameEvents.DayNightCycle.OnDayNightCycleUpdate += OnDayNightCycleUpdate;
        }

        private void OnDayNightCycleUpdate(DayNightCycleModel cycle)
        {
            if (cycle.ElapsedInGameMinutes < _nextMinuteToHandle) return;
            
            GameEvents.InfluencePoints.GainInfluencePoints(influencePoints);
            
            _nextMinuteToHandle = cycle.ElapsedInGameMinutes + intervalInInGameMinutes;
        }
    }
}
