using Models;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPC : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private NPCInfluenceArea influenceArea;
    
    private NPCModel _npcModel = new NPCModel();
    
    public void Awake()
    {
        influenceArea.Initialize(this);
    }

    public void IncreaseInfluence(int influence)
    {
        _npcModel.Faith += influence;   
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIEvents.UIOpen.OnOpenNpcMenu.Invoke(_npcModel);
        }
    }
}
