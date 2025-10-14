using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttackArea
{
    None = 0,
    Target = 1,
    Row = 2,
    Column = 3,
    All = 4,
}

public class SkillManager : MonoBehaviour
{
    private int useSkill;
    private ParticleSystem dust;
    private SpriteRenderer pentagram;

    [SerializeField]
    private Transform firePosition;
    private MonsterManager monsterManager;
    private AttributeManager attribute;

    private Button partnerBtn;
    private Image partnerFill;
    private Image partnerImage;
    private Image partnerFrame;
    private GameObject partnerSkillBack;
    private CharacterAnimationController partnerAnim;

    protected PlayerController player;

    public bool Usable
    {
        get
        {
            if (useSkill == 0)
                return true;
            return false;
        }
    }
    public bool SetUseType(int id, float cast)
    {
        if(useSkill == 0)
        {
            useSkill = id;
            StartCoroutine(LoopForCastTime(cast));
            pentagram.enabled = true;
            return true;
        }
        return false;
    }

    private IEnumerator LoopForCastTime(float cast)
    {
        var main = dust.main;
        main.loop = true;
        dust.Play();
        yield return new WaitForSeconds(cast-0.7f);
        main.loop = false;
    }

    public void ResetUseSkill()
    {
        useSkill = 0;
        dust.Stop();
        pentagram.enabled = false;
    }

    protected Dictionary<int,Skill> skills = new Dictionary<int, Skill>();

