using UnityEngine;
using UnityEngine.Events;

namespace UI.Test
{
    public abstract class UpdateableMenu<TData> : MonoBehaviour where TData : IObservableData<TData>
    {
        private UnityEvent<TData> _tDataEvent;

        private void RegisterData(UnityEvent<TData> dataEvent)
        {
            _tDataEvent?.RemoveListener(UpdateData);
            dataEvent.AddListener(UpdateData);
        }

        protected void OnOpen(TData data)
        {
            RegisterData(data.OnUpdateData);
            _tDataEvent = data.OnUpdateData;
            
            UpdateData(data);
            
            gameObject.SetActive(true);
        }

        public void OnClose()
        {
            _tDataEvent?.RemoveListener(UpdateData);
            gameObject.SetActive(false);
        }

        protected abstract void UpdateData(TData data);
        public abstract void Initialize();
    }
}