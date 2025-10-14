using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBall : MonoBehaviour
{
    private SkillBallManager manager;
    public int ID
    {
        get => GameManager.Inst.Exploration.skills[type];
    }
    private int type; // index
    public int Type
    {
        get => type;
    }

    private Image bubble;
    private Image icon;
    private Image light;
    public bool Light
    {
        get => light.enabled;
    }

    private EventTrigger trigger;
    private EventTrigger.Entry down;
    private EventTrigger.Entry enter;
    private EventTrigger.Entry exit;
    private EventTrigger.Entry up;

    private GameObject effect;

    private void Awake()
    {
        if(!transform.parent.TryGetComponent<SkillBallManager>(out manager))
        {
            Debug.Log("SkillBall - Awake - SkillBallManager");
        }
        if (transform.TryGetComponent<EventTrigger>(out trigger))
        {
            down = new EventTrigger.Entry();
            down.eventID = EventTriggerType.PointerDown;
            down.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
            trigger.triggers.Add(down);

            enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
            trigger.triggers.Add(enter);

            exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
            trigger.triggers.Add(exit);

            up = new EventTrigger.Entry();
            up.eventID = EventTriggerType.PointerUp;
            up.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
            trigger.triggers.Add(up);

        }
        else
        {
            Debug.Log("SkillBall - Awake - EventTrigger");
        }

        bubble = transform.GetComponent<Image>();
        if(transform.GetChild(0).TryGetComponent<Image>(out icon))
        {
            icon.enabled = false;
        }
        if (transform.GetChild(1).TryGetComponent<Image>(out light))
        {
            light.enabled = false;
        }
        else
        {
            Debug.Log("SkillBall - Awake - Image");
        }

        effect = transform.GetChild(2).gameObject;
        effect.SetActive(false);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        effect.SetActive(false);
        do
        {
            type = Random.Range(1, 6);
        }
        while (!manager.AddSkillType(type, this));
        int id;
        if (GameManager.Inst.Exploration.skills.TryGetValue(type, out id))
        {
            icon.enabled = true;
            Entity_Skill skill;
            GameManager.Inst.GetSkillData(id, out skill);
            if(id > 5)
            {
                icon.enabled = true;
                icon.sprite = Resources.Load<Sprite>("Skill/" + skill.Icon);
            }
            else
                icon.enabled= false;
            bubble.sprite = Resources.Load<Sprite>("Bubble/" + skill.Bubble);
        }
        light.enabled = false;
    }

    public void Respawn()
    {
        transform.position = transform.parent.position;
        bubble.rectTransform.sizeDelta = new Vector2(90, 90);
        transform.position += new Vector3(Random.Range(-5, 5), 0, 0);

        Init();
    }

    public void TurnOffLight()
    {
        light.enabled = false;
    }

    private void OnPointerDown(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
            return;
        if (manager.Deleting)
            return;
        if (manager.PlayType == PlayType.OneClick && manager.StartType == 0)
        {
            manager.StartType = type;
            manager.PreType = type;
            light.enabled = true;
            bubble.rectTransform.sizeDelta = new Vector2(100, 100);
            manager.AddConnected(this);
        }
    }

    private void OnPointerEnter(PointerEventData data)
    {
        if (manager.Deleting)
            return;
        if (manager.StartType == type)
        {
            if (manager.PreType == type)
            {
                if (light.enabled)
                {
                    Vector2 point = Camera.main.ScreenToWorldPoint(data.position);
                    if (point.x > transform.position.x - 2 && point.x < transform.position.x + 2
                        && point.y > transform.position.y - 2 && point.y < transform.position.y + 2)
                    {
                        bubble.rectTransform.sizeDelta = new Vector2(100, 100);
                        manager.PopSkillBalls(this);
                    }
                }
                else
                {
                    Vector3 pre = manager.GetPreSkillPosition();
                    if (pre != Vector3.zero)
                    {
                        if (Vector3.Distance(transform.position, pre) < manager.MaxDis)
                        {
                            light.enabled = true;
                            bubble.rectTransform.sizeDelta = new Vector2(100, 100);
                            manager.AddConnected(this);
                        }
                    }
                }
            }
            manager.PreType = type;
        }
    }

    private void OnPointerExit(PointerEventData data)
    {
        if (manager.Deleting)
            return;
        if (light.enabled)
        {
            bubble.rectTransform.sizeDelta = new Vector2(90, 90);
        }
    }

    private void OnPointerUp(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
            return;
        if (manager.Deleting)
            return;
        if (manager.PlayType == PlayType.OneClick)
        {
            if (light.enabled)
            {
                bubble.rectTransform.sizeDelta = new Vector2(90, 90);
            }
            manager.TakeSkillBalls();
        }
        else if(manager.StartType == 0)
        {
            manager.StartType = type;
            manager.PreType = type;
            light.enabled = true;
            bubble.rectTransform.sizeDelta = new Vector2(100, 100);
            manager.AddConnected(this);
        }
        else if (light.enabled)
        {
            bubble.rectTransform.sizeDelta = new Vector2(90, 90);
            manager.TakeSkillBalls();
        }

    }

    public IEnumerator DisableSkillBall()
    {
        int size = 90;
        bubble.rectTransform.sizeDelta = new Vector2(size, size);
        while (size > 0)
        {
            yield return null;
            size -= 5;
            bubble.rectTransform.sizeDelta = new Vector2(size, size);
            if(size < 30)
            {
                PlayEffect();
            }
        }
        
        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        Respawn();
    }

    public void PlayEffect()
    {
        effect.SetActive(true);
    }

    public void Dimmed()
    {
        if (!light.enabled)
        {
            bubble.color = new Vector4(0.6f, 0.6f, 0.6f, 1);
            icon.color = new Vector4(0.6f, 0.6f, 0.6f, 1);
        }
    }

    public void SetColorWhite()
    {
        bubble.color = Color.white;
        icon.color = Color.white;
    }

    public bool CheckDimmed()
    {
        if (bubble.color == Color.white)
            return false;
        return true;
    }

    public IEnumerator RespawnAllEffect()
    {
        Dimmed();
        yield return YieldInstructionCache.WaitForSeconds(0.2f);
        StartCoroutine(DisableSkillBall());
    }

    public void SetLightColor(Color color)
    {
        light.enabled = true;
        light.color = color;
    }
}
