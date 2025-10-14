using System.Collections;
using UnityEngine;

public class MonsterController : CharacterController
{
    private FightSceneManager manager;
    private MonsterManager monsterManager;
    private MonsterSkillManager skillManager;
    private Animator animator;
    private Entity_Monster data;
    public Entity_Monster Data
    {
        set => data = value;
    }

    private int priority;
    public int Priority
    {
        set => priority = value;
        get => priority;
    }

    private float fullHp;
    public float FullHP
    {
        set
        {
            fullHp = value;
            hp = value;
        }
    }
    private float hp;
    public float HP
    {
        get => hp;
    }

    private float DEF;
    private float criticalPercent;

    private int fullGage;
    public int FullGage
    {
        set
        {
            fullGage = value;
        }
    }
    private float gage;
    public float Gage
    {
        get => gage;
    }
    private float actionSpeed;
    public float ActionSpeed
    {
        set => actionSpeed = value;
        get => actionSpeed;
    }

    private GameObject hit;
    private int preSkillID;

    protected override void Awake()
    {
        base.Awake();
        GameObject m = GameObject.Find("MonsterManager");
        if(m == null || !m.transform.TryGetComponent<MonsterManager>(out monsterManager))
        {
            Debug.Log("MonsterController - Awake - MonsterManager");
        }
        GameObject sm = GameObject.Find("MonsterSkillManager");
        if(sm == null || !sm.transform.TryGetComponent<MonsterSkillManager>(out skillManager))
        {
            Debug.Log("MonsterController - Awake - MonsterManager");
        }
        if(!TryGetComponent<Animator>(out animator))
        {
            Debug.Log("MonsterController - Awake - Animator");
        }

        hit = transform.GetChild(1).gameObject;
        hit.SetActive(false);
    }

    private void Start()
    {
        manager = (FightSceneManager)GameManager.Inst.manager;
        criticalPercent = GameManager.Inst.Exploration.player.Critical;
        DEF = data.DEF;
        actionSpeed = 0;
        preSkillID = 0;
        isAlive = true;
    }

    public void SetAnimator(string path)
    {
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/"+path);
    }

    public void StartAction()
    {
        SetGageZero();
        StartCoroutine(ChargeGage());
        PlayAnimation(CharacterStateDC.Run);
    }

    private IEnumerator PlayHit(float time)
    {
        hit.SetActive(true);
        PlayAnimation(CharacterStateDC.TakeDamage);
        yield return YieldInstructionCache.WaitForSeconds(time);
        hit.SetActive(false);
    }

    private void UseSkill()
    {
        state = CharacterStateDC.Ready;
        switch (skillManager.UseSkill(this, transform.position, data.ATK * GameManager.Inst.Exploration.MonsterWeight))
        {
            case SkillAnimType.Fire:
                PlayAnimation(CharacterStateDC.Slash);
                break;
            case SkillAnimType.Point:
                PlayAnimation(CharacterStateDC.Jab);
                break;
            case SkillAnimType.Buff:
                PlayAnimation(CharacterStateDC.Push);
                break;
            default:
                PlayAnimation(CharacterStateDC.Attack);
                break;
        }
        SetGageZero();
        state = CharacterStateDC.Ready;
    }


    public void SetGageZero()
    {
        gage = 0;
        monsterManager.SetGage(priority, 0);
    }

    private IEnumerator ChargeGage()
    {
        yield return YieldInstructionCache.WaitForSeconds(1.7f);
        PlayAnimation(CharacterStateDC.Ready);

        while (isAlive)
        {
            yield return YieldInstructionCache.WaitForSeconds(1);
            gage += 1 * (GameManager.Inst.Exploration.MonsterWeight + actionSpeed);
            if(gage > fullGage)
            {
                gage = fullGage;
                if(state != CharacterStateDC.Slash && state != CharacterStateDC.Jab && state != CharacterStateDC.Push)
                {
                    UseSkill();
                }
            }
            monsterManager.SetGage(priority, (float)gage / fullGage);
        }
    }

    public void SetScale(bool all, int magnification)
    {
        if (all)
        {
            transform.localScale = new Vector3(magnification, magnification, 1);
        }
        else
        {
            CapsuleCollider2D col;
            col = transform.GetComponent<CapsuleCollider2D>();
            col.size *= magnification;
            col.transform.position += new Vector3(0, col.size.y, 0);
        }
    }

