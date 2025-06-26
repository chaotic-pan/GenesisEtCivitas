using Models;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public static class GameEvents
    {
        public static readonly InfluencePointsEvents InfluencePoints = new ();
        public static readonly DayNightCycleEvents DayNightCycle = new ();
        public static readonly LifecycleEvents Lifecycle = new ();
        public static readonly CameraEvents Camera = new ();
        public static readonly CivilizationEvents Civilization = new ();

        public class CameraEvents
        {
            public UnityAction<GameObject> OnJumpToCiv;
        }

        public class CivilizationEvents
        {
            public UnityAction<NPCModel> OnCivilizationSpawn;
        }
        
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