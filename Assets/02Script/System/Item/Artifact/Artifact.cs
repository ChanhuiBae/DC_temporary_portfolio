using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental;
using UnityEngine;
public class Artifact
{

    private int inventoryID;
    private int artifactID;
    public int GetID(bool getArtifactID)
    {
        if (getArtifactID)
            return artifactID;
        else
            return inventoryID;
    }
    public void SetID(bool setArtifactID, int ID)
    {
        if (setArtifactID)
        {
            artifactID = ID;
            GameManager.Inst.GetArtifactData(artifactID, out data);
        }
        else
            inventoryID = ID;
    }

    private Entity_Artifact data;
    public Entity_Artifact Data
    {
        get => data;
    }

    private ArtifactManager manager;
    
    public Artifact(int artifactID)
    {
        SetArtifact(artifactID);
    }

    public void SetArtifact(int artifactID)
    {
        inventoryID = GameManager.Inst.IDMaker;
        this.artifactID = artifactID;
        GameManager.Inst.GetArtifactData(artifactID, out data);
    }

    public void EquipArtifact()
    {
        manager = GameManager.Inst.artifactManager;
        manager.EquipArtifact(this);
    }

    public void UneuipArtifact() 
    {
        manager = GameManager.Inst.artifactManager;
        manager.UneuipArtifact(this);
    }

}
