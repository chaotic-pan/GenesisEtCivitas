using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace DefaultNamespace
{
    public class Language
    {
        private int _syllableCount;
        
        private int _onsetCount;
        private int _vowelCount;
        private int _codaCount;

        private string[] _possibleOnsets;
        private string[] _possibleCodas;
        private string[] _possibleVowels;
        
        
        public Language()
        {
            _onsetCount = CalculateOnsetCount();
            _codaCount = CalculateCodaCount();
            _vowelCount = CalculateVowelCount();
            
            _syllableCount = CalculateSyllableCount();

            _possibleOnsets = GetPossibleOnsets();
            _possibleCodas = GetPossibleCodas();
            _possibleVowels = GetPossibleVowels();
        }

        public string GenerateWord()
        {
            var rnd = new Random();
            var word = "";
            
            for (var i = 0; i < _syllableCount; i++)
            {
                var onsetIndex = rnd.Next(0, _possibleOnsets.Length);
                var vowelIndex = rnd.Next(0, _possibleVowels.Length);
                var codaIndex = rnd.Next(0, _possibleCodas.Length);

                if (_onsetCount != 0)
                    word += _possibleOnsets[onsetIndex];

                word += _possibleVowels[vowelIndex];
                
                if (_codaCount != 0)
                    word += _possibleCodas[codaIndex];
            }

      
            return $"{word[0].ToString().ToUpper()}{word[1..]}";
        }
        
        private string[] GetPossibleVowels()
        {
            if (_vowelCount == 0)
                return Array.Empty<string>();
            
            var rnd = new Random();
            var value = rnd.Next(3, 6);
            var vowels = new List<string>();


            for (var i = 0; i < value; i++)
            {
                var specialCharacter = rnd.Next(1, 101);
                var dipththong = 100;

                if (_vowelCount == 2) 
                    dipththong = rnd.Next(1, 101);
                
                var vowel = "";
                
                if (specialCharacter < 10)
                {
                    var randomIndex = rnd.Next(0, LanguageConfig.V1Special.Length);
                    vowel += LanguageConfig.V1Special[randomIndex];
                }
                else
                {
                    var randomIndex = rnd.Next(0, LanguageConfig.V1Base.Length);
                    vowel += LanguageConfig.V1Base[randomIndex];
                }

                if (dipththong < 50)
                {
                    var randomIndex = rnd.Next(0, LanguageConfig.V1Base.Length);
                    vowel += LanguageConfig.V1Base[randomIndex];
                }
                
                vowels.Add(vowel);
            }

            return vowels.ToArray();
        }
        
        private string[] GetPossibleCodas()
        {
            if (_codaCount == 0)
                return Array.Empty<string>();
            
            var rnd = new Random();
            var value = rnd.Next(5, 10);
            var codas = new string[value];
            ;

            if (_codaCount == 1)
            {
                for (var i = 0; i < value; i++)
                {
                    var randomIndex = rnd.Next(0, LanguageConfig.C1.Length);
                    codas[i] = LanguageConfig.C1[randomIndex];
                }
            } else
            {
                for (var i = 0; i < value; i++)
                {
                    var hasTwoConsonants = rnd.Next(1, 101);

                    if (hasTwoConsonants < 50)
                    {
                        var randomIndex = rnd.Next(0, LanguageConfig.C2Coda.Length);
                        codas[i] = LanguageConfig.C2Coda[randomIndex];
                    }
                    else
                    {
                        var randomIndex = rnd.Next(0, LanguageConfig.C1.Length);
                        codas[i] = LanguageConfig.C1[randomIndex];
                    }
                    
                }
            }

            return codas;
        }
        
        private string[] GetPossibleOnsets()
        {
            if (_onsetCount == 0)
                return Array.Empty<string>();
            
            var rnd = new Random();
            var value = rnd.Next(5, 10);
            var onsets = new string[value];
            
            switch (_onsetCount)
            {
                case 1:
                {
                    for (var i = 0; i < value; i++)
                    {
                        var randomIndex = rnd.Next(0, LanguageConfig.C1.Length);
                        onsets[i] = LanguageConfig.C1[randomIndex];
                    }

                    break;
                }
                case 2:
                {
                    for (var i = 0; i < value; i++)
                    {
                        var hasTwoConsonants = rnd.Next(1, 101);

                        if (hasTwoConsonants < 50)
                        {
                            var randomIndex = rnd.Next(0, LanguageConfig.C2Onset.Length);
                            onsets[i] = LanguageConfig.C2Onset[randomIndex];
                        }
                        else
                        {
                            var randomIndex = rnd.Next(0, LanguageConfig.C1.Length);
                            onsets[i] = LanguageConfig.C1[randomIndex];
                        }
                    
                    }

                    break;
                }
                default:
                {
                    for (var i = 0; i < value; i++)
                    {
                        var hasThreeConsonants = rnd.Next(1, 101);

                        if (hasThreeConsonants < 33)
                        {
                            var randomIndex = rnd.Next(0, LanguageConfig.C3Onset.Length);
                            onsets[i] = LanguageConfig.C3Onset[randomIndex];
                        }
                        else if (hasThreeConsonants < 66)
                        {
                            var randomIndex = rnd.Next(0, LanguageConfig.C2Onset.Length);
                            onsets[i] = LanguageConfig.C2Onset[randomIndex];
                        }
                        else
                        {
                            var randomIndex = rnd.Next(0, LanguageConfig.C1.Length);
                            onsets[i] = LanguageConfig.C1[randomIndex];
                        }
                    }

                    break;
                }
            }

            return onsets;
        }
        
        private int CalculateSyllableCount()
        {
            var rnd = new Random();
            var value = rnd.Next(1, 101);

            return value switch
            {
                < 5 => 1,
                < 75 => 2,
                _ => 3,
            };
        }
        
        private int CalculateVowelCount()
        {
            var rnd = new Random();
            var value = rnd.Next(1, 101);

            return value switch
            {
                < 75 => 1,
                _ => 2
            };
        }
        
        private int CalculateCodaCount()
        {
            var rnd = new Random();
            var value = rnd.Next(1, 101);

            return value switch
            {
                < 10 => 0,
                < 85 => 1,
                _ => 2
            };
        }
        
        private int CalculateOnsetCount()
        {
            var rnd = new Random();
            var value = rnd.Next(1, 101);

            return value switch
            {
                < 20 => 0,
                < 80 => 1,
                < 95 => 2,
                _ => 3
            };
        }
    }
}