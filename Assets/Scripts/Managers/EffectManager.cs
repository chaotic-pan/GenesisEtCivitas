using UnityEngine;
using System.Collections.Generic;

namespace Managers
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager instance;

        [Header("Effect Prefabs")] 
        public GameObject rainEffectPrefab;
        public GameObject earthquakeEffectPrefab;
        public GameObject plantGrowthEffectPrefab;

        private Dictionary<string, Queue<GameObject>> pools = new();

        private void Awake()
        {
            instance = this;
            InitializePool("Rain", rainEffectPrefab, 50);
            InitializePool("Earthquake", earthquakeEffectPrefab, 50);
            InitializePool("PlantGrowth", plantGrowthEffectPrefab, 50);
        }

        private void InitializePool(string effectName, GameObject prefab, int count)
        {
            GameObject go = new GameObject(effectName+" Particles");
            if (pools.ContainsKey(effectName)) return;
            pools[effectName] = new Queue<GameObject>();

            for (var i = 0; i < count; i++)
            {
                var effect = Instantiate(prefab, go.transform, true);
                effect.SetActive(false);
                pools[effectName].Enqueue(effect);
            }
        }

        public GameObject GetEffect(string effectName)
        {
            if (!pools.ContainsKey(effectName) || pools[effectName].Count <= 0) return null;
            var effect = pools[effectName].Dequeue();
            effect.SetActive(true);
            return effect;

        }

        public void ReturnEffect(string effectName, GameObject effect)
        {
            effect.SetActive(false);
            if (pools.ContainsKey(effectName))
            {
                pools[effectName].Enqueue(effect);
            }
        }
    }
}