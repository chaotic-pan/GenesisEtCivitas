using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private PlayerModel _playerModel;


    void Start()
    {

        GameObject playerController = GameObject.Find("PlayerController");
        _playerModel = playerController.GetComponent<PlayerModel>();

    }

    void Update()
    {
        IncreaseIP();
    }

    void IncreaseIP()
    {
        if(_playerModel.influencePoints < _playerModel.maxIP)
        {
            _playerModel.influencePoints++;
        }
    }
}
