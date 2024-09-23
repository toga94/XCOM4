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
                if (traitCount.ContainsKey(traitData.traitType))
                {
                    currentTypeCount = traitCount[traitData.traitType];
                }
            }

            string colorTag = currentTypeCount >= traitData.traitEffectsLevel[i] ? "<color=white>" : "<color=grey>";
            string effectDescription = string.Empty;

            try
            {
                effectDescription = traitData.traitEffects[i].effectDescription;
            }
            catch (System.Exception)
            {
                effectDescription = string.Empty;
            }


            bunusEffects += $"\n {colorTag}{traitData.traitEffectsLevel[i]} units: {effectDescription}</color>";
        }

        contentField.text = $"{traitData.traitDescription} \n {bunusEffects}";
    }
    private int GetTraitMaxStack(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.FetchTraitData(trait);
        //return traitdata.traitEffectsLevel[traitdata.traitEffectsLevel.Length - 1];
        return traitdata.traitEffectsLevel[0];
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
