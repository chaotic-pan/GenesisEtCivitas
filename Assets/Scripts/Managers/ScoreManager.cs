using System;
using System.Collections;
using Events;
using Models;
using UnityEngine;
using Player;
using TMPro;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private NPCSpawner _npcSpawner;
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        private PlayerModel _playerModel;

        private void Awake()
        {
            GameEvents.Lifecycle.OnGameEnd += CalculateFinalScore;
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

        private void CalculateFinalScore()
        {
            var finalScore = 0f;

            // Sum belief from all active civilizations.
            foreach (GameObject civObj in _npcSpawner.civilisations)
            {
                if (civObj == null) continue;
        
                Civilization civ = civObj.GetComponent<Civilization>();
                if (civ != null)
                {
                    finalScore += civ.Belief;
                }
            }
            _finalScoreText.SetText("Faith Score: " + finalScore.ToString("0"));
        }
        
        private void OnGainInfluencePoints(int points) => _playerModel.InfluencePoints += points;
    }
}