    private void Awake()
    {
        GameObject obj = GameObject.Find("MagicDust");
        if (obj == null || !obj.transform.TryGetComponent<ParticleSystem>(out dust))
        {
            Debug.Log("SkillManager - Awake - ParticleSystem");
        }
        obj = GameObject.Find("Pentagram");
        if (obj == null || !obj.transform.TryGetComponent<SpriteRenderer>(out pentagram))
        {
            Debug.Log("SkillManager - Awake - SpriteRenderer");
        }
        else
        {
            pentagram.enabled = false;
        }
        obj = GameObject.Find("MonsterManager");
        if (obj == null || !obj.transform.TryGetComponent<MonsterManager>(out monsterManager))
        {
            Debug.Log("SkillManager - Awake - MonsterManager");
        }
        obj = GameObject.Find("AttributeManager");
        if (obj == null || !obj.TryGetComponent<AttributeManager>(out attribute))
        {
            Debug.Log("SkillManager - Awake - AttributeManager");
        }
        obj = GameObject.Find("Player");
        if (obj == null || !obj.transform.TryGetComponent<PlayerController>(out player))
        {
            Debug.Log("SkillManager - Awake  - PlayerController");
        }

        obj = GameObject.Find("PartnerFill");
        if (obj != null)
        {
            if(!obj.transform.TryGetComponent<Button>(out partnerBtn))
            {
                Debug.Log("SkillManager - Awake - Button");
            }
            if (!obj.transform.TryGetComponent<Image>(out partnerFill))
            {
                Debug.Log("SkillManager - Awake - Image");
            }
            else
            {
                partnerFill.color = new Color(1, 0.8f, 0, 0.7f);
            }
            SetPartnerFill();

            partnerSkillBack = GameObject.Find("PartnerSkillBack");
            if(partnerSkillBack == null || !partnerSkillBack.transform.GetChild(0).TryGetComponent<CharacterAnimationController>(out partnerAnim))
            {
                Debug.Log("SkillManager - Awake - PartnerAnim");
            }
        }
        obj = GameObject.Find("PartnerImage");
        if (obj == null || !obj.TryGetComponent<Image>(out partnerImage))
        {
            Debug.Log("SkillManager - Awake - Image");
        }
        else
        {
            // todo: get partner icon;
        }
        obj = GameObject.Find("PartnerFrame");
        if (obj == null || !obj.TryGetComponent<Image>(out partnerFrame))
        {
            Debug.Log("SkillManager - Awake - Image");
        }
        else
        {
            partnerFrame.enabled = false;
        }
    }

    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Skill s = transform.GetChild(i).GetComponent<Skill>();
            skills.Add((int)s.ID, s);
        }

        // todo: get parthner data from game manager
        partnerBtn.onClick.AddListener(() => { StartPartnerSkill(skills[2]); });
        partnerBtn.enabled = false;

        partnerAnim.SetReady(true);
    }

    public bool UseSkill(int id, int skillBallCount, bool reenforce)
    {
        Entity_Skill e_skill;
        GameManager.Inst.GetSkillData(id, out e_skill);
        float cast = e_skill.Cast;
        cast *= GameManager.Inst.Exploration.cast_length * GameManager.Inst.Exploration.cast_speed;
        if (reenforce)
            cast *= 0.75f;
        if(cast < 0)
            cast = 0;
        if (SetUseType(id, cast))
        {
            StartCoroutine(WaitPlay(id, skillBallCount, reenforce, cast));
            return true;
        }
        return false;
    }

    private IEnumerator WaitPlay(int id, int skillBallCount, bool reenforce, float cast)
    {
        yield return YieldInstructionCache.WaitForSeconds(cast);
        switch (skills[id].Anim)
        {
            case SkillAnimType.Fire:
                player.PlayAnimation(CharacterStateDC.Slash);
                break;
            case SkillAnimType.Point:
                player.PlayAnimation(CharacterStateDC.Jab);
                break;
            case SkillAnimType.Buff:
                player.PlayAnimation(CharacterStateDC.Push);
                break;
        }
        yield return YieldInstructionCache.WaitForSeconds(0.7f);
        GameManager.Inst.Exploration.SetSkillUseCount(id);
        skills[id].gameObject.SetActive(true);
        skills[id].ATK = GameManager.Inst.Exploration.player.ATK;
        skills[id].SkillBall = skillBallCount;
        skills[id].F_Reenforce = reenforce;
        switch (skills[id].Anim)
        {
            case SkillAnimType.Fire:
                skills[id].StartPlay(player, firePosition.position, monsterManager.GetTargetCenter());
                break;
            case SkillAnimType.Point:
                skills[id].StartPlay(monsterManager.GetTargetGround());
                break;
            case SkillAnimType.Buff:
                skills[id].StartPlay(monsterManager.GetTargetGround());
                break;
        }
        GameManager.Inst.Exploration.UsingSkill = false;
    }


    private void StartPartnerSkill(Skill s)
    {
        GameManager.Inst.Exploration.PartnerValue = 0;
        partnerFrame.enabled = false;
        partnerFill.color = new Color(1,0.8f,0,0.7f);
        SetPartnerFill();
        StartCoroutine(UsePartnerSkill(s));
    }

    private IEnumerator UsePartnerSkill(Skill s)
    {
        partnerSkillBack.LeanMoveX(-2.5f, 0.25f);
        partnerAnim.transform.LeanMoveX(-40, 0.25f);
        yield return YieldInstructionCache.WaitForSeconds(0.25f);
        partnerAnim.transform.LeanMoveX(-5f, 0.5f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);
        partnerAnim.Slash();
        partnerAnim.transform.LeanMoveX(2f, 1f);
        yield return YieldInstructionCache.WaitForSeconds(0.9f);
        partnerAnim.transform.LeanMoveX(32, 0.5f);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        partnerSkillBack.transform.LeanMoveX(32, 0.25f);
        yield return YieldInstructionCache.WaitForSeconds(0.25f);
        UseSkill(s.ID ,s.SkillBall, false);
        partnerBtn.enabled = false;
        partnerSkillBack.transform.LeanMoveX(-40, 0);
        partnerAnim.transform.LeanMoveX(-40, 0f);
    }

    public void AddFill(int addValue)
    {
        GameManager.Inst.Exploration.PartnerValue += addValue;
        if(GameManager.Inst.Exploration.PartnerValue >= GameManager.Inst.Exploration.partnerMax)
        {
            partnerBtn.enabled = true;
            partnerFrame.enabled = true;
            partnerFill.color = new Color(1,0.8f,0,1);
        }
        SetPartnerFill();
    }
    private void SetPartnerFill()
    {
        partnerFill.fillAmount = (float)GameManager.Inst.Exploration.PartnerValue / GameManager.Inst.Exploration.partnerMax;
    }

    public bool DeleteAttribute(int amount)
    {
        if (attribute.GetCount() >= amount)
        {
            while (amount > 0)
            {
                AttributeType a = attribute.GetFirstAttribute();
                if (a != AttributeType.None)
                {
                    attribute.StartCoroutine(attribute.DeleteAttribute(a));
                }
                amount--;
            }
            return true;
        }
        return false;
    }

    public bool DeleteAttribute(AttributeType delete)
    {
        if (attribute.CheckAttribute(delete))
        {
            attribute.StartCoroutine(attribute.DeleteAttribute(delete));
            return true;
        }
        return false;
    }

    public void AddAttribute(AttributeType add)
    {
        attribute.StartCoroutine(attribute.AddAttribute(add));
    }
}
