using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private Text roundText;
    private GameStateSystem gameStateSystem;


    private void Start()
    {
        gameStateSystem = GameStateSystem.Instance;
        gameStateSystem.OnGameStateChanged += UpdateText;
        UpdateText();
    }

    void UpdateText(GameState gameState)
    {
        ChangeText();
    }
    void UpdateText()
    {
        ChangeText();
    }

    private void ChangeText()
    {
        roundText.text = $"Round : {gameStateSystem.GetRoundIndex + 1} - {gameStateSystem.GetCurrentStateIndexUI / 2}";
    }

}
