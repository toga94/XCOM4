using UnityEngine;

public class GoldSlots : MonoBehaviour
{
    [SerializeField] private Animator[] slots;
    [SerializeField] private MeshRenderer[] slotsRender;
    [SerializeField] private GameObject[] slotsLight;

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
            slotsLight[i].SetActive(state);
            slots[i].speed = state ? 1 : 0;
           if(!state) slotsRender[i].material.DisableKeyword("_EMISSION");
          else slotsRender[i].material.EnableKeyword("_EMISSION");
        }
    }
}
