using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

class NPCIdeling : MonoBehaviour
{
    public GameObject civiPrefab;

    private Civilization civ;
    private List<GameObject> idles = new();

    private void OnEnable()
    { 
        civ = GetComponent<Civilization>(); 
        GameEvents.Civilization.OnPreach += SaviourAudience; 
        GameEvents.Civilization.OnPreachEnd += YeetSaviourAudience;
    }

    private void OnDisable()
    { 
        GameEvents.Civilization.OnPreach -= SaviourAudience;
        GameEvents.Civilization.OnPreachEnd -= YeetSaviourAudience;
    }
    
    private void SaviourAudience(GameObject saviour)
    {
        var saviourPos = saviour.transform.position;
        if (transform.position != saviourPos) return;

        int audience = 0;
        float distance = 2;
        int count = 0;
        for (var j = 0; j < 3; j++)
        {
            distance += 5;
            count += 4;
            for (var i=0; i<count; i++) 
            {
                if (audience >= civ.population) return;

                audience++;
                var angle = Math.PI*2/count * i + Random.Range(-0.2f, 0.2f);
                float x = (float)(saviourPos.x + (distance+Random.Range(0f, 3f)) * Math.Cos(angle)) ;
                float z = (float)(saviourPos.z + (distance+Random.Range(0f, 3f)) * Math.Sin(angle));
                var civi= Instantiate(civiPrefab, new Vector3(x, saviourPos.y, z), Quaternion.LookRotation(saviourPos), transform);
                civi.transform.LookAt(saviour.transform);
                idles.Add(civi);
            }
        }
    }
    
    private void YeetSaviourAudience(GameObject saviour)
    {
        var saviourPos = saviour.transform.position;
        if (transform.position != saviourPos) return;

        foreach (var civi in idles)
        {
            Destroy(civi);
        }
        idles.Clear();
    }
}