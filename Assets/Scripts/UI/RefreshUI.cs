using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween namespace

public class RefreshUI : MonoBehaviour
{
    [SerializeField] GameObject[] refreshUI;
    [SerializeField] float fadeDuration = 0.5f; // Duration for fade-in and fade-out

    private void OnEnable()
    {
        foreach (var item in refreshUI)
        {
            Image img = item.GetComponent<Image>();
            if (img != null)
            {
                Color color = img.color;
                color.a = 0f; // Start with invisible
                img.color = color;
                item.SetActive(true);
                img.DOFade(1f, fadeDuration); // Fade in
            }
        }
    }

    private void OnDisable()
    {
        foreach (var item in refreshUI)
        {
            Image img = item.GetComponent<Image>();
            if (img != null)
            {
                img.DOFade(0f, fadeDuration).OnComplete(() => item.SetActive(false)); // Fade out and then disable
            }
            else
            {
                item.SetActive(false);
            }
        }
    }
}
