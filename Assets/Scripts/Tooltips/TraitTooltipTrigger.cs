using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TraitTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TraitData traitData;
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        TooltipSystem.Instance.ShowTrait(traitData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }
}
