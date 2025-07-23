using System;
using UnityEngine;
using UnityEngine.Events;

namespace Player.Skills
{
    [CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill", order = 1)]
    public class Skill : ScriptableObject
    {
        public Skill requirement;
        public int cost;
        public UnityAction onUnlocked;

        [Header("UI")] 
        public Sprite icon;
        public Color uiColor;
        public string title;
        [TextArea] public string description;

        public void Unlock()
        {
            onUnlocked?.Invoke();
        }
    }
}