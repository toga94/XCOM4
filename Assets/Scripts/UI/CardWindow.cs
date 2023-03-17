using UnityEngine;

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
            UnitCardButton cardButton = card.GetComponent<UnitCardButton>();
            cardButton.CharacterImage.sprite = item.unitImage;
            cardButton.CharacterName = item.unitName;
        }
    }
}
