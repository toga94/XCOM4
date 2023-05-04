using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraitDataManager : Singleton<TraitDataManager>
{

    public List<TraitData> traitDataList;

    private Dictionary<TraitType, TraitData> traitDataDict;

    private void Awake()
    {
        traitDataDict = traitDataList.ToDictionary(traitData => traitData.traitType);
    }

    public TraitData GetTraitData(TraitType traitType)
    {
        return traitDataDict.TryGetValue(traitType, out var traitData) ? traitData : null;
    }
}