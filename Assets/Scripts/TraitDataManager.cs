using System.Collections.Generic;
using System.Linq;

public class TraitDataManager : Singleton<TraitDataManager>
{
    public List<TraitData> traitDataList;

    private Dictionary<TraitType, TraitData> traitDataDict {
        get {

            return traitDataList.ToDictionary(traitData => traitData.traitType);

        }
    }

    public TraitData GetTraitData(TraitType traitType)
    {
        return traitDataDict.TryGetValue(traitType, out var traitData) ? traitData : null;
    }
}