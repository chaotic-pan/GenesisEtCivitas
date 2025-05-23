using Models;
using UnityEngine.Events;

namespace UI
{
    public static class UIEvents
    {
        public static readonly UIUpdateEvents UIUpdate = new ();
        public static readonly UIOpenEvents UIOpen = new ();
    
        public class UIUpdateEvents
        {
            public UnityAction<PlayerModel> OnUpdatePlayerData;
        }

        public class UIOpenEvents
        {
            public UnityAction<CityModel> OnOpenCityMenu;
        }
    }
}