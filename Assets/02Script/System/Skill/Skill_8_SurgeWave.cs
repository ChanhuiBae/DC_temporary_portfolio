using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_8_SurgeWave : Skill
{
    private BoxCollider2D col;
    protected override void Awake()
    {
        base.Awake();
        if(!TryGetComponent<BoxCollider2D>(out col))
        {
            Debug.Log("Skill_8 - Awake - BoxCollider2D");
        }
        attribute = AttributeType.Water;
        anim = SkillAnimType.Point;
        type = SkillType.Attack;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        a_reenforce = manager.DeleteAttribute(AttributeType.Thunder);
        yield return null;
        manager.AddAttribute(AttributeType.Water);
        transform.position = new Vector3(-7, target.y+3.5f, 0);
        transform.LeanScale(new Vector3(22, 22, 1), 0);
        col.offset = new Vector2(-0.05f, -0.15f);
        transform.LeanMoveLocalX(6f, 1.5f);
        yield return YieldInstructionCache.WaitForSeconds(0.9f);
        col.offset = new Vector2(0.06f, -0.1f);
        yield return YieldInstructionCache.WaitForSeconds(0.6f);
        transform.LeanMoveLocalX(8f, 0.35f);
        yield return YieldInstructionCache.WaitForSeconds(0.35f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        float damage = ((skillBall / 6f - 0.5f) + 0.8f) * attack * (0.9f + 0.1f * level);
        if (f_reenforce)
            return damage * 1.3f;
        return damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Monster")
        {
            if (a_reenforce)
            {
                //피격당한 적들의 행동 게이지 상승 속도를 5초간 -50%
            }
        }
    }
}
