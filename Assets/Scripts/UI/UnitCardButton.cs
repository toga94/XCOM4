using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardButton : MonoBehaviour
{
    private GameManager gameManager;
    public string CharacterName = "Lina";
    public Image CharacterImage;
    [SerializeField]private Text CharacterLabelText;
    void Start()
    {
        gameManager = GameManager.Instance;
        CharacterLabelText.text = CharacterName;
    }

    public void OnClick()
    {
        gameManager.SpawnUnitAtInventory(CharacterName);
    }
}
