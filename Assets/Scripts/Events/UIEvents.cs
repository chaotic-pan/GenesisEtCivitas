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
        public static readonly UIVariable UIVar = new ();

        public class UIMapEvents
        {
            public UnityAction<MapDisplay.MapOverlay> OnOpenHeatmap;
            public UnityAction<List<Vector2>, MapDisplay.MapOverlay> OnUpdateHeatmapChunks;
            public UnityAction<List<Vector2>, MapDisplay.MapOverlay[]> OnUpdateMultipleHeatmapChunks;
            public UnityAction<MapDisplay.MapOverlay> OnUpdateAllHeatmapsOfType;
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
            public UnityAction<Skill> OnOpenSkillItem;
            public UnityAction OnMouseEnterUI;
            public UnityAction OnMouseExitUI;
            public UnityAction OnBuySkill;
            public UnityAction OnUseSkill;
        }
        
        public class UIVariable
        {
            public bool saviourExists = false;
            public bool isCastingSaviourAction = false;
        }
    }
}