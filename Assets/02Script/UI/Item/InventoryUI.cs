using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private List<ItemButton> items = new List<ItemButton>();


    private void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            items.Add(transform.GetChild(i).GetComponent<ItemButton>());
        }
    }

    public void Start()
    {
        SetInventoryUI();
    }

    public void SetInventoryUI()
    {
        List<Item> list = GameManager.Inst.Exploration.player.invenory.GetItemList();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < list.Count)
            {
                items[i].SetButton(list[i]);
            }
            else if (i < GameManager.Inst.Exploration.player.invenory.MAXSlotCount)
            {
                items[i].SetEmpty();
            }
            else
            {
                items[i].SetLock();
            }
        }
    }

    public bool AddItem(Item item, out int remain)
    {
        bool result = GameManager.Inst.Exploration.player.invenory.AddItem(item, out remain);
        SetInventoryUI();
        return result;
    }

    public bool DeleteItem(int id, int amount)
    {
        bool result = GameManager.Inst.Exploration.player.invenory.DeleteItemAmount(id, amount);
        SetInventoryUI();
        return result;
    }
}
