using System.Collections.Generic;
using UnityEngine;

public class TraitDataManager : MonoBehaviour
{
    public static TraitDataManager Instance { get; private set; }

    public List<TraitData> traitDataList;

    private Dictionary<TraitType, TraitData> traitDataDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            traitDataDict = new Dictionary<TraitType, TraitData>();
            foreach (TraitData traitData in traitDataList)
            {
                traitDataDict.Add(traitData.traitType, traitData);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TraitData GetTraitData(TraitType traitType)
    {
        if (traitDataDict.TryGetValue(traitType, out TraitData traitData))
        {
            return traitData;
        }
        else
        {
            Debug.LogError("Could not find trait data for trait " + traitType);
            return null;
        }
    }
}