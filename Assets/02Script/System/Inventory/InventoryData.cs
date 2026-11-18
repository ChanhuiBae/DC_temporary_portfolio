
using System.Xml;

public class InventoryData
{
    protected int typeID;
    protected int inventoryID;
    protected int uniqueID;

    public int Type => typeID;
    public int InventoryID => inventoryID;
    public int UniqueID => uniqueID;

    public InventoryData(int typeID, int uniqueID)
    {
        this.typeID = typeID;
        this.uniqueID = uniqueID;
    }
}
