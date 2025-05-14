using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private PlayerModel _playerModel;
    
    private void Start()
    {

        GameObject playerController = GameObject.Find("PlayerController");
        _playerModel = playerController.GetComponent<PlayerModel>();

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
