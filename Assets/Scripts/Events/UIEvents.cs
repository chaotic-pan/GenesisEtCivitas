using System.Collections.Generic;
using Models;
using Player.Skills;
using Terrain;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public static class UIEvents
    {
        public static readonly UIUpdateEvents UIUpdate = new ();
        public static readonly UIOpenEvents UIOpen = new ();
        public static readonly UIMapEvents UIMap = new ();

        public class UIMapEvents
        {
            public UnityAction<MapDisplay.MapOverlay> OnOpenHeatmap;
            public UnityAction<List<Vector2>, MapDisplay.MapOverlay> OnUpdateHeatmapChunks;
        }
        
        public class UIUpdateEvents
        {
            public UnityAction<PlayerModel> OnUpdatePlayerData;
        }

        public class UIOpenEvents
        {
            public UnityAction<CityModel> OnOpenCityMenu;
            public UnityAction<NPCModel> OnOpenNpcMenu;
            public UnityAction<NPCModel> OnOpenMessiahMenu;
            public UnityAction<Civilization> OnSelectCityMessiahAction;
            public UnityAction<PlayerSkillSet> OnOpenSkillTree;
        }
    }
}