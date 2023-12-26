using System.Collections.Generic;
using System.Linq;

public class TraitDataManager : Singleton<TraitDataManager>
{
    public List<TraitData> traitDataList;

    private Dictionary<TraitType, TraitData> TraitDataDictionary {
        get {

            return traitDataList.ToDictionary(traitData => traitData.traitType);

        }
    }

    public TraitData FetchTraitData(TraitType traitType)
    {
        return TraitDataDictionary.TryGetValue(traitType, out var traitData) ? traitData : null;
    }
}