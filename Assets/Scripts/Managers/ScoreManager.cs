using System.Collections;
using UnityEngine;
using Player;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        private PlayerModel _playerModel;
        
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

                if (_playerModel.influencePoints < _playerModel.maxIP)
                {
                    _playerModel.influencePoints += 10;
                }
            }
        }
    }
}