using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_11_EnergyBash : Skill
{
    protected override void Awake()
    {
        base.Awake();
        attribute = AttributeType.None;
        anim = SkillAnimType.Point;
        type = SkillType.Attack;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        a_reenforce = manager.DeleteAttribute(2);
        transform.position = target + new Vector3(0, 3, 0);
        transform.LeanScale(new Vector3(4, 4, 1), 0);
        yield return YieldInstructionCache.WaitForSeconds(0.4f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        float damage = ((skillBall / 6f - 0.5f) * 1.5f + 1.0f) * attack * (0.85f + 0.15f * level);
        if (f_reenforce)
            damage *= 1.3f;
        if (a_reenforce)
            return damage * 2f;
        return damage;
    }
}
