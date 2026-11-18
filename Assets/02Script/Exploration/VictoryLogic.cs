using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VictoryEvent
{
    GetLoot = 0,
    LevelUp = 1,
    Merchant = 2
}
public class VictoryLogic
{
    private bool access = true;
    private Dictionary<int,NotifyLogic> notifies = new Dictionary<int, NotifyLogic>();
    public bool AddNotifyLogic(int key, NotifyLogic item)
    {
        if (access)
        {
            notifies.Add(key, item); 
            notifies.OrderBy(item => item.Key).ToDictionary(x => x.Key, x => x.Value);
            return true;
        }
        return false;
    }

    public void StartVictoryLogic()
    {
        OnVictoryProcess();
    }

    protected virtual void OnVictoryProcess()
    {
        if(notifies.Count > 0)
        {
            access = false;
            notifies.Last().Value.StartProcess();
            if(notifies.Count > 0)
                notifies.Remove(notifies.Last().Key);
            access = true;
        }
    }
}
