using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public enum AttributeType
{
    None = 0,
    Fire = 1,
    Water = 2,
    Thunder = 3,
    Earth = 4
}

public class AttributeManager : MonoBehaviour
{
    private Image[] show = new Image[4];
    private List<AttributeType> attributes = new List<AttributeType>();
    private bool deleting;
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent<Image>(out show[i]))
            {
                Debug.Log("AttributeManager - Awake - Image");
            }
        }
        deleting = false;
    }

    private void Start()
    {
        for (int i = 0; i < show.Length; i++)
        {
            show[i].enabled = false;
        }
    }

    private void UpdateAttribute()
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            show[i].enabled = true;
            switch (attributes[i])
            {
                case AttributeType.Fire:
                    show[i].color = Color.red;
                    break;
                case AttributeType.Water:
                    show[i].color = Color.blue;
                    break;
                case AttributeType.Thunder:
                    show[i].color = Color.yellow;
                    break;
                case AttributeType.Earth:
                    show[i].color = Color.green;
                    break;
            }
        }
        for (int i = attributes.Count; i < show.Length; i++)
        {
            show[i].enabled = false;
        }
    }

    public int GetCount()
    {
        return attributes.Count;
    }

    public AttributeType GetFirstAttribute()
    {
        if (attributes.Count > 0)
        {
            return attributes[0];
        }
        return AttributeType.None;
    }
    public bool CheckAttribute(AttributeType delete)
    {
        if (attributes.Contains(delete))
        {
            return true;
        }
        return false;
    }

    public IEnumerator DeleteAttribute(AttributeType value)
    {
        deleting = true;
        attributes.Remove(value);
        UpdateAttribute();
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        deleting = false;
    }

    public IEnumerator AddAttribute(AttributeType value)
    {
        while (deleting)
        {
            yield return null;
        }
        if (attributes.Count > 3)
        {
            attributes.RemoveAt(0);
            attributes.Add(value);
        }
        else
            attributes.Add(value);
        UpdateAttribute();
    }

}
