using UnityEngine;
using TMPro;
using Player;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        private PlayerModel _playerModel;
        [SerializeField] private TMP_Text _IpText;
        [SerializeField] private GameObject _SkillTreeMenu;
        private void Start()
        {
            var player = GameObject.Find("Player");
              _playerModel = player.GetComponent<PlayerModel>();
        }

        private void Update()
        {
            _IpText.text = _playerModel.influencePoints.ToString();
        }

        public void LoadSkillTree()
        {
            _SkillTreeMenu.SetActive(!_SkillTreeMenu.activeSelf);
        }


    }
}