    public Vector3 GetScale()
    {
        return transform.GetComponent<CapsuleCollider2D>().bounds.size;
    }
    public IEnumerator SetAtionSpeed(float value, float time)
    {
        actionSpeed = value;
        yield return YieldInstructionCache.WaitForSeconds(time);
        actionSpeed = 0;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAlive && collision.transform.tag == "Skill")
        {
            Skill skill;
            if (collision.transform.TryGetComponent<Skill>(out skill))
            {
                // Calculate BaseDamage
                float damage = skill.GetDamage();
                // Get critical
                bool critical = UnityEngine.Random.Range(0, 100) < criticalPercent ? true : false;

                CalculateByDEF(damage, skill.Attribute, critical, skill.ID);
            }
        }
    }

    public override void TakeDamage(float damage, AttributeType attributeType, bool critical)
    {
        CalculateByDEF(damage, attributeType, critical);
    }

    private void CalculateByDEF(float damage, AttributeType attributeType, bool critical, int skillID = 0)
    {
        // Check Artifact
        if (GameManager.Inst.Exploration.artifacts.ContainsKey(37))
        {
            DEF *= 0.95f;
            if (DEF <= 0)
            {
                DEF = 0;
                criticalPercent = GameManager.Inst.PlayerData.Critical * 1.15f;
            }
        }

        damage *= 100;
        damage /= data.DEF * GameManager.Inst.Exploration.MonsterWeight + 100;
        float attribute = 0;
        switch (attributeType)
        {
            case AttributeType.Fire:
                attribute = data.Fire * GameManager.Inst.Exploration.MonsterWeight;
                break;
            case AttributeType.Water:
                attribute = data.Water * GameManager.Inst.Exploration.MonsterWeight;
                break;
            case AttributeType.Thunder:
                attribute = data.Thunder * GameManager.Inst.Exploration.MonsterWeight;
                break;
            case AttributeType.Earth:
                attribute = data.Earth * GameManager.Inst.Exploration.MonsterWeight;
                break;
        }
        damage *= 1 - attribute * 0.01f;
        CalculateByCritical(damage, attribute, critical, skillID);
    }

    private void CalculateByCritical(float damage, float attribute, bool critical, int skillID = 0)
    {
        // Check Artifact
        GameManager.Inst.artifactManager.CheckAfterCritical(critical);

        if (critical)
            damage *= GameManager.Inst.Exploration.player.CriticalDamage * 0.01f;

        damage = GameManager.Inst.artifactManager.CheckBeforeTakeDamage(damage, this, attribute, critical);

        if (damage < 1)
            damage = 1;

        bool miss = Random.Range(0, 100) < GameManager.Inst.Exploration.miss ? true : false;
        damage = miss ? 0 : damage;

        TakeDamage((int)damage, attribute, critical);

        if (!miss)
        {
            if (preSkillID == 0 || preSkillID != skillID)
            {
                GameManager.Inst.artifactManager.CheckEachSkill(critical);
                preSkillID = skillID;
            }
            GameManager.Inst.artifactManager.CheckEachBlow(critical, this);
        }
    }

    public void TakeDamage(int damage, float attribute, bool critical) // final step
    {
        StartCoroutine(PlayHit(0.05f));
        hp -= damage;
        GameManager.Inst.eventManager.PostNotification(GameEventType.MonsterTakeDamage, damage);
        monsterManager.SetHP(priority, hp / fullHp);
        Color color = Color.white;
        if (attribute >= 30)
            color = Color.gray;
        else if (attribute <= -30)
            color = new Color(255, 128, 0, 1);
        else
            color = Color.white;
        manager.ShowDamage(transform.position + new Vector3(-0.8f, 1.2f, 0) * data.Magnification, (int)damage, color, critical);
        anim.TakeDamage();
        if (hp <= 0)
        {
            isAlive = false;
            monsterManager.DisableGage(priority);
            monsterManager.ReTarget();
            PlayAnimation(CharacterStateDC.Die);
            GameManager.Inst.eventManager.PostNotification(GameEventType.MonsterDie);
        }
    }



}
