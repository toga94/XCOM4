using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaginationController : MonoBehaviour
{
    [SerializeField]
    private VerticalLayoutGroup verticalLayoutGroup;


    [SerializeField]
    private int itemsPerPage = 3;
    [SerializeField]
    private int currentPageIndex = 0;
    private int pageCount;
    private int itemCount;
    void Start()
    {
         itemCount = verticalLayoutGroup.transform.childCount;
         pageCount = Mathf.CeilToInt((float)itemCount / (float)itemsPerPage);
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

    void RefreshLayout()
    {
        int startIndex = currentPageIndex * itemsPerPage;
        int endIndex = (currentPageIndex + 1) * itemsPerPage;
        int childCount = verticalLayoutGroup.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = verticalLayoutGroup.transform.GetChild(i);
            child.gameObject.SetActive(i >= startIndex && i < endIndex);
        }

        verticalLayoutGroup.CalculateLayoutInputVertical();
        verticalLayoutGroup.SetLayoutVertical();
    }


    public void NextPage()
    {
        currentPageIndex = Mathf.Min(currentPageIndex + 1, pageCount - 1);
        RefreshLayout();
    }

    public void PreviousPage()
    {
        currentPageIndex = Mathf.Max(currentPageIndex - 1, 0);
        RefreshLayout();
    }

    void GoToPage(int pageIndex)
    {
        currentPageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);
        RefreshLayout();
    }
}