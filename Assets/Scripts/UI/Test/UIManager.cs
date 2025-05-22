using UnityEngine;

namespace UI.Test
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CityMenu cityMenu;

        void Awake()
        {
            cityMenu.Initialize();
        }
    }
}