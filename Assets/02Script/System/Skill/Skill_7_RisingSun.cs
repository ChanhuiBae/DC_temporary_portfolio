using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_7_RisingSun : Skill
{
    private BoxCollider2D col;

    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<BoxCollider2D>(out col))
        {
            Debug.Log("Skill_7 - Awake - BoxCollider2D");
        }        
        attribute = AttributeType.Fire;
        anim = SkillAnimType.Point;
        type = SkillType.Attack;
    }

    protected override IEnumerator PlaySkill(Vector3 target)
    {
        a_reenforce = manager.DeleteAttribute(AttributeType.Water);
        yield return null;
        manager.AddAttribute(AttributeType.Fire);
        transform.position = new Vector3(-9, 7, 0);
        transform.LeanScale(new Vector3(0, 0, 0), 0f);
        transform.LeanScale(new Vector3(3, 3, 1), 1f);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        manager.ResetUseSkill();
        for (int i = 0; i < 10; i++) 
        {
            col.enabled = true;
            yield return YieldInstructionCache.WaitForSeconds(1f);
            col.enabled = false;
        }
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        float damage = ((skillBall / 6f - 0.5f) * 0.15f + 0.2f) * attack * (0.9f + 0.1f * level);
        if (f_reenforce)
            return damage * 1.3f;
        return damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Monster")
        {
            if (a_reenforce)
            {
                //지속 시간 동안 적들의 방어력 - 50 %
            }
        }
    }
}
