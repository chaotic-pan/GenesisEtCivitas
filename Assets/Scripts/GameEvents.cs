using UI.Test;
using UnityEngine.Events;

public static class GameEvents
{
    public static readonly InfluencePointsEvents InfluencePoints = new ();
    public static readonly UIEvents UI = new ();
    
    public class InfluencePointsEvents
    {
        public UnityAction<int> OnGetInfluencePoints;
    }

    public class UIEvents
    {
        public UnityAction<CityData> OnOpenCityMenu;
    }
}