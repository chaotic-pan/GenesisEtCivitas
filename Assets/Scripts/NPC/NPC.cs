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
        int layerMask = ~(1 << LayerMask.NameToLayer("Ability"));
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                UIEvents.UIOpen.OnOpenNpcMenu.Invoke(_npcModel);
            }
        }

    }
}
