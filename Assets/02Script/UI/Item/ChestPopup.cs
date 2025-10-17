using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestPopup : MonoBehaviour
{
    private ExplorationManager manager;
    private ChestAnimationController anim;
    private Image flash;

    private Entity_Item item;
    private Entity_Artifact artifact;
    private GameObject popup;
    private Image icon;
    private TextMeshProUGUI amount;
    private Image type;
    private bool isArtifact;
    private Button getButton;
    private Button throwButton;

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<ChestAnimationController>(out anim))
        {
            Debug.Log("ChestPopup - Awake - ChestAnimationController");
        }
        if(!transform.GetChild(1).TryGetComponent<Image>(out flash))
        {
            Debug.Log("ChestPopup - Awake - Image");
        }

        popup = GameObject.Find("ChestItemPopup");
        if(popup == null || !popup.transform.GetChild(0).TryGetComponent<Image>(out icon))
        {
            Debug.Log("ChestPopup - Awake - Image");
        }
        if(popup == null || !popup.transform.GetChild(1).TryGetComponent<TextMeshProUGUI>(out amount))
        {
            Debug.Log("ChestPopup - Awake - TextMeshProUGUI");
        }

        GameObject obj = GameObject.Find("ChestItemTypeIcon");
        if(obj == null || !obj.TryGetComponent<Image>(out type))
        {
            Debug.Log("ChestPopup - Awake - Image");
        }

        obj = GameObject.Find("GetChestItem");
        if(obj == null || !obj.TryGetComponent<Button>(out getButton))
        {
            Debug.Log("ChestPopup - Awake - Button");
        }
        obj = GameObject.Find("ThrowChestItem");
        if (obj == null || !obj.TryGetComponent<Button>(out throwButton))
        {
            Debug.Log("ChestPopup - Awake - Button");
        }


        getButton.onClick.AddListener(GetItem);
        throwButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        flash.enabled = false;
        popup.SetActive(false);
        anim.gameObject.SetActive(false);
        gameObject.SetActive(false);
        manager = (ExplorationManager)GameManager.Inst.manager;
    }

    public void SetPopup(ChestType chest)
    {
        popup.SetActive(true);
        if (chest == ChestType.Golden)
        {
            int result = UnityEngine.Random.Range(0, 100);
            if (result < 70)
            {
                GameManager.Inst.GetArtifactData(GameManager.Inst.GetRandomMaterialOrItemArtifact(), out artifact);
                icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Icon);
                type.sprite = Resources.Load<Sprite>("ItemType/Artifact");
                isArtifact = true;
                amount.text = "";
            }
            else
            {
                GameManager.Inst.GetReinforcementJewelItem(out item);
                icon.sprite = Resources.Load<Sprite>("Item/" + item.Icon);
                type.sprite = Resources.Load<Sprite>("ItemType/" + item.Type);
                isArtifact = false;
                amount.text = "";
            }
        }
        else if(chest == ChestType.Silver)
        {
            int result = UnityEngine.Random.Range(0, 100);
            if (result < 10)
            {
                GameManager.Inst.GetArtifactData(GameManager.Inst.GetRandomMaterialOrItemArtifact(), out artifact);
                icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Icon);
                type.sprite = Resources.Load<Sprite>("ItemType/Artifact");
                isArtifact = true;
                amount.text = "";
            }
            else 
            {
                if (result < 40)
                {
                    GameManager.Inst.GetConsumableItem(out item);
                    amount.text = "";
                }
                else if (result < 70)
                {
                    GameManager.Inst.GetExchangeItem(out item);
                    amount.text = "";
                }
                else
                {
                    result = UnityEngine.Random.Range(0, 100);
                    if(result < 70)
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
                }
                icon.sprite = Resources.Load<Sprite>("Item/" + item.Icon);
                type.sprite = Resources.Load<Sprite>("ItemType/" + item.Type);
                isArtifact = false;
            }
        }
        else
        {
            int result = UnityEngine.Random.Range(0, 100);
            if(result < 70)
                GameManager.Inst.GetConsumableItem(out item);
            else
                GameManager.Inst.GetExchangeItem(out item);
            icon.sprite = Resources.Load<Sprite>("Item/" + item.Icon);
            type.sprite = Resources.Load<Sprite>("ItemType/" + item.Type);
            isArtifact = false;
            amount.text = "";
        }

        popup.SetActive(false);
        anim.gameObject.SetActive(true);
        switch (chest)
        {
            case ChestType.Golden:
                anim.SetType(0);
                break;
            case ChestType.Silver:
                anim.SetType(1);
                break;
            case ChestType.Wooden:
                anim.SetType(2);
                break;
        }
        StartCoroutine(ShowItem());
    }

    public void SetPopupItem(int amount)
    {
        this.amount.text = amount.ToString();
    }

    private IEnumerator ShowItem()
    {
        yield return null; 
        anim.SetType(-1);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        flash.enabled = true;
        if (artifact != null)
        {
            switch (artifact.Grade)
            {
                case 3:
                    flash.color = new Color(255, 215, 0);
                    break;
                case 2:
                    flash.color = new Color(128, 0, 128);
                    break;
                case 1:
                    flash.color = new Color(108, 160, 220);
                    break;
            }
        }
        else
            flash.color = Color.white;
        popup.SetActive(true);
    }

    private void GetItem()
    {
        if (isArtifact)
        {
            if (manager.AddArtifact(artifact.ID))
            {
                Close();
            }
            else
            {
                manager.SetFull(false);
            }

        }
        else
        {
            int value = 1;
            if (amount.text != "")
                value = Int32.Parse(amount.text);
            Item newItem = new Item(item.ID, value);
            int remain = 0;
            if (manager.AddItem(newItem, out remain))
            {
                Close();
            }
            else
            {
                amount.text = remain.ToString();    
                manager.SetFull(true);
            }

        }
    }


    private void Close()
    {
        artifact = null;
        item = null;
        flash.enabled = false;
        anim.gameObject.SetActive(false);
        manager.EndEvent();
        gameObject.SetActive(false);
    }
}
