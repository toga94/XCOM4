using System.Collections.Generic;
using UnityEngine;

public class TraitDataManager : Singleton<TraitDataManager>
{

    public List<TraitData> traitDataList;

    private Dictionary<TraitType, TraitData> traitDataDict;

    private void Awake()
    {
        traitDataDict = new Dictionary<TraitType, TraitData>();
        foreach (TraitData traitData in traitDataList)
        {
            traitDataDict.Add(traitData.traitType, traitData);
        }
    }

    public TraitData GetTraitData(TraitType traitType)
    {
        return traitDataDict.TryGetValue(traitType, out var traitData) ? traitData : null;
    }
}