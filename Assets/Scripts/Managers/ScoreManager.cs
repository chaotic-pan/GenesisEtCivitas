using System;
using System.Collections;
using Events;
using Models;
using UnityEngine;
using Player;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private NPCSpawner _npcSpawner;
        private PlayerModel _playerModel;

        private void Awake()
        {
            GameEvents.InfluencePoints.GainInfluencePoints += OnGainInfluencePoints;
        }

        private void Start()
        {

            var player = GameObject.Find("Player");
            _playerModel = player.GetComponent<PlayerModel>();

            StartCoroutine(IncreaseIP());
        }

        private IEnumerator IncreaseIP()
        {
            while (true)
            {
                yield return new WaitForSeconds(5f);

                if (_playerModel.InfluencePoints < _playerModel.MaxIP)
                {
                    foreach (GameObject civ in _npcSpawner.civilisations)
                    {
                        //_playerModel.InfluencePoints += 10;
                        
                        if (civ == null) continue;
                        civ.GetComponent<Civilization>().CalcValues();
                        _playerModel.InfluencePoints += (int)civ.GetComponent<Civilization>().Belief;
                    }
                    
                }
            }
        }
        
        private void OnGainInfluencePoints(int points) => _playerModel.InfluencePoints += points;
    }
}