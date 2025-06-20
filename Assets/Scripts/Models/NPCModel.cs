using Models.Interfaces;
using UnityEngine.Events;

namespace Models
{
    public class NPCModel : IObservableData<NPCModel>
    {
        public UnityEvent<NPCModel> OnUpdateData { get; } = new();
        
        private float _faith;
        private float _food;
        private float _water;
        private float _safety;
        private float _shelter;
        private float _energy;

        public float Faith
        {
            get => _faith;
            set
            {
                _faith = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Food
        {
            get => _food;
            set
            {
                _food = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Water
        {
            get => _water;
            set
            {
                _water = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Safety
        {
            get => _safety;
            set
            {
                _safety = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Shelter
        {
            get => _shelter;
            set
            {
                _shelter = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Energy
        {
            get => _energy;
            set
            {
                _energy = value;
                OnUpdateData.Invoke(this);
            }
        }

        public string NPCName = "Pedro";
    }
}