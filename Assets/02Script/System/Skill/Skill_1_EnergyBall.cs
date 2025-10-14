using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Skill_1_EnergyBall : Skill
{
    protected override void Awake()
    {
        base.Awake();
        attribute = AttributeType.None;
        anim = SkillAnimType.Fire;
        type = SkillType.Attack;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        Vector3 v1 = target - transform.position;
        Vector3 v2 = new Vector3(target.x, transform.position.y, 0) - transform.position;
        float theta = Vector3.Angle(v1, v2);
        if (target.y < transform.position.y)
            theta = -theta;
        transform.LeanRotateZ(theta, 0);
        transform.LeanScale(new Vector3(8, 8, 0), 0);
        transform.LeanMove(target, 0.3f);
        yield return YieldInstructionCache.WaitForSeconds(0.3f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        if (f_reenforce)
            return ((skillBall / 6f - 0.5f) * 0.6f + 0.5f) * attack * 1.3f;
        return ((skillBall / 6f - 0.5f) * 0.6f + 0.5f) * attack; 
    }
}
