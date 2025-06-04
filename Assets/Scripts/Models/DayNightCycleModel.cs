using Events;
using Models.Interfaces;
using UnityEngine.Events;

namespace Models
{
    public class DayNightCycleModel
    {
        public int CurrentInGameDay;
        public int CurrentInGameHour;
        public int CurrentInGameMinute;
        
        private int _maxDays;
        private bool _advancedCycle;

        private double _elapsedInGameMinutes;
        
        public double ElapsedInGameMinutes
        {
            get => _elapsedInGameMinutes;
            set
            {
                _elapsedInGameMinutes = value;
                GameEvents.DayNightCycle.OnDayNightCycleUpdate.Invoke(this);
            }
        }
        
        
        public bool AdvanceCycle
        {
            get => _advancedCycle;
            set
            {
                if (_advancedCycle == value) return;
                
                _advancedCycle = value;
                GameEvents.DayNightCycle.OnDayNightCycleUpdate.Invoke(this);
            }
        }
    }
}