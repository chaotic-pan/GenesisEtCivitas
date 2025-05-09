using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private PlayerModel _playerModel;
    [SerializeField] private TMP_Text _IpText;
    void Start()
    {
        GameObject playerController = GameObject.Find("PlayerController");
          _playerModel = playerController.GetComponent<PlayerModel>();
    }

    void Update()
    {
        _IpText.text = _playerModel.influencePoints.ToString();
    }


}
