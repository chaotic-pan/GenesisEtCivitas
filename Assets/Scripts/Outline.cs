using System;
using UnityEngine;

public class Outline : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    
    private void Start()
    {
        ActivateOutline();
    }

    public void ActivateOutline()
    {
        AddOutlineLayerToRenderer(OutlineLayer.Outline);
    }
    
    public void DeactivateOutline()
    {
        RemoveOutlineLayerFromRenderer(OutlineLayer.Outline);
    }
    
    public void ActivateHoverOutline()
    {
        AddOutlineLayerToRenderer(OutlineLayer.HoverOutline);
    }
    
    public void DeactivateHoverOutline()
    {
        RemoveOutlineLayerFromRenderer(OutlineLayer.HoverOutline);
    }
    
    public void ActivateSelectedOutline()
    {
        AddOutlineLayerToRenderer(OutlineLayer.SelectedOutline);
    }
    
    public void DeactivateSelectedOutline()
    {
        RemoveOutlineLayerFromRenderer(OutlineLayer.SelectedOutline);
    }

    private void AddOutlineLayerToRenderer(OutlineLayer outlineLayer)
    {
        foreach (var rend in renderers)
        {
            rend.renderingLayerMask |= 1U << (int)outlineLayer;
        }
    }
    
    private void RemoveOutlineLayerFromRenderer(OutlineLayer outlineLayer)
    {
        foreach (var rend in renderers)
        {
            if(rend is null) //TODO: this is not a solution, but a fix for testing
                continue;
                
            rend.renderingLayerMask &= ~(1U << (int)outlineLayer);
        }
    }

    private enum OutlineLayer
    {
        Outline = 7,
        HoverOutline = 8,
        SelectedOutline = 9
    }
}
