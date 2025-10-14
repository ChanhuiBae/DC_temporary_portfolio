using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    private ExplorationManager manager;

    private Image icon;
    private TextMeshProUGUI amount;
    private Item item;

    private EventTrigger trigger;
    private EventTrigger.Entry up;
    private EventTrigger.Entry enter;
    private EventTrigger.Entry exit;
    private EventTrigger.Entry begin;
    private EventTrigger.Entry drag;
    private EventTrigger.Entry drop;

    private Vector3 startPosition;
    private ShopPopup shop;
    private bool sale;

    private void Awake()
    {
        if(!transform.TryGetComponent<Image>(out icon))
        {
            Debug.Log("ItemButton - Awake - Image");
        }
        if (!transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out amount))
        {
            Debug.Log("ItemButton - Awake - TextMeshProUGUI");
        }
        if (!transform.TryGetComponent<EventTrigger>(out trigger))
        {
            Debug.Log("ItemButton - Awake - EventTrigger");
        }

        up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener((data) => { OnUp((PointerEventData)data); });
        trigger.triggers.Add(up);

        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) => { OnEnter((PointerEventData)data); });
        trigger.triggers.Add(enter);

        exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) => { OnExit((PointerEventData)data); });
        trigger.triggers.Add(exit);

        begin = new EventTrigger.Entry();
        begin.eventID = EventTriggerType.BeginDrag;
        begin.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
        trigger.triggers.Add(begin);

        drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(drag);

        drop = new EventTrigger.Entry();
        drop.eventID = EventTriggerType.Drop;
        drop.callback.AddListener((data) => { OnDrop((PointerEventData)data); });
        trigger.triggers.Add(drop);

        GameObject obj = GameObject.Find("Shop");
        if(obj == null || !obj.TryGetComponent<ShopPopup>(out shop))
        {
            Debug.Log("ItemButton - Awake - ShopPopup");
        }
        startPosition = transform.position;
        sale = false;
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
    }
    public void SetLock()
    {
        icon.enabled = true;
        icon.sprite = Resources.Load<Sprite>("Item/Lock"); 
        amount.text = "";
        trigger.enabled = false;

    }

    public void SetEmpty()
    {
        icon.enabled = false;
        amount.text = "";
        trigger.enabled = false;
    }

    public void SetButton(Item item)
    {
        this.item = item;
        icon.enabled = true;
        icon.sprite = Resources.Load<Sprite>("Item/" + item.Data.Icon);
        amount.text = item.Amount.ToString();
        trigger.enabled = true;
    }

    private void UseItem()
    {
        if (GameManager.Inst.itemManager.UseItem(item.GetID(true)))
        {
            this.item = GameManager.Inst.Exploration.player.invenory.GetItem(this.item.GetID(false));
            if(item == null)
            {
                SetEmpty();
            }
            else
            {
                amount.text = item.Amount.ToString();
            }
        }
    }

    private void OnUp(PointerEventData eventData)
    {
        if(Input.mousePosition.x > startPosition.x - 30 && Input.mousePosition.x < startPosition.x + 30
            && Input.mousePosition.y > startPosition.y - 30 && Input.mousePosition.y < startPosition.y + 30)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                UseItem();
                StopExplan();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                GameManager.Inst.Exploration.player.invenory.DeleteItemAmount(item.GetID(true), 1);
                int value = Int32.Parse(amount.text) - 1;
                if (value <= 0)
                {
                    SetEmpty();
                }
                else
                {
                    amount.text = value.ToString();
                }
                StopExplan();
            }
        }
    }

    private void OnEnter(PointerEventData eventData)
    {
        StartCoroutine(WaitAndOpenPopup());
    }

    private IEnumerator WaitAndOpenPopup()
    {
        yield return YieldInstructionCache.WaitForSeconds(1);
        manager.ShowExplanItem(transform.position, item.GetID(true));
    }

    private void OnExit(PointerEventData eventData)
    {
        StopExplan();
    }

    private void StopExplan()
    {
        StopAllCoroutines();
        manager.CloseExplan();
    }

    private void OnBeginDrag(PointerEventData data)
    {
        if(shop.gameObject.activeSelf)
            amount.enabled = false;
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (shop.gameObject.activeSelf)
            transform.position = Input.mousePosition;
    }

    private void OnDrop(PointerEventData eventData)
    {
        if (sale)
        {
            StopAllCoroutines();
            manager.CloseExplan();
            shop.SaleItem(item);
        }
        amount.enabled = true;
        transform.position = startPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shop")
        {
            sale = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Shop")
        {
            sale = false;
        }
    }
}
