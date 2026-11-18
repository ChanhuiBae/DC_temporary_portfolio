using UnityEditor.Experimental;

public class ItemData : InventoryData
{
    public int amount;

    public ItemData(int inventoryID, int itemID, int amount) : base(1, itemID)
    {
        this.inventoryID = inventoryID;
        this.amount = amount;
    }
}
