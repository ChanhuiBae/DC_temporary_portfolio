using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactUI : MonoBehaviour
{
    private Image bubble;
    private Image icon;
    private int id;
    public int ID
    {
        get => id;
    }

    private EventTrigger trigger;
    private EventTrigger.Entry enter;
    private EventTrigger.Entry exit;

    private ExplorationManager manager;

    private void Awake()
    {
        if(!TryGetComponent<Image>(out bubble))
        {
            Debug.Log("ArtifactUI - Awake - Image");
        }
        if(!transform.GetChild(0).TryGetComponent<Image>(out icon))
        {
            Debug.Log("ArtifactUI - Awake - Image");
        }
        id = 0;

        if (!transform.TryGetComponent<EventTrigger>(out trigger))
        {
            Debug.Log("Bubble - Awake - EventTrigger");
        }
        else
        {
            enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener((data) => { OnEnter((PointerEventData)data); });
            trigger.triggers.Add(enter);

            exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnExit((PointerEventData)data); });
            trigger.triggers.Add(exit);
        }
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
    }

    public void SetBubble(int mergre)
    {
        bubble.enabled = true;
        switch (mergre)
        {
            case 0:
                bubble.enabled = false;
                break;
            case 1:
                bubble.sprite = Resources.Load<Sprite>("Bubble/Bubble1");
                break;
            case 2:
                bubble.sprite = Resources.Load<Sprite>("Bubble/Bubble2");
                break;
            case 3:
                bubble.sprite = Resources.Load<Sprite>("Bubble/Bubble3");
                break;
        }
    }

    public void SetArtifact(int id)
    {
        if(id == 0)
        {
            bubble.enabled = false;
            icon.enabled = false;
        }
        else
        {
            this.id = id;
            bubble.enabled = false;
            icon.enabled = true;
            Entity_Artifact artifact;
            GameManager.Inst.GetArtifactData(id, out artifact);
            icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Icon);
        }
    }

    private void OnEnter(PointerEventData data)
    {
        manager.ShowExplanArtifact(transform.position, id);
    }

    private void OnExit(PointerEventData data)
    {
        manager.CloseExplan();
    }
}
