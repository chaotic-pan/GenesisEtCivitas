using Models;
using UnityEngine.Events;

namespace Events
{
    public static class GameEvents
    {
        public static readonly InfluencePointsEvents InfluencePoints = new ();
        public static readonly DayNightCycleEvents DayNightCycle = new ();
        public static readonly LifecycleEvents Lifecycle = new ();
        
        public class LifecycleEvents
        {
            public UnityAction OnGameEnd;
        }
        
        public class InfluencePointsEvents
        {
            public UnityAction<int> GainInfluencePoints;
        }

        public class DayNightCycleEvents
        {
            public UnityAction<DayNightCycleModel> OnDayNightCycleUpdate;
        }
    }
}