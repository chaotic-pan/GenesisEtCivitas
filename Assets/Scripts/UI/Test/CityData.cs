using UnityEngine.Events;

namespace UI.Test
{
    public class CityData : IObservableData<CityData>
    {
        public UnityEvent<CityData> OnUpdateData { get; } = new();

        private int _population;
        private string _cityName;
        private int _otherInfo;

        public int Population
        {
            get => _population;
            set
            {
                _population = value; 
                OnUpdateData?.Invoke(this);
            }
        }
        
        public string CityName 
        {
            get => _cityName;
            set
            {
                _cityName = value; 
                OnUpdateData?.Invoke(this);
            }
        }
        
        public int OtherInfo {
            get => _otherInfo;
            set
            {
                _otherInfo = value; 
                OnUpdateData?.Invoke(this);
            }
        }

        
    }
}
