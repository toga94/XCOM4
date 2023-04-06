using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public TraitData traitData;

    public static TooltipSystem Instance { get; private set; }
    [SerializeField] private TooltipUI tooltip;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TooltipSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Show()
    {
        tooltip.gameObject.SetActive(true);
    }
    public void ShowTrait(TraitData traitData)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.SetText(traitData);
    }
    public void Hide()
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}
