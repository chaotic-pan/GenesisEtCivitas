using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UIHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI influencePointsText;
        
        public void Initialize()
        {
            UIEvents.UIUpdate.OnUpdatePlayerData += OnUpdatePlayerData;
        }

        private void OnUpdatePlayerData(PlayerModel playerData)
        {
            influencePointsText.text = playerData.InfluencePoints.ToString();
        }
    }
}
