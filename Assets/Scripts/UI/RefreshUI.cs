using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshUI : MonoBehaviour
{
    [SerializeField] GameObject refreshUI;
    private void OnEnable()
    {
        refreshUI.SetActive(true);
    }

    private void OnDisable()
    {
        refreshUI.SetActive(false);
    }
}
