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
            public UnityAction<GameObject, bool> OnSwim;
            public UnityAction<GameObject> OnStopWalking;
            public UnityAction<GameObject> OnPreach;
            public UnityAction<GameObject> OnPreachEnd;
            public UnityAction<GameObject> OnPray;
            public UnityAction<GameObject> OnListen;
            public UnityAction<GameObject> OnBuild;

            public UnityAction<GameObject, GameObject> CreateBuilding; //civObject, building
            
            public UnityAction<GameObject> OnTileStatsDecay;
        }
        
        public class LifecycleEvents
        {
            public UnityAction OnGameEnd;
            public UnityAction OnGamePause;
            public UnityAction OnTileManagerFinishedInitializing;
            
        }
        
        public class InfluencePointsEvents
        {
            public UnityAction<int> GainInfluencePoints;
            
            public UnityAction<Vector3> PlantsEffect;
        }

        public class DayNightCycleEvents
        {
            public UnityAction<DayNightCycleModel> OnDayNightCycleUpdate;
        }
    }
}