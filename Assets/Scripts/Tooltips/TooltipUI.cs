using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private LayoutElement layoutElement;

    [SerializeField] private int characterWrapLimit;

    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetText(TraitData traitData)
    {
        headerField.text = traitData.name;
        contentField.text = traitData.traitDescription;
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
