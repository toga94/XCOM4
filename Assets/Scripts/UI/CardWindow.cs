using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
public class CardWindow : MonoBehaviour
{
    [SerializeField] private CardShop cardShop;
    [SerializeField] private RectTransform itemPanel;
    //private GameObject unitCard;
    [SerializeField]
    private LeanGameObjectPool unitCardPool;
    [SerializeField]
    private LeanGameObjectPool traitCardItemUIPool;
    private void Awake()
    {
        cardShop = CardShop.Instance;
        //unitCard = (GameObject)Resources.Load("UnitCard");
        //traitCardItemUI = (GameObject)Resources.Load("traitCardItemUI");
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
        traitCardItemUIPool.DespawnAll();
        unitCardPool.DespawnAll();
       
       
        foreach (UnitObject item in e)
        {
            GameObject card = unitCardPool.Spawn(transform.position, Quaternion.identity, itemPanel.transform) as GameObject;
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
            GameObject DisabledPanel = card.transform.Find("Disabled").gameObject;
            DisabledPanel.SetActive(false);
            Button unitCardButton = card.GetComponent<Button>();
            unitCardButton.enabled = true;
            foreach (var trait in item.traits)
            {
                GameObject traitUIItem = traitCardItemUIPool.Spawn(traitPanel.transform);
                traitUIItem.transform.localScale = new Vector3(2.5f, 2.5f, 0);
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
