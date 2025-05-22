using UnityEngine.Events;

namespace UI.Test
{
    public interface IObservableData<T>
    {
        UnityEvent<T> OnUpdateData { get; }
    }
}