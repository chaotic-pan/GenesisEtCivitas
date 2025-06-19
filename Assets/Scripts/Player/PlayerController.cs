using Models;
using Player.Abilities;
using Player.Skills;
using Terrain;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel _playerModel;
        private PlayerSkillSet _playerSkillSet;
        private PlayerAbility _activeAbility;
        
        private bool _isWaitingForTileClick;

        private void Awake()
        {
            _playerModel = GetComponent<PlayerModel>();
            _playerSkillSet = new PlayerSkillSet(_playerModel);
            _playerSkillSet.OnSkillUnlocked += PlayerSkillSet_OnSkillUnlocked;
        }
        
        private void Update()
        {
            if (_isWaitingForTileClick)
            {
                // ShowHoverEffect();

                if (Input.GetMouseButtonDown(0))
                {
                    HandleTileClick();
                }
            }
        }

        private void ShowHoverEffect()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (!plane.Raycast(ray, out var distance)) return;
            
            var point = ray.GetPoint(distance);
            var tileGridPos = TileManager.Instance.map.WorldToCell(point);

            if (TileManager.Instance.getTileDataByGridCoords(tileGridPos) != null)
            {
                VisualizeAreaOfEffect(tileGridPos);
            }
        }

        private void VisualizeAreaOfEffect(Vector3Int tileGridPos)
        {
            throw new System.NotImplementedException();
        }

        private void HandleTileClick()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out var distance))
            {
                var point = ray.GetPoint(distance);
                var tileGridPos = TileManager.Instance.map.WorldToCell(point);
                var tileData = TileManager.Instance.getTileDataByGridCoords(tileGridPos);
                if (tileData == null)
                {
                    Debug.Log("No Tile Data!");
                    return;
                }
                Debug.Log("Tile grid pos: " + tileGridPos);
                Debug.Log("Tile water value: " + tileData.waterValue);
                
                CastAbility(tileGridPos);
                Debug.Log("New tile water value: " + tileData.waterValue);
                
                // testheatmap update
               UIEvents.UIMap.OnUpdateHeatmapChunks.Invoke(TileManager.Instance.getWorldPositionOfTile(tileGridPos), MapDisplay.MapOverlay.WaterValue);
            }

            _isWaitingForTileClick = false;
        }

        public void EnterRainAbility()
        {
            EnterAbility(AbilityType.Rain);
        }

        public void EnterEarthquakeAbility()
        {
            EnterAbility(AbilityType.Earthquake);
        }

        private void EnterAbility(AbilityType type)
        {
            _activeAbility  = type switch
            {
                AbilityType.Rain => gameObject.AddComponent<RainAbility>(),
                AbilityType.Earthquake => gameObject.AddComponent<EarthquakeAbility>(),
                _ => null
            };
            if (!_activeAbility) return;
            
            _activeAbility.EnterAbility();
            _isWaitingForTileClick = true;
        }

        private void CastAbility(Vector3Int tileGridPos)
        {
            if (_playerModel.InfluencePoints < _activeAbility.Cost)
            {
                // Debug.Log("Too little CASH");
                return;
            }
            _playerModel.InfluencePoints -= _activeAbility.Cost;
            _activeAbility.CastAbility(tileGridPos);
            // Debug.Log("Cost: " + _activeAbility.Cost);
            
            _activeAbility = null;
        }

        //Skills

        private void PlayerSkillSet_OnSkillUnlocked(object sender, PlayerSkillSet.OnSkillUnlockedEventArgs e)
        {
            switch (e.skill)
            {
                case PlayerSkillSet.Skill.WaterOne:
                    SetWaterOne();
                    break;
                case PlayerSkillSet.Skill.DeathOne:
                    SetDeathOne();
                    break;
                case PlayerSkillSet.Skill.WaterTwo:
                    SetWaterTwo();
                    break;
                case PlayerSkillSet.Skill.DeathTwo:
                    SetDeathTwo();
                    break;
            }
        }

        public PlayerSkillSet GetPlayerSkillSet()
        {
            return _playerSkillSet;
        }

        // Water Skills
        public bool CanUseWaterOne()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.WaterOne);
        }
        private void SetWaterOne()
        {
            Debug.Log("Water One Unlocked");
        }

        public bool CanUseWaterTwo()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.WaterTwo);
        }
        private void SetWaterTwo()
        {
            Debug.Log("Water Two Unlocked");
        }

        // Death Skill set
        public bool CanUseDeathOne()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.DeathTwo);
        }
        private void SetDeathOne()
        {
            Debug.Log("Death One Unlocked");
        }
        
         public bool CanUseDeathTwo()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.DeathTwo);
        }
        private void SetDeathTwo()
        {
            Debug.Log("Death Two Unlocked");
        }
    }
}