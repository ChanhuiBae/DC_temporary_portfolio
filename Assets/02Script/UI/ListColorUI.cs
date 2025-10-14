using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ListColorUI : MonoBehaviour
{
    List<Image> list;

    private void Awake()
    {
        list = new List<Image>();
        for (int i = 0; i < transform.childCount; i++)
        {
            list.Add(transform.GetChild(i).GetComponent<Image>());
        }
    }

    public void SetStars(int value)
    {
        if (value <= list.Count)
        {
            for (int i = 0; i < value; i++)
            {
                list[i].color = Color.yellow;
            }
            for (int i = value; i < list.Count; i++)
            {
                list[i].color = Color.white;
            }
        }
    }

    public void SetPosition(bool[] set)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(set[i])
            {
                list[i].color = Color.red;
            }
            else
            {
                list[i].color = Color.white;
            }
        }
    }
}
