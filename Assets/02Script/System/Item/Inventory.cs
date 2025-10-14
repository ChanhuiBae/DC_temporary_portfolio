using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Inventory
{
    private ExplorationManager manager = (ExplorationManager)GameManager.Inst.manager;
    protected int maxSlotCount = 10;
    public int MAXSlotCount
    {
        get => maxSlotCount;
    }

    public bool IsFull
    {
        get { return items.Count >= maxSlotCount; }
    }
    public int DeltaSlotCount
    {
        get { return maxSlotCount - items.Count; }
    }

    protected List<Item> items;

    public Inventory()
    {
        items = new List<Item>();
    }

    public Inventory(int max)
    {
        this.maxSlotCount = max;
        items = new List<Item>();
        Item item = new Item(1, 5);
        items.Add(item);
        item = new Item(2, 4);
        items.Add(item);
        item = new Item(3, 3);
        items.Add(item);
        items.Add(new Item(1, 2));
    }

    public void Copy(Inventory data)
    {
        maxSlotCount = data.maxSlotCount;
        items = data.GetItemList();
    }


    public int FindIndexByInventoryID(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].GetID(false) == id)
            {
                return i;
            }
        }
        return -1;
    }

    public List<int> FindIndexByItemID(int itemID)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].GetID(true) == itemID)
            {
                result.Add(i);
            }
        }
        return result;
    }

    public bool CheckAddItem(int id, int amount)
    {
        List<int> indexs = FindIndexByItemID(id);
        if(indexs.Count == 0)
        {
            if (IsFull)
                return false;
            return true;
        }

        int delta = items[indexs[indexs.Count - 1]].Data.MaxCount - items[indexs[indexs.Count - 1]].Amount;
        if (delta >= amount)
            return true;
        else if (delta == 0)
        {
            if (IsFull) 
                return false;
            return true;
        }
        else
        {
            if (IsFull)
                return false;
            return true;
        }
    }

    public bool AddItem(Item newItem, out int remain)
    {
        List<int> indexs = FindIndexByItemID(newItem.GetID(true));
        if (indexs.Count == 0) // haven't in inventory
        {
            if (IsFull)
            {
                remain = newItem.Amount;
                return false;
            }

            items.Add(newItem);
            remain = 0;
            return true;
        }
  
        int delta = newItem.Data.MaxCount - items[indexs[indexs.Count - 1]].Amount;
        if (delta >= newItem.Amount) // add at last same item
        {
            items[indexs[indexs.Count - 1]].AddAmount(newItem.Amount);
            remain = 0;
            return true;
        }
        else if(delta == 0)
        {
            if (IsFull) // can't store. It's full.
            {
                remain = newItem.Amount;
                return false;
            }

            items.Add(newItem); // It store newly.
            remain = 0;
            return true;
        }
        else
        {
            items[indexs[indexs.Count - 1]].AddAmount(delta); // add at last same item to been full,
            if (IsFull)
            {
                remain = newItem.Amount - delta;
                return false;
            }

            items.Add(newItem); // It store newly.
            remain = 0;
            return true;
        }
    }

    public List<Item> GetItemList()
    {
        return items;
    }

    public int GetItemAmount(int itemID)
    {
        List<int> indexs = FindIndexByItemID(itemID);

        if (indexs.Count == 0)
            return 0;
        else
        {
            int amount = 0;
            foreach (int i in indexs)
            {
                amount += items[i].Amount;
            }
            return amount;
        }
    }

    public Item GetItem(int inventoryID)
    {
        int index = FindIndexByInventoryID(inventoryID);
        if (index != -1)
        {
            return items[index];
        }
        return null;
    }

    public bool DeleteItemAmount(int itemID, int amount)
    {
        if(GetItemAmount(itemID) >= amount)
        {
            List<int> indexs = FindIndexByItemID(itemID);
            if (indexs.Count > 0)
            {
                int i = indexs.Count - 1;
                while(i > -1)
                {
                    items[indexs[i]].Amount -= amount;
                    if (items[indexs[i]].Amount < 1)
                    {
                        amount = Mathf.Abs(items[indexs[i]].Amount);
                        items.RemoveAt(indexs[i]);
                        i -= 1;
                    }
                    else
                        break;
                }
                return true;
            }
        }
        return false;
    }

    public bool DeleteItem(Item deleteItem)
    {
        int index = FindIndexByInventoryID(deleteItem.GetID(false));
        if (-1 < index)
        {
            items[index].Amount -= deleteItem.Amount;
            if (items[index].Amount < 1)
            {
                items.RemoveAt(index);
            }
            return true;
        }
        return false;
    }

    public void UpdateItemInfo(Item data)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].GetID(false) == data.GetID(false))
            {
                items[i].SetID(false, data.GetID(false));
                items[i].Amount = data.Amount;
            }
        }
    }
}
