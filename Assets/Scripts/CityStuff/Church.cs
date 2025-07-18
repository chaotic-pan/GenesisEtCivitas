using Events;
using Models;
using Unity.VisualScripting;
using UnityEngine;

namespace CityStuff
{
    public class Church : CityBuilding
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
        
        protected override float getHeight()
        {
            return 9.6f;
        }
    }
}
