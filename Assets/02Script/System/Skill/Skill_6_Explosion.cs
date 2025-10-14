using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_6_Explosion : Skill
{
    protected override void Awake()
    {
        base.Awake();
        attribute = AttributeType.Fire;
        anim = SkillAnimType.Point;
        type = SkillType.Attack;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        a_reenforce = manager.DeleteAttribute(AttributeType.Water);
        yield return null;
        manager.AddAttribute(AttributeType.Fire);
        transform.position = new Vector3(4, 3.5f, 0);
        transform.LeanScale(new Vector3(2.5f, 2.5f, 1), 0);
        yield return YieldInstructionCache.WaitForSeconds(0.7f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        float damage = ((skillBall / 6f - 0.5f) + 1) * attack * (0.9f + 0.1f * level);
        if (f_reenforce)
            damage *= 1.3f;
        if (a_reenforce)
            return damage * 1.3f;
        return damage;
    }
}
