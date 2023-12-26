using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using System.Collections.Generic;

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
    private TraitDataManager traitManager;

    private void Awake()
    {
        cardShop = CardShop.Instance;
    }
    private void OnEnable()
    {
        cardShop.onItemsChanged += ReDraw;
    }
    private void OnDisable()
    {
        cardShop.onItemsChanged -= ReDraw;
    }
    private void Start()
    {
        traitManager = TraitDataManager.Instance;
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

    private static readonly Dictionary<RareOptions, Color> RarityColors = new Dictionary<RareOptions, Color>
    {
        { RareOptions.Common, new Color(0.5f, 0.5f, 0.5f) },          // Grey
        { RareOptions.Uncommon, new Color(0.016f, 0.247f, 0.831f) },  // #043FD4
        { RareOptions.Rare, new Color(0.961f, 0f, 0.659f) },          // #F500A8
        { RareOptions.Epic, new Color(0.839f, 0.714f, 0.051f) },      // #D6B50D
        { RareOptions.Legendary, new Color(0.961f, 0.176f, 0.020f) }  // #F52D05
    };

    private static void RarityColor(Image cardBgImage, RareOptions cardRarity)
    {
        if (RarityColors.ContainsKey(cardRarity))
        {
            cardBgImage.color = RarityColors[cardRarity];
        }
    }
    private void TraitUI(UnitObject item, GameObject traitPanel)
    {
        foreach (TraitType trait in item.traits)
        {
            GameObject traitUIItem = traitCardItemUIPool.Spawn(traitPanel.transform);
            TraitUICatch traitUICatch = traitUIItem.GetComponent<TraitUICatch>();
            TraitTooltipTrigger traitTooltip = traitUICatch.traitTooltip;

            traitTooltip.traitData = traitManager.FetchTraitData(trait);

            traitUIItem.transform.localScale = new Vector3(2.5f, 2.5f, 0);

            traitUICatch.traitUIImage.sprite = GetTraitSprite(trait);

            traitUICatch.traitText.text = trait.ToString();
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
        TraitData traitdata = traitManager.FetchTraitData(trait);

        return traitdata.traitSprite;
    }

}
