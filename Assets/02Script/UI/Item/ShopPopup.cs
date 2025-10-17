using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopPopup : MonoBehaviour
{
    private Button close;

    private Button buy;
    private Button min;
    private Button minus;
    private Button plus;
    private Button max;
    private TextMeshProUGUI amount;
    private TextMeshProUGUI payment;
    private TextMeshProUGUI name;
    private TextMeshProUGUI description;
    private Image icon;
    private Image check;
    private TextMeshProUGUI instance;
    private List<Merchandise> merchandises;
    private int current;
    private int buyAmount;
    private int maxAmount;
    private int onePayment;

    private ExplorationManager manager;
    private SalePopup sale;
    private Image randomItem;
    private RandomItemPopup randomPopup;

    private void Awake()
    {
        Transform c = transform.Find("Close");
        if (c == null || !c.TryGetComponent<Button>(out close))
        {
            Debug.Log("ShopPopup - Awake - Button");
        }
        else
        {
            close.onClick.AddListener(CloseShop);
        }
        Transform b = transform.Find("Back");
        Transform back = b.Find("Buy");
        if (back == null || !back.TryGetComponent<Button>(out buy))
        {
            Debug.Log("ShopPopup - Awake - Button");
        }
        back = b.Find("MinLeft");
        if (back == null || !back.TryGetComponent<Button>(out min))
        {
            Debug.Log("ShopPopup - Awake - Button");
        }
        back = b.Find("Left");
        if (back == null || !back.TryGetComponent<Button>(out minus))
        {
            Debug.Log("ShopPopup - Awake - Button");
        }
        back = b.Find("Right");
        if (back == null || !back.TryGetComponent<Button>(out plus))
        {
            Debug.Log("ShopPopup - Awake - Button");
        }
        back = b.Find("MaxRight");
        if (back == null || !back.TryGetComponent<Button>(out max))
        {
            Debug.Log("ShopPopup - Awake - Button");
        }
        back = b.Find("Amount");
        if (back == null || !back.TryGetComponent(out amount))
        {
            Debug.Log("ShopPopup - Awake - TextMeshProUGUI");
        }
        back = b.Find("Payment");
        if (back == null || !back.TryGetComponent(out payment))
        {
            Debug.Log("ShopPopup - Awake - TextMeshProUGUI");
        }
        back = b.Find("Name");
        if (back == null || !back.TryGetComponent(out name))
        {
            Debug.Log("ShopPopup - Awake - TextMeshProUGUI");
        }
        back = b.Find("Description");
        if (back == null || !back.TryGetComponent(out description))
        {
            Debug.Log("ShopPopup - Awake - TextMeshProUGUI");
        }
        back = b.Find("BuyIcon");
        if (back == null || !back.TryGetComponent(out icon))
        {
            Debug.Log("ShopPopup - Awake - Image");
        }
        GameObject cb = GameObject.Find("CheckBack");
        if (cb == null || !cb.TryGetComponent(out check))
        {
            Debug.Log("ShopPopup - Awake - Image");
        }
        GameObject ins = GameObject.Find("Instance");
        if (ins == null || !ins.transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out instance))
        {
            Debug.Log("ShopPopup - Awake - TextMeshProUGUI");
        }

        merchandises = new List<Merchandise>();
        Transform items = transform.Find("Items");
        if (items != null)
        {
            for (int i = 0; i < items.childCount; i++)
            {
                merchandises.Add(items.GetChild(i).GetComponent<Merchandise>());
            }
        }

        Transform sp = transform.Find("SalePopup");
        if(sp == null || !sp.TryGetComponent<SalePopup>(out sale))
        {
            Debug.Log("ShopPoup - Awake - SalePopup");
        }
        Transform t = transform.Find("RandomItem");
        if (t == null || !t.TryGetComponent<Image>(out randomItem))
        {
            Debug.Log("LootPopup - Awake - Image");
        }
        t = transform.Find("RandomPopup");
        if (t == null || !t.TryGetComponent<RandomItemPopup>(out randomPopup))
        {
            Debug.Log("LootPopup - Awake - RandomItemPopup");
        }
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
        instance.transform.parent.gameObject.SetActive(false);
        min.onClick.AddListener(SetMin);
        max.onClick.AddListener(SetMax);
        plus.onClick.AddListener(SetPlus);
        minus.onClick.AddListener(SetMinus);
        buy.onClick.AddListener(Buy);
        SetShop();
        randomItem.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void SetShop()
    {
        merchandises[0].SetMerchandise(new Item(1, 5));
        merchandises[1].SetMerchandise(new Item(2, 5));
        merchandises[2].SetMerchandise(new Item(3, 5));
        merchandises[3].SetMerchandise(new Item(4, 3));
        merchandises[4].SetMerchandise(new Item(5, 3));
        merchandises[5].SetMerchandise(new Item(6, 3));
        int id = Random.Range(1, 6);
        Entity_Item item;
        GameManager.Inst.GetItemData(id, out item);
        merchandises[6].SetMerchandise(new Item(id, item.MaxCount));
        merchandises[7].SetMerchandise(new Artifact(GameManager.Inst.GetRandomMaterialOrItemArtifact()));
        merchandises[8].SetMerchandise(new Artifact(GameManager.Inst.GetRandomMaterialOrItemArtifact()));
        merchandises[9].SetMerchandise(new Item(8, 3));

        SetNone();
    }
   

    public void SetBuy(Merchandise merchandise)
    {
        if(merchandise.GetAmount() == 0)
            return;

        current = merchandises.IndexOf(merchandise);
        check.enabled = true;
        check.transform.position = merchandise.transform.position;
        SetButton(true);
        if (merchandise.IsItem)
            SetBuyItem(merchandise.GetItem());
        else
            SetBuyArtifact(merchandise.GetArtifact());
        maxAmount = merchandise.GetAmount();
        SetMin();
    }

    private void SetBuyItem(Item item)
    {
        icon.enabled = true;
        icon.sprite = Resources.Load<Sprite>("Item/" + item.Data.Icon);
        name.text = item.Data.Name;
        description.text = item.Data.Explanation;
        onePayment = item.Data.Purchase;
    }

    private void SetBuyArtifact(Artifact artifact)
    {
        icon.enabled = true;
        icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Data.Icon);
        name.text = artifact.Data.Name;
        description.text = artifact.Data.Explanation;
        if (artifact.Data.Grade == 1)
            onePayment = 200;
        else
            onePayment = 300;
    }

    private void CloseShop()
    {
        manager.SpeakMerchant();
        gameObject.SetActive(false);
    }

    private void SetMin()
    {
        if (maxAmount > 0)
        {
            buyAmount = 1;
            amount.text = buyAmount.ToString();
            payment.text = onePayment.ToString();
        }
    }
    private void SetMax()
    {
        if (maxAmount > 0)
        {
            buyAmount = maxAmount;
            amount.text = buyAmount.ToString();
            payment.text = (buyAmount * onePayment).ToString();
        }
    }

    private void SetPlus()
    {
        if (buyAmount > 0 && buyAmount < maxAmount)
        {
            buyAmount += 1;
            amount.text = buyAmount.ToString();
            payment.text = (buyAmount * onePayment).ToString();
        }
    }

    private void SetMinus()
    {
        if (buyAmount > 1)
        {
            buyAmount -= 1;
            amount.text = buyAmount.ToString();
            payment.text = (buyAmount * onePayment).ToString();
        }
    }

    private void Buy()
    {
        if(buyAmount * onePayment <= GameManager.Inst.Exploration.player.Pearl)
        {
            if (merchandises[current].IsItem) // Item Case
            {
                if (merchandises[current].GetID() == 8)
                {
                    StartCoroutine(ShowRandomItem()); 
                    maxAmount -= buyAmount;
                    merchandises[current].SetAmount(maxAmount);
                    if (maxAmount == 0)
                        SetNone();
                }
                else if (GameManager.Inst.Exploration.player.invenory.CheckAddItem(merchandises[current].GetID(), buyAmount))
                {
                    Item newItem = new Item(merchandises[current].GetID(), buyAmount);
                    int remain = 0;
                    manager.AddItem(newItem, out remain);
                    manager.SetPearl(-buyAmount * onePayment);
                    maxAmount -= buyAmount;
                    merchandises[current].SetAmount(maxAmount);
                    if (maxAmount == 0)
                        SetNone();
                    else
                        SetBuy(merchandises[current]);
                }
                else
                {
                    StartCoroutine(ShowInstance(1));
                }
            }
            else // Artifact Case
            {
                if (GameManager.Inst.Exploration.CheckAddArtifact())
                {
                    manager.AddArtifact(merchandises[current].GetID());
                    manager.SetPearl(-buyAmount * onePayment);
                    maxAmount -= buyAmount;
                    merchandises[current].SetAmount(maxAmount);
                    SetBuy(merchandises[current]);
                }
                else
                {
                    StartCoroutine(ShowInstance(2));
                }
            }
        }
        else
        {
            StartCoroutine(ShowInstance(0));
        }
        
    }

    private void SetNone()
    {
        check.enabled = false;
        icon.enabled = false;
        name.text = "";
        amount.text = "";
        payment.text = "";
        description.text = "";
        SetButton(false);
    }

    private void SetButton(bool enabled)
    {
        min.enabled = enabled;
        max.enabled = enabled;
        plus.enabled = enabled;
        minus.enabled = enabled;
        buy.enabled = enabled;
    }

    private IEnumerator ShowInstance(int type)
    {
        instance.transform.parent.gameObject.SetActive(true);
        if (type == 1)
            instance.text = "가방 공간이 부족합니다.";
        else if (type == 0) 
            instance.text = "보유 중인 펄의 양이 부족합니다.";
        else
            instance.text = "아티팩트 최대 보유량에 도달했습니다.";
        yield return YieldInstructionCache.WaitForSeconds(2f);
        instance.transform.parent.gameObject.SetActive(false);
    }

    public void SaleItem(Item item)
    {
        if (item.Amount > 1)
        {
            sale.gameObject.SetActive(true);
            sale.SetSale(item);
        }
        else if (manager.SaleItem(item.GetID(true), 1))
        {
            manager.SetPearl(item.Data.Sale);
        }
    }

    public void SaleArtifact(Entity_Artifact artifact)
    {
        GameManager.Inst.GetArtifactData(artifact.ID, out artifact);
        switch (artifact.Grade)
        {
            case 3:
                manager.SetPearl(200);
                break;
            case 2:
                manager.SetPearl(150);
                break;
            default:
                manager.SetPearl(100);
                break;
        }
    }

    private IEnumerator ShowRandomItem()
    {
        randomItem.gameObject.SetActive(true);
        randomItem.transform.LeanScale(Vector3.one, 0);
        randomItem.transform.LeanScale(Vector3.zero, 1);
        randomItem.transform.LeanRotateZ(1080, 1);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        randomItem.gameObject.SetActive(false);

        randomPopup.gameObject.SetActive(true);
        randomPopup.transform.LeanScale(Vector3.zero, 0);
        for (int i = 0; i < buyAmount; i++)
        {
            Entity_Item item; 
            int result = UnityEngine.Random.Range(0, 100);
            if (result < 70)
            {
                GameManager.Inst.GetItemData(9, out item);
                amount.text = GameManager.Inst.GetRuneAmount().ToString();
                if (amount.text == "1")
                    amount.text = "";
            }
            else
            {
                GameManager.Inst.GetReinforcementJewelItem(out item);
                amount.text = "";
            }
            randomPopup.SetItem(item.ID);
        }
        randomPopup.transform.LeanScale(Vector3.one, 0.2f);
        SetMin();
    }

}
