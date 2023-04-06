using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerField;

    [SerializeField] private LayoutElement layoutElement;

    [SerializeField] private int characterWrapLimit;

    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    [SerializeField] private TextMeshProUGUI contentField;

    public void SetText(TraitData traitData)
    {
        headerField.text = traitData.name;

        string bunusEffects = "";
        int traitlevel = traitData.traitEffectsLevel.Length;
        int countGrid = GameManager.Instance.GetAllUnitsOnGrid.Count;
        for (int i = traitlevel - 1; i >= 0; i--)
        {
            var traitCount = TraitsUI.Instance.traitCounts;
            int currentTypeCount = 0;
            if (countGrid > 0)
            {
                currentTypeCount = traitCount[traitData.traitType];
            }

            string colorTag = currentTypeCount >= traitData.traitEffectsLevel[i] ? "<color=white>" : "<color=grey>";
            string effectDescription = traitData.traitEffects[i].effectDescription;

            bunusEffects += $"\n {colorTag}{traitData.traitEffectsLevel[i]} units: {effectDescription}</color>";
        }

        contentField.text = $"{traitData.traitDescription} \n {bunusEffects}";
    }
    private int GetTraitMaxStack(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);
        //return traitdata.traitEffectsLevel[traitdata.traitEffectsLevel.Length - 1];
        return traitdata.traitEffectsLevel[0];
    }
    private Color GetTraitColor(TraitType trait, int level)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);
        int maxStack = GetTraitMaxStack(trait);

        Color[] traitColors = new Color[] {
        new Color(0f, 0f, 0f), // level 1 color
        new Color(0f, 0f, 1f), // level 2 color
        new Color(0f, 1f, 0f), // level 3 color
        new Color(1f, 0.8f, 0f), // level 4 color
        new Color(1f, 0f, 0f) // level 5 color
    };
        int colorIndex = 0;

        if (traitdata.traitEffectsLevel.Length > 0 && traitdata.traitEffectsLevel[0] <= level) colorIndex = 4;
        else if (traitdata.traitEffectsLevel.Length > 1 && traitdata.traitEffectsLevel[1] <= level) colorIndex = 3;
        else if (traitdata.traitEffectsLevel.Length > 2 && traitdata.traitEffectsLevel[2] <= level) colorIndex = 2;
        else if (traitdata.traitEffectsLevel.Length > 3 && traitdata.traitEffectsLevel[3] <= level) colorIndex = 1;
        else if (traitdata.traitEffectsLevel.Length > 4 && traitdata.traitEffectsLevel[4] <= level) colorIndex = 0;


        return traitColors[colorIndex];
    }
    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int contentlength = contentField.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentlength > characterWrapLimit) ? true : false;
        }

        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        //rectTransform.pivot = new Vector2(pivotX, pivotY * 2);
        transform.position = position;
        
    }
}
