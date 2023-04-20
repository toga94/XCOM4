using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
public class CardWindow : MonoBehaviour
{
    [SerializeField] 
    private CardShop cardShop;
    [SerializeField] 
    private RectTransform itemPanel;
    [SerializeField]
    private LeanGameObjectPool unitCardPool;
    [SerializeField]
    private LeanGameObjectPool traitCardItemUIPool;
    private void Awake()
    {
        cardShop = CardShop.Instance;
    }
    private void OnEnable()
    {
        cardShop.onItemsChanged += ReDraw;
    }
    private void ReDraw(object sender, UnitObject[] e)
    {
        Transform itemPanelTransform = itemPanel.transform;
        traitCardItemUIPool.DespawnAll();
        unitCardPool.DespawnAll();

        foreach (UnitObject item in e)
        {
            GameObject card = unitCardPool.Spawn(transform.position, Quaternion.identity, itemPanel.transform) as GameObject;
            Transform cardTransform = card.transform;
            Image cardBgImage = cardTransform.Find("TextFrame").GetComponent<Image>();

            UnitCardButton unitCardButton = card.GetComponent<UnitCardButton>();
            unitCardButton.unit = item.Prefab.GetComponent<Unit>();
            unitCardButton.CharacterImage.sprite = item.unitImage;
            unitCardButton.CharacterName = item.unitName;
            Text unitNameText = cardTransform.Find("Text").GetComponent<Text>();
            unitNameText.text = item.unitName;

            RareOptions cardRarity = item.rareOptions;
            RarityColor(cardBgImage, cardRarity);
            RarityGold(card, cardRarity);
            unitCardButton.rareOptions = cardRarity;

            GameObject traitPanel = cardTransform.Find("traitPanel").gameObject;
            GameObject DisabledPanel = cardTransform.Find("Disabled").gameObject;

            TraitUI(item, traitPanel);
            ReEnableUI(card, DisabledPanel);

            unitCardButton.CheckUpgrade();
        }
    }

    private static void RarityColor(Image cardBgImage, RareOptions cardRarity)
    {
        switch (cardRarity)
        {
            case RareOptions.Common:
                cardBgImage.color = new Color(0.5f, 0.5f, 0.5f); // Grey
                break;
            case RareOptions.Uncommon:
                cardBgImage.color = new Color(0.016f, 0.247f, 0.831f); // #043FD4
                break;
            case RareOptions.Rare:
                cardBgImage.color = new Color(0.961f, 0f, 0.659f); // #F500A8
                break;
            case RareOptions.Epic:
                cardBgImage.color = new Color(0.839f, 0.714f, 0.051f); // #D6B50D
                break;
            case RareOptions.Legendary:
                cardBgImage.color = new Color(0.961f, 0.176f, 0.020f); // #F52D05
                break;
        }
    }

    private void TraitUI(UnitObject item, GameObject traitPanel)
    {
        TraitDataManager traitManager = TraitDataManager.Instance;
        foreach (TraitType trait in item.traits)
        {
            GameObject traitUIItem = traitCardItemUIPool.Spawn(traitPanel.transform);
            var traitTooltip = traitUIItem.GetComponent<TraitTooltipTrigger>();

            traitTooltip.traitData = traitManager.GetTraitData(trait);
            Transform traitUITransform = traitUIItem.transform;
            traitUITransform.localScale = new Vector3(2.5f, 2.5f, 0);
            Image traitUIImage = traitUITransform.Find("traitImage").GetComponent<Image>();
            traitUIImage.sprite = GetTraitSprite(trait);
            Text traitText = traitUITransform.Find("traitText").GetComponent<Text>();
            traitText.text = trait.ToString();
        }
    }

    private static void ReEnableUI(GameObject card, GameObject DisabledPanel)
    {
        DisabledPanel.SetActive(false);
        Button unitCardButton = card.GetComponent<Button>();
        unitCardButton.enabled = true;
    }

    private static void RarityGold(GameObject card, RareOptions cardRarity)
    {
        int rarityGold = ((int)cardRarity) + 1;
        Text goldtext = card.transform.Find("gold").GetComponent<Text>();
        goldtext.text = rarityGold.ToString();
    }

    private Sprite GetTraitSprite(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);

        return traitdata.traitSprite;
    }

}
