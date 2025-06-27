using System.Collections.Generic;
using Models;
using Player.Abilities;
using Player.Skills;
using Terrain;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel _playerModel;
        private PlayerSkillSet _playerSkillSet;
        private PlayerAbility _activeAbility;
        
        public UnityAction<AbilityType> callAbility;
        
        private bool _isWaitingForTileClick;
        
        private List<Vector3Int> aoePreviewTiles = new List<Vector3Int>();
        
        private void Awake()
        {
            _playerModel = GetComponent<PlayerModel>();
            _playerSkillSet = new PlayerSkillSet(_playerModel);
            _playerSkillSet.OnSkillUnlocked += PlayerSkillSet_OnSkillUnlocked;
            callAbility += EnterAbility;
            
        }
        
        private void Update()
        {
            if (_isWaitingForTileClick)
            {
                PreviewAoeTiles();
        
                if (Input.GetMouseButtonDown(0)) // Left click.
                {
                    HandleTileClick();
                }
                else if (Input.GetMouseButtonDown(1)) // Right click.
                {
                    CancelAbility();
                }
            }
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
                CastAbility(tileGridPos);
                
                // testheatmap update
               UIEvents.UIMap.OnUpdateHeatmapChunks.Invoke(TileManager.Instance.getWorldPositionOfTile(tileGridPos), MapDisplay.MapOverlay.WaterValue);
            }

            _isWaitingForTileClick = false;
        }
        
        private void PreviewAoeTiles()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out var distance))
            {
                var point = ray.GetPoint(distance);
                var tileGridPos = TileManager.Instance.map.WorldToCell(point);
        
                int diameter = _activeAbility.EffectDiameter;
                int radius = (diameter - 1) / 2; // For hex grids.
        
                List<Vector3Int> newAoeTiles = TileManager.Instance.GetSpecificRange(tileGridPos, radius);
        
                if (!newAoeTiles.SequenceEqual(aoePreviewTiles))
                {
                    aoePreviewTiles = newAoeTiles;
                    GridOverlayManager.Instance.ShowAoeOverlay(aoePreviewTiles);
                }
            }
        }

        public void EnterRainAbility()
        {
            EnterAbility(AbilityType.Rain);
        }
        
        private void CancelAbility()
        {
            _isWaitingForTileClick = false;
            Destroy(_activeAbility);
            _activeAbility = null;
            GridOverlayManager.Instance.HideAoeOverlay();
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
                AbilityType.SendSaviour => gameObject.AddComponent<SendSaviourAbility>(),
                _ => null
            };
            if (!_activeAbility) return;
            
            _activeAbility.EnterAbility();
            _isWaitingForTileClick = true;
            GridOverlayManager.Instance.ShowAoeOverlay(new List<Vector3Int>());
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
            GridOverlayManager.Instance.HideAoeOverlay();        
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