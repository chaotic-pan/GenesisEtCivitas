using Models.Interfaces;
using UnityEngine.Events;

namespace Models
{
    public class CityModel : IObservableData<CityModel>
    {
        public UnityEvent<CityModel> OnUpdateData { get; } = new();

        private int _population;
        private string _cityName;

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
    }
}
