using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    None = 0,
    Attack = 1,
    Buff = 2,
    Defense = 3,
}
public enum SkillAnimType
{
    None = 0,
    Fire = 1,
    Point = 2,
    Buff = 3,
    Attack = 4,
}

public class Skill : MonoBehaviour
{
    protected SkillManager manager;
    protected SkillType type;
    public SkillType Type
    {
        get => type;
    }

    protected SkillAnimType anim;

    public SkillAnimType Anim
    {
        get => anim;
    }

    protected int id;
    public int ID
    {
        get => id;
    }
    protected int level = 1;
    public int Level
    {
        get => level;
        set => level = value;
    }

    protected CharacterController user;
    public CharacterController User
    {
        get => user;
    }

    protected float attack;
    public float ATK
    {
        set => attack = value;
        get => attack;
    }
    protected int skillBall;
    public int SkillBall
    {
        set => skillBall = value;
        get => skillBall;
    }
    protected bool f_reenforce;
    public bool F_Reenforce
    {
        set => f_reenforce = value;
    }
    protected bool a_reenforce = false;
    protected AttributeType attribute;
    public AttributeType Attribute
    {
        get => attribute;
        set => attribute = value;
    }
    protected Animator animator;


    protected virtual void Awake()
    {
        id = Int32.Parse(gameObject.name);
        if(!transform.parent.TryGetComponent<SkillManager>(out manager))
        {
            Debug.Log("Skill - Awake - SkillManager");
        }
        if(!TryGetComponent<Animator>(out animator))
        {
            Debug.Log("Skill - Awake - Animator");
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }


    public void StartPlay(CharacterController user, Vector3 start, Vector3 target)
    {
        this.user = user;
        transform.LeanScale(Vector3.zero, 0);
        transform.position = start;
        StartCoroutine(PlaySkill(target));
    }

    public void StartPlay(Vector3 target)
    {
        transform.LeanScale(Vector3.zero, 0);
        StartCoroutine(PlaySkill(target));
    }

    protected virtual IEnumerator PlaySkill(Vector3 target)
    {
        transform.LeanScale(new Vector3(3,3,0), 1);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        transform.LeanMove(target, 1f);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }
    protected virtual IEnumerator PlaySkill()
    {
        transform.LeanScale(new Vector3(3, 3, 0), 1);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }

    public virtual float GetDamage()
    {
        return 0;
    }
}
