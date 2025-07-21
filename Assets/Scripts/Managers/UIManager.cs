using System;
using Models;
using UnityEngine;
using TMPro;
using Player;
using UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        private PlayerModel _playerModel;
        [SerializeField] private TMP_Text _IpText;
        [SerializeField] private GameObject _SkillTreeMenu;
        
        [SerializeField] private UICityMenu uiCityMenu;
        [SerializeField] private UINpcMenu uiNpcMenu;
        [SerializeField] private UIMessiahMenu uiMessiahMenu;


        private void Awake()
        {
            uiNpcMenu?.Initialize();
            uiMessiahMenu?.Initialize();
        }

        private void Start()
        {
            var player = GameObject.Find("Player");
              _playerModel = player.GetComponent<PlayerModel>();
        }

        private void Update()
        {
            _IpText.text = _playerModel.InfluencePoints.ToString();
            
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene("MainMenu");
        }
    }
}