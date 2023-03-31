using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PaginationController : MonoBehaviour
{
    [SerializeField]
    private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField]
    private int itemsPerPage = 3;
    [SerializeField]
    private GameObject traitCountUIObj;
    [SerializeField]
    private Text nextTraitsCountText;
    [SerializeField]
    private RectTransform traitCountUIRect;

    private int currentPageIndex = 0;
    private int pageCount;
    private int itemCount;

    private void Start()
    {
        itemCount = verticalLayoutGroup.transform.childCount;
        pageCount = Mathf.CeilToInt((float)itemCount / (float)itemsPerPage);
        RefreshLayout();
    }

    private void OnEnable()
    {
        //traitCountUIRect = traitCountUIRect.GetComponent<RectTransform>();
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += UpdateTraitLayout;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += UpdateTraitLayout;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition += UpdateTraitLayout;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += UpdateTraitLayout;
        StartCoroutine(UpdateTraits());
    }

    private void OnDisable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= UpdateTraitLayout;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= UpdateTraitLayout;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition -= UpdateTraitLayout;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition -= UpdateTraitLayout;
    }

    private void UpdateTraitLayout(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        StartCoroutine(UpdateTraits());
    }

    private void UpdateTraitLayout(object sender, InventoryGrid.OnAnyUnitMovedInventoryPositionEventArgs e)
    {
        StartCoroutine(UpdateTraits());
    }
    private void UpdateTraitLayout(object sender, LevelGrid.OnAnyUnitSwappedGridPositionEventArgs e)
    {
        StartCoroutine(UpdateTraits());
    }

    private void UpdateTraitLayout(object sender, InventoryGrid.OnAnyUnitSwappedInventoryPositionEventArgs e)
    {
        StartCoroutine(UpdateTraits());
    }

    IEnumerator UpdateTraits()
    {
        yield return new WaitForSeconds(0.0012f);
        Start();
    }
    [SerializeField] private RectTransform lastitemTransform;

    private void RefreshLayout()
    {
        int startIndex = currentPageIndex * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, itemCount);

        int inactiveTraitsCount = 0;

        for (int i = 0; i < itemCount; i++)
        {
            bool isActive = i >= startIndex && i < endIndex;
            verticalLayoutGroup.transform.GetChild(i).gameObject.SetActive(isActive);
            if (!isActive) inactiveTraitsCount++;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());

        int nextTraitsCount = Mathf.Max(0, itemCount - (endIndex - 1));
        if (inactiveTraitsCount > 0)
        {
            traitCountUIObj.SetActive(true);
            nextTraitsCountText.text = $"+{inactiveTraitsCount}";
            lastitemTransform = verticalLayoutGroup.transform.GetChild(itemCount - inactiveTraitsCount - 1 ).GetComponent<RectTransform>();
            float offset = traitCountUIRect.rect.height + 40;
            traitCountUIRect.transform.position = new Vector3(traitCountUIRect.transform.position.x, lastitemTransform.transform.position.y - offset , 0);
        }
        else
        {
            traitCountUIObj.SetActive(false);
        }


    }

    public void NextPage()
    {
        if (currentPageIndex < pageCount - 1)
        {
            currentPageIndex++;
            Start();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            Start();
        }
    }
    public void NextOrFirstPage()
    {
        if (currentPageIndex < pageCount - 1)
        {
            currentPageIndex++;
        }
        else
        {
            currentPageIndex = 0;
        }
        Start();
    }

    public void PreviousOrLastPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
        }
        else
        {
            currentPageIndex = pageCount - 1;
        }
        Start();
    }
    public void GoToPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < pageCount)
        {
            currentPageIndex = pageIndex;
            Start();
        }
    }
}