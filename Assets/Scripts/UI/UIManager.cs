using Models;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UICityMenu uiCityMenu;
        [SerializeField] private UIHUD uiHUD;

        void Awake()
        {
            uiCityMenu.Initialize();
            uiHUD.Initialize();
        }
    }
}