using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_10_EarthSpear : Skill
{
    protected override void Awake()
    {
        base.Awake();
        attribute = AttributeType.Earth;
        anim = SkillAnimType.Point;
        type = SkillType.Attack;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        a_reenforce = manager.DeleteAttribute(AttributeType.Fire);
        yield return null;
        manager.AddAttribute(AttributeType.Earth);
        transform.position = target + new Vector3(0, 5f, 0);
        transform.LeanScale(new Vector3(17, 17, 1), 0);
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        float damage = ((skillBall / 6f - 0.5f) + 1.5f) * attack * (0.9f + 0.1f * level);
        if (f_reenforce)
            damage *= 1.3f;
        if (a_reenforce)
            return damage * 1.5f;
        return damage;
    }

}
