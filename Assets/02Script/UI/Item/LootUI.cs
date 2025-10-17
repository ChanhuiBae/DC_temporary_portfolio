using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootUI : MonoBehaviour
{
    private Image icon;
    private Image type;
    private TextMeshProUGUI value;
    private Button button;
    private Entity_Item item;
    private Entity_Artifact artifact;
    private ExplorationManager manager;

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<Image>(out icon))
        {
            Debug.Log("LootUI - Awake - Image");
        }
        if(transform.childCount < 2 || !transform.GetChild(1).TryGetComponent<TextMeshProUGUI>(out value))
        {
            Debug.Log("LootUI - Awake - TextMeshProUGUi");
        }
        if(!icon.transform.GetChild(1).TryGetComponent<Image>(out type))
        {
            Debug.Log("LootUI - Awake - Image");
        }
        if(!transform.TryGetComponent<Button>(out button))
        {
            Debug.Log("LootUI - Awake - Button");
        }
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
    }


    public void SetLoot(bool isItem, int id, int value = 1)
    {
        SetEnable(true);
        if (isItem)
        {
            GameManager.Inst.GetItemData(id,out item);
            icon.sprite = Resources.Load<Sprite>("Item/" + item.Icon);
            type.sprite = Resources.Load<Sprite>("ItemType/" + item.Type);
            if(this.value != null)
            {
                if (value > 1)
                {
                    this.value.enabled = true;
                    this.value.text = value.ToString();
                }
                else
                {
                    this.value.text = "1";
                    this.value.enabled = false;
                }
            }
            button.onClick.AddListener(GetItem);
            
        }
        else
        {
            GameManager.Inst.GetArtifactData(id, out artifact);
            icon.sprite = Resources.Load<Sprite>("Artifact/" +  artifact.Icon);
            type.sprite = Resources.Load<Sprite>("ItemType/Artifact");
            if(this.value != null)
            {
                this.value.text = "1";
                this.value.enabled = false;
            }
            button.onClick.AddListener(GetArtifact);
        }
    }

    private void GetItem()
    {
        int amount = 1;
        if(value != null && value.text.Length > 0)
            amount = Int32.Parse(value.text);
        Item newItem = new Item(item.ID, amount);
        int remain = 0;
        if (manager.AddItem(newItem,out remain))
        {
            SetEnable(false);
        }
        else
        {
            value.text = remain.ToString();
            manager.SetFull(true);
        }
    }

    private void GetArtifact()
    {
        if (GameManager.Inst.Exploration.CheckAddArtifact())
        {
            manager.AddArtifact(artifact.ID);
            SetEnable(false);
        }
        else
        {
            manager.SetFull(false);
        }
    }

    public void SaleItem()
    {
        manager.SaleItem(item.ID, 1);
    }

    private void SetEnable(bool value)
    {
        if (value)
        {
            icon.color = Color.white;
            type.color = Color.white;
        }
        else
        {
            button.onClick.RemoveAllListeners();
            icon.color = Color.gray;
            type.color = Color.gray;
        }
    }

    public bool GetEnable()
    {
        if (!gameObject.activeSelf)
            return false;
        else if (icon.color == Color.gray)
            return false;
        return true;
    }
}
