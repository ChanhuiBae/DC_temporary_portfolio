using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    private SkillQueue queue;
    private Image bubble;
    private Image icon;
    private int id;
    public int ID
    {
        get => id;
    }
    private int skillballCount;
    public int Skillball
    {
        get => skillballCount;
    }
    private GameObject effectFlash;
    private GameObject effectSparkle;

    public bool Fever
    {
        get => effectSparkle.activeSelf;
    }

    private void Awake()
    {
        if(!transform.parent.parent.TryGetComponent<SkillQueue>(out queue))
        {
            Debug.Log("SkillItem  - Awake - SkillQueue");
        }
        bubble = transform.GetComponent<Image>();
        if(!transform.GetChild(0).TryGetComponent<Image>(out icon))
        {
            Debug.Log("SkillItem - Awake - Image");
        }
        effectFlash = transform.GetChild(2).gameObject;
        effectFlash.SetActive(false);
        effectSparkle = transform.GetChild(3).gameObject;
        effectSparkle.SetActive(false);
    }

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, -50, 0);
    }

    public void Init(int id,Vector3 position, int skillballCount)
    {
        this.id = id;
        this.skillballCount = skillballCount;
        Entity_Skill skill;
        GameManager.Inst.GetSkillData(id, out skill);
        bubble.sprite = Resources.Load<Sprite>("Bubble/" + skill.Bubble);
        if(id > 5)
        {
            icon.enabled = true;
            icon.sprite = Resources.Load<Sprite>("Skill/"+ skill.Icon);
        }
        else
            icon.enabled = false;
        effectFlash.SetActive(false);
        if (queue.Fever)
            effectSparkle.SetActive(true);
        else
            effectSparkle.SetActive(false);
        transform.position = position;
        transform.LeanScale(Vector3.zero, 0);
        transform.LeanScale(new Vector3(1.2f,1.2f,1.2f), 0.3f);
        StartCoroutine(WaitScaling());
    }

    private IEnumerator WaitScaling()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.3f);
        transform.LeanScale(Vector3.one, 0.15f);
        yield return YieldInstructionCache.WaitForSeconds(0.3f);
        queue.SetReady(this, true);
    }

    public void MoveUp(Vector3 position)
    {
        queue.SetReady(this, false);
        transform.LeanMove(position, 0.5f);
        StartCoroutine(WaitMoving());
    }

    private IEnumerator WaitMoving()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        queue.SetReady(this,true);
    }

    public void StartFlash()
    {
        StartCoroutine(PlayFlash());
    }

    private IEnumerator PlayFlash()
    {
        int size = 75;
        bubble.rectTransform.sizeDelta = new Vector2(size, size);
        while (size > 0)
        {
            yield return null;
            size -= 1;
            bubble.rectTransform.sizeDelta = new Vector2(size, size);
            if (size < 30)
            {
                effectFlash.SetActive(true);
            }
        }
        queue.Dequeue();
        transform.position = new Vector3(transform.position.x, -50, 0);
        bubble.rectTransform.sizeDelta = new Vector2(75, 75);
    }
}
