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
        public static PlayerController instance;
        public PlayerModel _playerModel;
        public PlayerSkillSet _playerSkillSet;
        public PlayerAbility _activeAbility;
        
        public UnityAction<AbilityType> callAbility;
        
        private bool _isWaitingForTileClick;
        
        private List<Vector3Int> aoePreviewTiles = new List<Vector3Int>();
        
        private void Awake()
        {
            _playerModel = GetComponent<PlayerModel>();
            _playerSkillSet = new PlayerSkillSet(_playerModel);
            callAbility += EnterAbility;
            instance = this;
            
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
            if (_playerModel.InfluencePoints < _activeAbility.Cost)
            {
                Debug.Log("Not enough IP!");
                CancelAbility();
                return;
            }
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out var distance))
            {
                var point = ray.GetPoint(distance);
                var tileGridPos = TileManager.Instance.WorldToCell(point);
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
                var tileGridPos = TileManager.Instance.WorldToCell(point);
        
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
                AbilityType.PlantGrowth => gameObject.AddComponent<PlantGrowthAbility>(),
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
            UIEvents.UIOpen.OnUseSkill?.Invoke();
        }

        public PlayerSkillSet GetPlayerSkillSet()
        {
            return _playerSkillSet;
        }
    }
}