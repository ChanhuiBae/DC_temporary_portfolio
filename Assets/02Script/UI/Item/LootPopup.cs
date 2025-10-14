using UnityEngine;
using UnityEngine.UI;

public class LootPopup : MonoBehaviour, Popup
{
    private ExplorationManager manager;
    private Image[] flash = new Image[5];
    private LootUI[] loot = new LootUI[5];
    private Button close;
    private DropPopup drop;
    private void Awake()
    {
        Transform t = transform.Find("Close");
        if(t == null || !t.TryGetComponent<Button>(out close))
        {
            Debug.Log("LootPopup - Awake - Button");
        }
        t = transform.Find("FlashParent");
        Transform l = transform.Find("Loot");
        for (int i = 0; i < flash.Length; i++)
        {
            if(!t.GetChild(i).TryGetComponent<Image>(out flash[i]))
            {
                Debug.Log("LootPopup - Awake - Image");
            }
            if(!l.GetChild(i).TryGetComponent<LootUI>(out loot[i]))
            {
                Debug.Log("LootPopup - Awake - LootUI");
            }
        }
        GameObject d = GameObject.Find("DropPopup");
        if(d == null || !d.TryGetComponent<DropPopup>(out drop))
        {
            Debug.Log("LootPopup - Awake - DropPopup");
        }

        for (int i = 0; i < flash.Length; i++)
        {
            flash[i].gameObject.SetActive(false);
            loot[i].gameObject.SetActive(false);
        }
        close.onClick.AddListener(CheckClose);
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
        gameObject.SetActive(false);
    }

    public void SetItem(int id,  int value)
    {
        for (int i = 0; i < loot.Length; i++) 
        {
            if (!loot[i].gameObject.activeSelf)
            {
                loot[i].gameObject.SetActive(true);
                loot[i].SetLoot(true, id, value);
                flash[i].gameObject.SetActive(true);
                flash[i].enabled = false;
                break;
            }
        }
    }

    public void SetArtifact(int id)
    {
        for (int i = 0; i < loot.Length; i++)
        {
            if (!loot[i].gameObject.activeSelf)
            {
                loot[i].gameObject.SetActive(true);
                loot[i].SetLoot(false, id);
                Entity_Artifact artifact;
                GameManager.Inst.GetArtifactData(id, out artifact);
                flash[i].gameObject.SetActive(true);
                flash[i].enabled = true;
                switch (artifact.Grade)
                {
                    case 3:
                        flash[i].color = new Color(255, 215, 0);
                        break;
                    case 2:
                        flash[i].color = new Color(128, 0, 128);
                        break;
                    case 1:
                        flash[i].color = new Color(108, 160, 220);
                        break;
                }
                break;
            }
        }
    }

    private void CheckClose()
    {
        for (int i = 0; i < loot.Length; i++)
        {
            if (loot[i].GetEnable())
            {
                drop.gameObject.SetActive(true);
                drop.popup = this;
                drop.SetText(true);
                return;
            }
        }
        Close();
    }

    public void Close()
    {
        for (int i = 0; i < loot.Length; i++)
        {
            loot[i].gameObject.SetActive(false);
            flash[i].gameObject.SetActive(false);
        }
        manager.StartNextVictoryLogic();
        gameObject.SetActive(false);
    }
}
