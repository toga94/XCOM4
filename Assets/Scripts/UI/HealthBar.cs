using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image damageEffectImage;
    [SerializeField] private float damageEffectDuration = 0.5f;
    private float targetFillAmount;

    private void Awake()
    {
        targetFillAmount = foregroundImage.fillAmount;
    }

    public void SetHealth(float healthNormalized)
    {
        targetFillAmount = healthNormalized;
    }

    public void ApplyDamageEffect(float healthPercentageLost)
    {
        float damageEffectWidth = foregroundImage.rectTransform.rect.width * healthPercentageLost;
        damageEffectImage.rectTransform.sizeDelta = new Vector2(damageEffectWidth, damageEffectImage.rectTransform.rect.height);
        damageEffectImage.enabled = true;
        Invoke("HideDamageEffect", damageEffectDuration);
    }

    private void HideDamageEffect()
    {
        damageEffectImage.enabled = false;
    }

    private void Update()
    {
        foregroundImage.fillAmount = Mathf.Lerp(foregroundImage.fillAmount, targetFillAmount, Time.deltaTime * 10f);
    }
}