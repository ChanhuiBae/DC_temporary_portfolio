public class Item
{
    private int inventoryID;
    private int itemID;
    public int GetID(bool getItemID)
    {
        if (getItemID)
            return itemID;
        else
            return inventoryID;
    }
    public void SetID(bool setItemID, int ID)
    {
        if (setItemID)
            itemID = ID;
        else
            inventoryID = ID;
    }

    private Entity_Item data;
    public Entity_Item Data
    {
        get
        {
            GameManager.Inst.GetItemData(itemID, out data);
            return data;
        }
    }

    private int amount;
    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
            if (amount > data.MaxCount)
            {
                amount = data.MaxCount;
            }
            else if(amount < 1)
            {
                amount = 0;
            }
        }
    }
    public int AddAmount(int value)
    {
        if (amount + value > data.MaxCount)
        {
            int current = amount;
            amount = data.MaxCount;
            return (current + value) - data.MaxCount;
        }
        else
        {
            amount += value;
            return 0;
        }
    }

    new public string GetType()
    {
        return data.Type;
    }

    public Item(int itemID, int amount)
    {
        inventoryID = GameManager.Inst.IDMaker;
        this.itemID = itemID;
        GameManager.Inst.GetItemData(itemID, out data);
        this.amount = amount;
    }
}
