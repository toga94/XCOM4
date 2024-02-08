using UnityEngine;

public class GoldSlots : MonoBehaviour
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private MeshRenderer[] slotsObjMaterial;
    void Start()
    {
        EconomyManager.OnGoldChanged += UpdateSlot;

        UpdateSlot(EconomyManager.GetGold());
    }


    void UpdateSlot(int gold)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) break;
            bool state = i < gold / 10;
            slots[i].SetActive(state);
           if(!state) slotsObjMaterial[i].material.DisableKeyword("_EMISSION");
           else slotsObjMaterial[i].material.EnableKeyword("_EMISSION");
        }
    }
}
