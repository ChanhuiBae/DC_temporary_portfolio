using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    private Entity_Artifact artifact;
    private int mergre;
    public int Mergre
    {
        get => mergre;
    }
    private int id;
    public int InventoryID
    {
        get => id;
    }
    public int ArtifactID
    {
        get => artifact.ID;
    }
    private int speed = 15;
    private Rigidbody2D rig;
    private Vector2 force;

    private Image bubble;
    private Image icon;

    private EventTrigger trigger;
    private EventTrigger.Entry down;
    private EventTrigger.Entry up;
    private EventTrigger.Entry exit;
    private EventTrigger.Entry begin;
    private EventTrigger.Entry drag;
    private EventTrigger.Entry end;

    private ExplorationManager manager;

    private Bubble inBubble;

    private int index; // 0: not equip

    private ShopPopup shop;
    private bool sale;

    private Vector3 prePosition;

    private void Awake()
    {
        mergre = 1;
        inBubble = null;

        if (!transform.TryGetComponent<Rigidbody2D>(out rig))
        {
            Debug.Log("Bubble - Awake - Rigidbody2D");
        }
        if(!transform.TryGetComponent<Image>(out bubble))
        {
            Debug.Log("Bubble - Awake - Image");
        }
        if(!transform.GetChild(0).TryGetComponent<Image>(out icon))
        {
            Debug.Log("Bubble - Awake - Image");
        }
        if (!transform.TryGetComponent<EventTrigger>(out trigger))
        {
            Debug.Log("Bubble - Awake - EventTrigger");
        }
        else
        {
            down = new EventTrigger.Entry();
            down.eventID = EventTriggerType.PointerDown;
            down.callback.AddListener((data) => { OnDown((PointerEventData)data); });
            trigger.triggers.Add(down);

            up = new EventTrigger.Entry();
            up.eventID = EventTriggerType.PointerUp;
            up.callback.AddListener((data) => { OnUp((PointerEventData)data); });
            trigger.triggers.Add(up);

            exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => { StartCoroutine(CheckExit()); });
            trigger.triggers.Add(exit);

            begin = new EventTrigger.Entry();
            begin.eventID = EventTriggerType.BeginDrag;
            begin.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
            trigger.triggers.Add(begin);

            drag = new EventTrigger.Entry();
            drag.eventID = EventTriggerType.Drag;
            drag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
            trigger.triggers.Add(drag);

            end = new EventTrigger.Entry();
            end.eventID = EventTriggerType.EndDrag;
            end.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
            trigger.triggers.Add(end);
        }

        GameObject obj = GameObject.Find("Shop");
        if (obj == null || !obj.TryGetComponent<ShopPopup>(out shop))
        {
            Debug.Log("Bubble - Awake - ShopPopup");
        }
        sale = false;
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
    }

    public void SetBubble(int id, int mergre)
    {
        this.mergre = mergre;
        bubble.sprite = Resources.Load<Sprite>("Bubble/Bubble" + mergre);
        GameManager.Inst.GetArtifactData(id, out artifact);

        icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Icon);
        transform.position = transform.parent.transform.position;
        transform.LeanScale(Vector2.zero, 0);
        transform.LeanScale(Vector2.one, 0.5f);
        StartCoroutine(Moving());
    }

    private IEnumerator Moving()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        rig.isKinematic = false;
        rig.AddForce(new Vector2(Random.Range(-3, 4), Random.Range(-3, 4)) * speed, ForceMode2D.Impulse);
        force = rig.velocity;
    }

    void OnDown(PointerEventData data)
    {
        StopAllCoroutines();
        rig.velocity = Vector2.zero;
        if (data.button == PointerEventData.InputButton.Left)
        {
            ShowPopup();
        }
    }

    void OnUp(PointerEventData data)
    {
        if(data.button == PointerEventData.InputButton.Right)
        {
            manager.SetArtifactPopup(false, this);
        }
    }

    private void ShowPopup()
    {
        manager.ShowExplanArtifact(transform.position, artifact.ID);
    }

    private IEnumerator CheckExit()
    {
        while((Input.mousePosition.x < transform.position.x + 10
            && Input.mousePosition.x > transform.position.x - 10)
            || (Input.mousePosition.y < transform.position.y + 10
            && Input.mousePosition.y > transform.position.y - 10))
        {
            yield return null;
        }
        manager.CloseExplan();
        StartCoroutine(Moving());
    }

    void OnBeginDrag(PointerEventData data)
    {
        StopAllCoroutines();
        manager.CloseExplan();
        rig.velocity = Vector2.zero;
        rig.isKinematic = true;
        prePosition = transform.position;
        transform.SetAsLastSibling();
    }
    void OnDrag(PointerEventData data)
    {
        if (data.position.x < 0 || data.position.x > Screen.width || data.position.y < 0 || data.position.y > Screen.height)
        {
            StopAllCoroutines();
            rig.velocity = Vector2.zero;
            rig.isKinematic = true;
        }
        else
        {
            transform.position = data.position;
        }
    }

    void OnEndDrag(PointerEventData data)
    {
        if (inBubble != null)
        {
            if(mergre == inBubble.mergre)
            {
                int result = GameManager.Inst.GetArtifactCombinationResult(artifact.ID, inBubble.ArtifactID);
                if (result > 0)
                {
                    mergre++;
                    bubble.sprite = Resources.Load<Sprite>("Bubble/Bubble" + mergre);
                    inBubble.Despown();
                    inBubble = null;
                    SetBubble(result, mergre);
                }
            }
        }
        else if (index > 0)
        {
            if (GameManager.Inst.Exploration.artifacts.ContainsKey(index))
            {
                manager.SetArtifactPopup(true, this);
            }
            else
            {
                Equip();
            }
        }
        else if (sale)
        {
            StopAllCoroutines();
            manager.CloseExplan();
            shop.SaleArtifact(artifact);
            Despown();
        }
        else
        {
            float x = transform.position.x - transform.parent.position.x;
            float y = transform.position.y - transform.parent.position.y;
            if (Mathf.Abs(x) > 220 || Mathf.Abs(y) > 190)
            {
                StartCoroutine(GoPreposition());
            }
            else
            {
                StartCoroutine(Moving());
            }
        }
    }

    private IEnumerator GoPreposition()
    {
        float time = Mathf.Abs(Vector3.Distance(prePosition, transform.position)) * 0.001f;
        transform.LeanMove(prePosition, time);
        yield return YieldInstructionCache.WaitForSeconds(time);
        StartCoroutine(Moving());
    }

    public void Despown()
    {
        GameManager.Inst.Exploration.bubbles.Remove(this);
        gameObject.SetActive(false);
    }


    public void Equip()
    {
        if(GameManager.Inst.Exploration.SetArtifact(index, artifact.ID))
        {
            index = 0;
            Despown();
        }
        else
        {
            StartCoroutine(GoPreposition());
        }
    }

    public void GoToPreposition()
    {
        StartCoroutine(GoPreposition());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shop")
        {
            sale = true;
        }
        else if(collision.tag == "Artifact")
        {
            index = System.Int32.Parse(collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Shop")
        {
            sale = false;
        }
        else if (collision.tag == "Artifact")
        {
            index = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rig.isKinematic)
        {
            collision.transform.TryGetComponent<Bubble>(out inBubble);
        }
        else
        {
            Vector2 avgNormal = Vector2.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                avgNormal += collision.GetContact(i).normal;
            }
            avgNormal /= collision.contactCount;
            rig.velocity = Vector2.Reflect(force, avgNormal);
            force = rig.velocity;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (rig.isKinematic)
        {
            inBubble = null;
        }
    }
}
