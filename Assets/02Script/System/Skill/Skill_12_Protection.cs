using System.Collections;
using UnityEngine;

public class Skill_12_Protection : Skill
{
    private CircleCollider2D col;
    private float hp;
    private int H_Start = Animator.StringToHash("Start");
    private int H_Hit = Animator.StringToHash("Hit");
    private int H_Pop = Animator.StringToHash("Pop");
    protected override void Awake()
    {
        base.Awake();
        if(!TryGetComponent<CircleCollider2D>(out  col))
        {
            Debug.Log("Skill_12 - Awake - CircleCollider2D");
        }
        attribute = AttributeType.None;
        anim = SkillAnimType.Point;
        type = SkillType.Defense;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        GameManager.Inst.player.AddShield(1, col);
        GameManager.Inst.player.SetShield(1, true);
        transform.position = GameManager.Inst.player.transform.position + new Vector3(0,2,0);
        animator.SetTrigger(H_Start);
        transform.LeanScale(new Vector3(1.5f, 1.5f, 1), 0);
        hp = ((skillBall / 6f - 0.5f) * 3f + 1f) * GameManager.Inst.Exploration.player.MaxHP * 0.05f * (0.9f + 0.1f * level);
        if (f_reenforce)
            hp *= 1.3f;
        yield return null;
        manager.ResetUseSkill();
    }

    private IEnumerator EnableSkill()
    {
        animator.SetTrigger(H_Pop);
        yield return null;
        GameManager.Inst.player.SetShield(1, false);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Skill")
        {
            hp -= collision.transform.GetComponent<Skill>().GetDamage();
            collision.gameObject.SetActive(false);
            animator.SetTrigger(H_Hit);
            if (hp <= 0)
            {
                StartCoroutine(EnableSkill());
            }
        }
    }
}
