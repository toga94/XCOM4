using System;
using UnityEngine;
using UnityEngine.UI;

public class PaginationController : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private int itemsPerPage = 3;
    [SerializeField] private Text nextTraitsCountText;

    private int currentPageIndex = 0;
    private int pageCount;
    private int itemCount;

    private void Start()
    {
        itemCount = verticalLayoutGroup.transform.childCount;
        pageCount = Mathf.CeilToInt((float)itemCount / (float)itemsPerPage);
        RefreshLayout();
        Debug.Log("LayoutRefresh");
    }

    private void OnEnable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += UpdateTraitLayout;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += UpdateTraitLayout;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition += UpdateTraitLayout;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += UpdateTraitLayout;
    }

    private void OnDisable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= UpdateTraitLayout;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= UpdateTraitLayout;
    }

    private void UpdateTraitLayout(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        Start();
    }

    private void UpdateTraitLayout(object sender, InventoryGrid.OnAnyUnitMovedInventoryPositionEventArgs e)
    {
        Start();
    }
    private void UpdateTraitLayout(object sender, LevelGrid.OnAnyUnitSwappedGridPositionEventArgs e)
    {
        Start();
    }

    private void UpdateTraitLayout(object sender, InventoryGrid.OnAnyUnitSwappedInventoryPositionEventArgs e)
    {
        Start();
    }
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
        nextTraitsCountText.text = $"+{inactiveTraitsCount}";
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

    public void GoToPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < pageCount)
        {
            currentPageIndex = pageIndex;
            Start();
        }
    }
}