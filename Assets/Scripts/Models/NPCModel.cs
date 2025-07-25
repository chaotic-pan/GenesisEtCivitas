﻿using CityStuff;
using Events;
using Models.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Models
{
    public class NPCModel : IObservableData<NPCModel>
    {
        public UnityEvent<NPCModel> OnUpdateData { get; } = new();
        private string _npcName;
        private int _population;    //von CityModel
        private float _faith;
        private float _food;
        private float _water;
        private float _safety;
        private float _shelter;
        private float _energy;
        private float _cityName;

        private bool _isMessiah;

        public GameObject NPC;
        private City _city;

        private bool _isStruggling;
        
        public City City
        {
            get => _city;
            set
            {
                _city = value;
                OnUpdateData?.Invoke(this);
            }
        }
        
        public string NPCName
        {
            get => _npcName;
            set
            {
                _npcName = value;
                OnUpdateData?.Invoke(this);
            }
        }
        
        public int Population
        {
            get => _population;
            set
            {
                _population = value;
                OnUpdateData?.Invoke(this);
            }
        }

        public bool IsMessiah
        {
            get => _isMessiah;
            set
            {
                _isMessiah = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Faith
        {
            get => _faith;
            set
            {
                _faith = value;
                OnUpdateData.Invoke(this);

                if (_faith < 5)
                {
                    if (_isStruggling) return;
                    
                    GameEvents.Civilization.OnCivilizationLowOnStats.Invoke(NPC);
                    _isStruggling = true;
                }
                else
                {
                    _isStruggling = false;
                }
                
                
            }
        }
        public float Food
        {
            get => _food;
            set
            {
                _food = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Water
        {
            get => _water;
            set
            {
                _water = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Safety
        {
            get => _safety;
            set
            {
                _safety = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Shelter
        {
            get => _shelter;
            set
            {
                _shelter = value;
                OnUpdateData.Invoke(this);
            }
        }
        public float Energy
        {
            get => _energy;
            set
            {
                _energy = value;
                OnUpdateData.Invoke(this);
            }
        }

        public float CityName
        {
            get => _cityName;
            set
            {
                _cityName = value;
                OnUpdateData.Invoke(this);
            }
        }
    }
}