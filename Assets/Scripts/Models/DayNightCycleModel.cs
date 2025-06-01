using Models.Interfaces;
using UnityEngine.Events;

namespace Models
{
    public class DayNightCycleModel : IObservableData<DayNightCycleModel>
    {
        public UnityEvent<DayNightCycleModel> OnUpdateData { get; } = new();

        private float _currentInGameTimeInSeconds;
        private bool _advancedCycle;
        
        public  float ElapsedRealTimeInSeconds;

        public float CurrentInGameTimeInSeconds
        {
            get => _currentInGameTimeInSeconds;
            set
            {
                _currentInGameTimeInSeconds = value;
                OnUpdateData.Invoke(this);
            }
        }
        
        public bool AdvanceCycle
        {
            get => _advancedCycle;
            set
            {
                _advancedCycle = value;
                OnUpdateData.Invoke(this);
            }
        }
    }
}