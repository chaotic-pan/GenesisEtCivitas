using Events;
using Models.Interfaces;
using UnityEngine.Events;

namespace Models
{
    public class DayNightCycleModel
    {
        private int _currentInGameDay;
        private int _currentInGameHour;
        private int _currentInGameMinute;
        
        private int _maxDays;
        private bool _advancedCycle;
        

        public int CurrentInGameDay
        {
            get => _currentInGameDay;
            set
            {
                _currentInGameDay = value;
                GameEvents.DayNightCycle.OnDayNightCycleUpdate.Invoke(this);
            }
        }
        
        public int CurrentInGameHour
        {
            get => _currentInGameHour;
            set
            {
                _currentInGameHour = value;
                GameEvents.DayNightCycle.OnDayNightCycleUpdate.Invoke(this);
            }
        }
        
        public int CurrentInGameMinute
        {
            get => _currentInGameMinute;
            set
            {
                _currentInGameMinute = value;
                GameEvents.DayNightCycle.OnDayNightCycleUpdate.Invoke(this);
            }
        }
        
        public bool AdvanceCycle
        {
            get => _advancedCycle;
            set
            {
                _advancedCycle = value;
                GameEvents.DayNightCycle.OnDayNightCycleUpdate.Invoke(this);
            }
        }
    }
}