using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void Notify();
public class NotifyLogic 
{
    public event Notify notifyEvent;
    public void StartProcess()
    {
        OnProcessCompleted();
    }

    protected virtual void OnProcessCompleted()
    {
        notifyEvent?.Invoke();
    }
}
