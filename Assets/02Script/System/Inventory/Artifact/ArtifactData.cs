using System;
using Unity.VisualScripting;

[Serializable]
public class ArtifactData : InventoryData
{
    public int level;

    public ArtifactData(int inventoryID, int artifactID, int level) : base(2, artifactID)
    {
        this.inventoryID = inventoryID;
        this.level = level;
    }
}
