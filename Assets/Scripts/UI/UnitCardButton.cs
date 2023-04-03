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

    private void LateUpdate()
    {
        bool upgradeTo3Star = GameManager.Instance.GetCountUpgradeTo3Star(unit);
        bool upgradeTo2Star = GameManager.Instance.GetCountUpgradeTo2Star(unit);

        TreeStarPanel.SetActive(upgradeTo3Star);
        TwoStarPanel.SetActive(!upgradeTo3Star && upgradeTo2Star);
    }


    public void OnClick()
    {
        gameManager.SpawnUnitAtInventory(CharacterName);
    }
}
