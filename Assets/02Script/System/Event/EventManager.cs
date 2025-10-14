using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<GameEventType, List<IListener>> listeners = new Dictionary<GameEventType, List<IListener>>();

    public void AddListener(GameEventType eventType, IListener listener)
    {
        List<IListener> listenerList = null;
        if (listeners.TryGetValue(eventType, out listenerList))
        {
            listenerList.Add(listener);
        }
        else
        {
            listenerList = new List<IListener>();
            listenerList.Add(listener);
            listeners.Add(eventType, listenerList);
        }
    }

    public void PostNotification(GameEventType eventType, object param = null)
    {
        List<IListener> listenerList = null;

        if (listeners.TryGetValue(eventType, out listenerList))
        {
            foreach (var listener in listenerList)
                listener.OnEvent(eventType, param);
        }
    }
    public void PostNotification(GameEventType eventType, object sender, object param = null)
    {
        List<IListener> listenerList = null;

        if (listeners.TryGetValue(eventType, out listenerList))
        {
            foreach (var listener in listenerList)
                listener.OnEvent(eventType, sender, param);
        }
    }

    public void RemoveEvent(GameEventType eventType) => listeners.Remove(eventType);

    public void RemoveRedundancies()
    {
        Dictionary<GameEventType, List<IListener>> newListeners = new Dictionary<GameEventType, List<IListener>>();

        foreach(var listener in listeners)
        {
            for(int i = listener.Value.Count - 1; i >= 0; i--)
            {
                if (listener.Value[i].Equals(null))
                    listener.Value.RemoveAt(i);
            }

            if (listener.Value.Count > 0)
                newListeners.Add(listener.Key, listener.Value);
        }
        listeners = newListeners;   
    }

    private void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }

}
