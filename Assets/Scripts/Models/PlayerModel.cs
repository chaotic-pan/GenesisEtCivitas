using UI;
using UnityEngine;

namespace Models
{
    public class PlayerModel : MonoBehaviour
    {
        private int _influencePoints = 100;
        public int virtuePoints = 2;
        private int _maxIP = 9999;

        public int InfluencePoints
        {
            get => _influencePoints;
            set
            {
                _influencePoints = value; 
                UIEvents.UIUpdate.OnUpdatePlayerData?.Invoke(this);
            } 
        }
        
        public int MaxIP
        {
            get => _maxIP;
            set
            {
                _maxIP = value; 
                UIEvents.UIUpdate.OnUpdatePlayerData?.Invoke(this);
            } 
        }
    }
}