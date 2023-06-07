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
    private List<GameObject> stateImageObjects; // To store the instantiated state image objects
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

        OnGameStateChanged(GameStateSystem.Instance.GetCurrentState);
    }

    private void OnGameStateChanged(GameState obj)
    {
        stateImageMap = new Dictionary<System.Type, Sprite>();
        stateImageMap.Add(typeof(CarouselState), carouselStateSprite);
        stateImageMap.Add(typeof(PlayerCombat_PhaseState), combatStateSprite);
        stateImageMap.Add(typeof(Minion_1_1_PhaseState), minionStateSprite);
        // Add other state types and their corresponding sprites

        UpdateList(obj);
    }

    private Sprite GetStateImage(GameState gameState)
    {

        System.Type stateType = gameState.GetType();

        // Check if the stateType exists in the stateImageMap dictionary
        if (stateImageMap.ContainsKey(stateType))
        {
            return stateImageMap[stateType];
        }

        // If no specific image is found for the state type, return a default image or handle it as needed
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

            // TODO: Instantiate and position the state image object in the UI

            // Example instantiation using a UI image component
            GameObject stateImageObject = stateUIPool.Spawn(Vector3.zero, Quaternion.identity);
            stateImageObject.transform.SetParent(transform); // Set the UI object as the parent
            Image uiImage = stateImageObject.GetComponent<Image>();
            uiImage.sprite = stateImage;


            if (GameStateSystem.Instance.GetCurrentStateIndexUI / 2 == i)
            {
                uiImage.color = Color.white;
            }
            else {
                uiImage.color = Color.grey;
            }

            //if (gameState == curstate)
            //{
            //    uiImage.color = Color.white;
            //}
            //else if (gameState.IsFinished && gameState.IsWin)
            //{
            //    uiImage.color = Color.green;
            //}
            //else if (gameState.IsFinished && !gameState.IsWin)
            //{
            //    uiImage.color = Color.red;
            //}
            //else 
            //{
            //    uiImage.color = Color.grey;
            //}



            // Position the state image object in the UI (you can modify this based on your UI layout)
            //stateUIListObj.transform.localPosition = new Vector3(0, -i * 100, 0);
            uiImage.transform.parent = stateUIListObj.transform;
            //stateImageObjects.Add(stateImageObject); // Add the object to the list
        }
    }

}
