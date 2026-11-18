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
    // 현재 UI가 표시하는 데이터. 보유 목록에도 동일한 객체가 들어 있습니다.
    private ArtifactData data;
    public int InventoryID
    {
        get => data != null ? data.InventoryID : -1;
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
        if (!transform.TryGetComponent<Image>(out bubble))
        {
            Debug.Log("Bubble - Awake - Image");
        }
        if (!transform.GetChild(0).TryGetComponent<Image>(out icon))
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

    // 씬 복귀 또는 풀 재사용 시 기존 데이터에 새 UI를 연결합니다.
    // 여기서는 보유 목록에 추가하지 않습니다.
    public void Bind(ArtifactData data)
    {
        StopAllCoroutines();
        LeanTween.cancel(gameObject);
        this.data = data;
        manager = (ExplorationManager)GameManager.Inst.manager;

        // 이전에 표시하던 버블의 드래그/판매/물리 상태를 초기화합니다.
        inBubble = null;
        index = 0;
        sale = false;
        force = Vector2.zero;
        rig.velocity = Vector2.zero;
        rig.angularVelocity = 0f;
        rig.isKinematic = true;

        SetBubble(data.UniqueID, data.level);
        prePosition = transform.position;
    }

    // 합성 시 UI뿐 아니라 보유 데이터도 함께 변경합니다.
    public void SetBubble(int id, int mergre)
    {
        if (data == null)
        {
            Debug.LogError("Bubble: Bind로 데이터를 연결한 뒤 호출해야 합니다.");
            return;
        }

        Entity_Artifact nextArtifact;
        if (!GameManager.Inst.GetArtifactData(id, out nextArtifact)
            || nextArtifact == null)
        {
            Debug.LogError("Bubble: 존재하지 않는 아티팩트 ID: " + id);
            return;
        }

        ArtifactData newArtifact = new ArtifactData(data.InventoryID, id, mergre);
        data = newArtifact;
        this.mergre = mergre;
        artifact = nextArtifact;
        bubble.sprite = Resources.Load<Sprite>("Bubble/Bubble" + mergre);
        icon.sprite = Resources.Load<Sprite>("Artifact/" + artifact.Icon);

        StopAllCoroutines();
        LeanTween.cancel(gameObject);
        rig.velocity = Vector2.zero;
        rig.isKinematic = true;
        transform.position = transform.parent.position;
        transform.LeanScale(Vector2.zero, 0);
        transform.LeanScale(Vector2.one, 0.5f);
        StartCoroutine(Moving());
    }

    // 씬 전환이나 단순 비활성화는 아이템 소비가 아닙니다.
    // 이곳에서는 연출만 중단하고 보유 데이터는 삭제하지 않습니다.
    private void OnDisable()
    {
        StopAllCoroutines();
        LeanTween.cancel(gameObject);
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
        if (data.button == PointerEventData.InputButton.Right)
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
        while ((Input.mousePosition.x < transform.position.x + 10
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
        if (inBubble != null && inBubble != this)
        {
            if (mergre == inBubble.mergre)
            {
                int result = GameManager.Inst.GetArtifactCombinationResult(
                    artifact.ID, inBubble.ArtifactID);
                Entity_Artifact resultArtifact;
                if (result > 0
                    && GameManager.Inst.GetArtifactData(result, out resultArtifact)
                    && resultArtifact != null)
                {
                    Bubble consumedBubble = inBubble;
                    int nextMergeLevel = mergre + 1;

                    // 내 데이터는 합성 결과로 변경하고 상대 데이터 한 개만 소비합니다.
                    SetBubble(result, nextMergeLevel);
                    inBubble = null;
                    consumedBubble.Despown();
                    return;
                }
            }

            // 합성할 수 없는 조합은 이동을 재개합니다.
            StartCoroutine(GoPreposition());
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
        // 합성 재료 소비, 장착, 판매, 파괴 시에만 보유 목록에서 제거합니다.
        // 같은 artifactID를 가진 다른 데이터는 영향을 받지 않습니다.
        if (data != null)
        {
            GameManager.Inst.Exploration.bubbles.Remove(data);
            data = null;
        }
        inBubble = null;
        index = 0;
        sale = false;
        gameObject.SetActive(false);
    }


    public void Equip()
    {
        if (GameManager.Inst.Exploration.SetArtifact(index, artifact.ID))
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
        else if (collision.tag == "Artifact")
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
