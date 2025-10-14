using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SalePopup : MonoBehaviour
{
    private Image icon;
    private TextMeshProUGUI price;
    private TextMeshProUGUI amount;
    private Button sale;
    private Button min;
    private Button minus;
    private Button plus;
    private Button max;
    private Button close;

    private int id;
    private int saleAmount;
    private int maxAmount;
    private int onePrice;

    private ExplorationManager manager;

    private void Awake()
    {
        Transform obj = transform.Find("SaleIcon");
        if (obj == null || !obj.TryGetComponent<Image>(out icon))
        {
            Debug.Log("SalePopup - Awake - Image");
        }
        obj = transform.Find("Price");
        if (obj == null || !obj.TryGetComponent<TextMeshProUGUI>(out price))
        {
            Debug.Log("SalePopup - Awake - TextMeshProUGUI");
        }
        obj = transform.Find("Amount");
        if (obj == null || !obj.TryGetComponent<TextMeshProUGUI>(out amount))
        {
            Debug.Log("SalePopup - Awake - TextMeshProUGUI");
        }
        obj = transform.Find("MinLeft");
        if (obj == null || !obj.TryGetComponent<Button>(out min))
        {
            Debug.Log("SalePopup - Awake - Button");
        }
        obj = transform.Find("Left");
        if (obj == null || !obj.TryGetComponent<Button>(out minus))
        {
            Debug.Log("SalePopup - Awake - Button");
        }
        obj = transform.Find("Right");
        if (obj == null || !obj.TryGetComponent<Button>(out plus))
        {
            Debug.Log("SalePopup - Awake - Button");
        }
        obj = transform.Find("MaxRight");
        if (obj == null || !obj.TryGetComponent<Button>(out max))
        {
            Debug.Log("SalePopup - Awake - Button");
        }
        obj = transform.Find("Sale");
        if (obj == null || !obj.TryGetComponent<Button>(out sale))
        {
            Debug.Log("SalePopup - Awake - Button");
        }
        obj = transform.Find("Close");
        if (obj == null || !obj.TryGetComponent<Button>(out close))
        {
            Debug.Log("SalePopup - Awake - Button");
        }
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
        min.onClick.AddListener(SetMin);
        max.onClick.AddListener(SetMax);
        plus.onClick.AddListener(SetPlus);
        minus.onClick.AddListener(SetMinus);
        sale.onClick.AddListener(Sale);
        close.onClick.AddListener(Close);
        gameObject.SetActive(false);
    }

    public void SetSale(Item item)
    {
        id = item.GetID(true);
        icon.sprite = Resources.Load<Sprite>("Item/" + item.Data.Icon);
        maxAmount = item.Amount;
        onePrice = item.Data.Sale;
        SetMin();
    }

    private void Sale()
    {
        if(manager.SaleItem(id, saleAmount))
        {
            manager.SetPearl(saleAmount * onePrice);
        }
        gameObject.SetActive(false);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void SetMin()
    {
        if (maxAmount > 0)
        {
            saleAmount = 1;
            amount.text = saleAmount.ToString();
            price.text = (saleAmount * onePrice).ToString();
        }
    }
    private void SetMax()
    {
        if (maxAmount > 0)
        {
            saleAmount = maxAmount;
            amount.text = saleAmount.ToString();
            price.text = (saleAmount * onePrice).ToString();
        }
    }

    private void SetPlus()
    {
        if (saleAmount > 0 && saleAmount < maxAmount)
        {
            saleAmount += 1;
            amount.text = saleAmount.ToString();
            price.text = (saleAmount * onePrice).ToString();
        }
    }

    private void SetMinus()
    {
        if (saleAmount > 1)
        {
            saleAmount -= 1;
            amount.text = saleAmount.ToString();
            price.text = (saleAmount * onePrice).ToString();
        }
    }
}
