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
                yield return new WaitForSeconds(1f);

                if (_playerModel.InfluencePoints < _playerModel.MaxIP)
                {
                    _playerModel.InfluencePoints += 10;
                }
            }
        }
        
        private void OnGainInfluencePoints(int points) => _playerModel.InfluencePoints += points;
    }
}