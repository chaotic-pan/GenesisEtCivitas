using Models.Interfaces;
using UnityEngine.Events;

namespace Models
{
    public class NPCModel : IObservableData<NPCModel>
    {
        public UnityEvent<NPCModel> OnUpdateData { get; } = new();
        
        private float _faith;

        public float Faith
        {
            get => _faith;
            set
            {
                _faith = value;
                OnUpdateData.Invoke(this);
            }
        }

        public string NPCName = "Pedro";
    }
}