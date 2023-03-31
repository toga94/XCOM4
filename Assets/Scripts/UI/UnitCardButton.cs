using UnityEngine;
using UnityEngine.UI;

public class UnitCardButton : MonoBehaviour
{
    private GameManager gameManager;
    public string CharacterName = "Lina";
    public Image CharacterImage;
    [SerializeField] private Text CharacterLabelText;
    public GameObject TreeStarPanel;
    public GameObject TwoStarPanel;

    public Unit unit;

    void Start()
    {
        gameManager = GameManager.Instance;
        CharacterLabelText.text = CharacterName;
    }

    private void Update()
    {

        if (GameManager.Instance.GetCountUpgradeTo3Star(unit))
        {
            TreeStarPanel.SetActive(true);
            TwoStarPanel.SetActive(false);
        }
        else
        {
            if (GameManager.Instance.GetCountUpgradeTo2Star(unit))
            {
                TwoStarPanel.SetActive(true);
                TreeStarPanel.SetActive(false);
            }
            else {
                TwoStarPanel.SetActive(false);
                TreeStarPanel.SetActive(false);
            }
        }
    }


    public void OnClick()
    {
        gameManager.SpawnUnitAtInventory(CharacterName);
    }
}
