using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Recorder.OutputPath;

public class RandomItemPopup : MonoBehaviour, Popup
{
    private LootUI[] items = new LootUI[3];
    private Button close;
    private DropPopup drop;

    private void Awake()
    {
        if(!transform.GetChild(1).GetChild(0).TryGetComponent<LootUI>(out items[0])
            || !transform.GetChild(1).GetChild(1).TryGetComponent<LootUI>(out items[1])
            || !transform.GetChild(1).GetChild(2).TryGetComponent<LootUI>(out items[2]))
        {
            Debug.Log("RandomItemPopup - Awake - LootUI");
        }
        if(!transform.GetChild(2).TryGetComponent<Button>(out close))
        {
            Debug.Log("RandomItemPopup - Awake - Button");
        }
        else
        {
            close.onClick.AddListener(CheckClose);
        }
        GameObject d = GameObject.Find("DropPopup");
        if (d == null || !d.TryGetComponent<DropPopup>(out drop))
        {
            Debug.Log("RandomItemPopup - Awake - DropPopup");
        }
    }

    private void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void SetItem(int id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].gameObject.activeSelf)
            {
                items[i].gameObject.SetActive(true);
                items[i].SetLoot(true, id);
                break;
            }
        }
    }

    private void CheckClose()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetEnable())
            {
                drop.gameObject.SetActive(true);
                drop.popup = this;
                drop.SetText(false);
                return;
            }
        }
        Close();
    }

    public void Close()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetEnable())
            {
                items[i].SaleItem();
            }
            items[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
