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
            public UnityAction<GameObject, GameObject> OnMessiahSpawn; // Messiah, Prev Civ
            
            // All GameObjects must have NPC
            public UnityAction<GameObject> OnCivilizationSpawn;
            public UnityAction<GameObject, GameObject> OnCivilizationSplit; // A split from B
            public UnityAction<GameObject, GameObject> OnCivilizationMerge; // A joined B
            public UnityAction<GameObject> OnCivilizationDeath;
            public UnityAction<GameObject> OnCityFounded;
            public UnityAction<GameObject> OnCivilizationLowOnStats;

            public UnityAction<GameObject> OnStartWalking;
            public UnityAction<GameObject> OnStopWalking;
            public UnityAction<GameObject> OnPreach;
            public UnityAction<GameObject> OnPreachEnd;

            public UnityAction<GameObject> OnStatsDecay;
        }
        
        public class LifecycleEvents
        {
            public UnityAction OnGameEnd;
            public UnityAction OnTileManagerFinishedInitializing;
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