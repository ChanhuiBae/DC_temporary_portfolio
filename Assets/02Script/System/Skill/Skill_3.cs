using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_3 : Skill
{
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        transform.LeanScale(new Vector3(3, 3, 1), 0);
        transform.position = target + new Vector3(1, 2, 0);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        return attack;
    }
}
