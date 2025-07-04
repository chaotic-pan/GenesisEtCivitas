using System;
using Player.Abilities;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class NPCInfluenceArea : MonoBehaviour
{
    private NPC _npc;

    public void Initialize(NPC npc) => _npc = npc;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ability")) return;
        
        var ability = other.GetComponent<PlayerAbility>();
        
        _npc.IncreaseInfluence(); 
        //_npc.IncreaseInfluence(ability.influence);
    }
}
