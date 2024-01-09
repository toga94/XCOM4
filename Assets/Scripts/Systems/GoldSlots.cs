using UnityEngine;

public class GoldSlots : MonoBehaviour
{
    [SerializeField] private GameObject[] slots;
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
            slots[i].SetActive(i < gold / 10);
        }
    }
}
