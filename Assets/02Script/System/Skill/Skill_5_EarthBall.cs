using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_5_EarthBall : Skill
{
    protected override void Awake()
    {
        base.Awake();
        attribute = AttributeType.Earth;
        anim = SkillAnimType.Fire;
        type = SkillType.Attack;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        transform.LeanScale(new Vector3(23, 23, 0), 0);
        yield return YieldInstructionCache.WaitForSeconds(0.3f);

        transform.LeanMove(target, 0.5f);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        if(f_reenforce)
            return ((skillBall / 6f - 0.5f) * 0.6f + 0.5f) * attack * 1.3f;
        return ((skillBall / 6f - 0.5f) * 0.6f + 0.5f) * attack;
    }
}
