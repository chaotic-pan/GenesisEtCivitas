using UnityEngine.Events;

namespace Models.Interfaces
{
    public interface IObservableData<T>
    {
        UnityEvent<T> OnUpdateData { get; }
    }
}