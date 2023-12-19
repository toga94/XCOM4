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

        stateUIPool.DespawnAll();

        int currentStateIndexUI = gameStateSystem.GetCurrentStateIndexUI / 2;

        for (int i = 0; i < statesInRound.Count; i++)
        {
            GameState gameState = statesInRound[i];
            Sprite stateImage = GetStateImage(gameState);

            GameObject stateImageObject = stateUIPool.Spawn(Vector3.zero, Quaternion.identity);
            stateImageObject.transform.SetParent(transform);
            Image uiImage = stateImageObject.GetComponent<Image>();
            uiImage.sprite = stateImage;

            if (currentStateIndexUI == i)
            {
                uiImage.color = Color.white;
            }
            else if (currentStateIndexUI < i)
            {
                uiImage.color = Color.black;
            }
            else
            {
                SetStateColor(uiImage, gameState, i);
            }
            uiImage.transform.parent = stateUIListObj.transform;
        }
    }

    private void SetStateColor(Image uiImage, GameState state, int index)
    {
        if (state is Minion_1_1_PhaseState || state is PlayerCombat_PhaseState)
        {
            if (state.IsWin)
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


}
