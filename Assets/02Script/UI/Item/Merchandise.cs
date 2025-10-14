using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Merchandise : MonoBehaviour
{
    private bool isItem;
    public bool IsItem
    {
        get => isItem;
    }

    private Item item;
    public Item GetItem()
    {
        return item;
    }
    private Artifact artifact;
    public Artifact GetArtifact()
    {
        return artifact;
    }

    private Image icon;
    private Button btn;
    private Image type;
    private TextMeshProUGUI amount;

    private ShopPopup shop;

    private void Awake()
    {
        if(!TryGetComponent<Image>(out icon))
        {
            Debug.Log("Merchandise - Awake - Image");
        }
        if(!TryGetComponent<Button>(out btn))
        {
            Debug.Log("Merchandise - Awake - Button");
        }
        Transform t = transform.Find("Type");
        if (t == null || !t.TryGetComponent<Image>(out type))
        {
            Debug.Log("Merchandise - Awake - Image");
        }
        t = transform.Find("Amount");
        if(t == null || !t.TryGetComponent<TextMeshProUGUI>(out amount))
        {
            Debug.Log("Merchandise - Awake - TextMeshProUGUI");
        }
        if(!transform.parent.parent.TryGetComponent<ShopPopup>(out shop))
        {
            Debug.Log("Merchandise - Awake - ShopPopup");
        }
    }

    private void Start()
    {
        btn.onClick.AddListener(() => { shop.SetBuy(this); });
    }

    public void SetMerchandise(Item item)
    {
        isItem = true;
        this.item = item;
        this.artifact = null;
        icon.sprite = Resources.Load<Sprite>("Item/" + item.Data.Icon);
        amount.text = item.Amount.ToString();
        type.sprite = Resources.Load<Sprite>("ItemType/" + item.GetType());
    }

    public void SetMerchandise(Artifact artifact)
    {
        isItem = false;
        this.artifact = artifact;
        this.item = null;
        icon.sprite= Resources.Load<Sprite>("Artifact/" + artifact.Data.Icon);
        amount.text = "";
        type.sprite = Resources.Load<Sprite>("ItemType/Artifact");
    }

    public void SetZero()
    {
        if (amount.text != "")
            amount.text = "0";
        icon.color = Color.gray;
        type.color = Color.gray;
    }

    public void SetAmount(int amount)
    {
        if(amount == 0)
            SetZero();
        else
            this.amount.text = amount.ToString();

        if(isItem)
            item.Amount = amount;
    }

    public int GetAmount()
    {
        if(isItem)
        {
            return Int32.Parse(amount.text);
        }
        else
        {
            if (icon.color == Color.gray)
                return 0;
            else 
                return 1;
        }
    }

    public int GetPayment()
    {
        if (isItem)
            return item.Data.Purchase;
        else
        {
            switch (artifact.Data.Grade)
            {
                case 3:
                    return 100;
                case 2:
                    return 80;
                default:
                    return 60;
            }
        }
    }

    public int GetID()
    {
        if (isItem)
            return item.GetID(true);
        else
            return artifact.GetID(true);
    }
}
