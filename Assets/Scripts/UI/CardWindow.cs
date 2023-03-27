using UnityEngine;
using UnityEngine.UI;

public class CardWindow : MonoBehaviour
{
    [SerializeField] private CardShop cardShop;
    [SerializeField] private RectTransform itemPanel;

    private void Start()
    {
        cardShop = CardShop.Instance;
        cardShop.onItemAdded += ReDraw;
    }
    private void OnDestroy()
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
