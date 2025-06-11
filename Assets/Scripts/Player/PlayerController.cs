using Models;
using Player.Abilities;
using Player.Skills;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel _playerModel;
        private PlayerSkillSet _playerSkillSet;
        private PlayerAbility _activeAbility;
        
        private bool _isWaitingForTileClick;

        private void Start()
        {
            _playerModel = GetComponent<PlayerModel>();
            _playerSkillSet = new PlayerSkillSet();
        }
        
        private void Update()
        {
            if (_isWaitingForTileClick)
            {
                ShowHoverEffect();

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
            var tileGridPos = TileManager.Instance.GetTileGridPositionByWorldCoords(point);

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
                var tileGridPos = TileManager.Instance.GetTileGridPositionByWorldCoords(point);
                var tileData = TileManager.Instance.getTileDataByGridCoords(tileGridPos);
                if (!tileData)
                {
                    Debug.Log("No Tile Data!");
                    return;
                }
                Debug.Log("Tile grid pos: " + tileGridPos);
                Debug.Log("Tile water value: " + tileData.waterValue);
                
                CastAbility(tileGridPos);
                Debug.Log("New tile water value: " + tileData.waterValue);
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

        public bool CanUseWaterOne()
        {
            return _playerSkillSet.IsSkillUnlocked(PlayerSkillSet.Skill.WaterOne);
        }
    }
}