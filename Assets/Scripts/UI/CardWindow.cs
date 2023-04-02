using UnityEngine;
using UnityEngine.UI;

public class CardWindow : MonoBehaviour
{
    [SerializeField] private CardShop cardShop;
    [SerializeField] private RectTransform itemPanel;

    private void Awake()
    {
        cardShop = CardShop.Instance;

    }
    private void OnEnable()
    {
        cardShop.onItemAdded += ReDraw;
    }
    private void OnDisable()
    {
        cardShop.onItemAdded -= ReDraw;
    }

    private void ReDraw(object sender, UnitObject[] e)
    {
        Transform itemPanelTransform = itemPanel.transform;
        foreach (Transform child in itemPanelTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (UnitObject item in e)
        {
            GameObject card = Instantiate(Resources.Load("UnitCard"), transform.position, Quaternion.identity, itemPanel.transform) as GameObject;
            Image cardBgImage = card.transform.Find("TextFrame").GetComponent<Image>();
            RareOptions cardRarity = item.rareOptions;
            card.GetComponent<UnitCardButton>().unit = item.Prefab.GetComponent<Unit>();


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


            GameObject traitPanel = card.transform.Find("traitPanel").gameObject;
            foreach (var trait in item.traits)
            {
                GameObject traitUIItem = (GameObject)Instantiate(Resources.Load("traitCardItemUI"), traitPanel.transform);
                Image traitUIImage = traitUIItem.transform.Find("traitImage").GetComponent<Image>();
                traitUIImage.sprite = GetTraitSprite(trait);
                Text traitText = traitUIItem.transform.Find("traitText").GetComponent<Text>();
                traitText.text = trait.ToString();
            }
            UnitCardButton cardButton = card.GetComponent<UnitCardButton>();
            cardButton.CharacterImage.sprite = item.unitImage;
            cardButton.CharacterName = item.unitName;
        }
    }

    private Sprite GetTraitSprite(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);

        return traitdata.traitSprite;
    }

}
