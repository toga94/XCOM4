using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateListUI : MonoBehaviour
{
    public Sprite carouselStateSprite;
    public Sprite championSelectionStateSprite;
    public Sprite errorStateSprite;
    public Sprite combatStateSprite;
    public Sprite minionStateSprite;


    [SerializeField] private Transform stateUIListObj;

    [SerializeField] private GameObject stateListUISprite;

    [SerializeField]
    private Dictionary<System.Type, Sprite> stateImageMap;
    private LeanGameObjectPool stateUIPool;
    private List<GameObject> stateImageObjects; 
    private GameObject childObject;

    private void Start()
    {
        GameStateSystem.Instance.OnGameStateChanged += OnGameStateChanged;

        GameObject poolObj = GameObject.Find("_Pooling");
        childObject = new GameObject("StateListUIPool");
        childObject.transform.parent = poolObj.transform;
        stateUIPool = childObject.AddComponent<LeanGameObjectPool>();
        stateUIPool.Prefab = stateListUISprite;
        stateImageObjects = new List<GameObject>(); // Initialize the list
        stateImageMap = new Dictionary<System.Type, Sprite>();
        stateImageMap.Add(typeof(CarouselState), carouselStateSprite);
        stateImageMap.Add(typeof(PlayerCombat_PhaseState), combatStateSprite);
        stateImageMap.Add(typeof(Minion_1_1_PhaseState), minionStateSprite);

        OnGameStateChanged(GameStateSystem.Instance.GetCurrentState);
    }

    private void OnGameStateChanged(GameState obj)
    {
        Debug.Log("state" + obj.GetType());
        UpdateList(obj);
    }

    private Sprite GetStateImage(GameState gameState)
    {

        System.Type stateType = gameState.GetType();
        if (stateImageMap.ContainsKey(stateType))
        {
            return stateImageMap[stateType];
        }

        return errorStateSprite;
    }

    private void UpdateList(GameState curstate)
    {
        GameStateSystem gameStateSystem = GameStateSystem.Instance;
        List<GameState> statesInRound = gameStateSystem.GetStatesInRound();

        // Destroy previously instantiated state image objects
        foreach (GameObject stateImageObject in stateImageObjects)
        {
            Destroy(stateImageObject);
        }

        stateUIPool.DespawnAll();

        for (int i = 0; i < statesInRound.Count; i++)
        {

            GameState gameState = statesInRound[i];
            Sprite stateImage = GetStateImage(gameState);

            GameObject stateImageObject = stateUIPool.Spawn(Vector3.zero, Quaternion.identity);
            stateImageObject.transform.SetParent(transform); 
            Image uiImage = stateImageObject.GetComponent<Image>();
            uiImage.sprite = stateImage;


            int _currentStateIndex = GameStateSystem.Instance.GetCurrentStateIndexUI / 2;
            Debug.Log(_currentStateIndex);
            if (_currentStateIndex == i)
            {
                uiImage.color = Color.white;
            }
            else if (_currentStateIndex < i)
            {
                uiImage.color = Color.black;
            }
            else
            {
                GameState stateInRound = GameStateSystem.Instance.GetStatesInRound()[i];
                if (stateInRound is Minion_1_1_PhaseState || stateInRound is PlayerCombat_PhaseState)
                { 
                    if (GameStateSystem.Instance.GetStatesInRound()[i].IsWin)
                    {
                        uiImage.color = Color.green;
                    }
                    else
                    {
                        uiImage.color = Color.red;
                    }
                }
                else
                {
                    uiImage.color = Color.grey;
                }
            }
            uiImage.transform.parent = stateUIListObj.transform;
        }
    }

}
