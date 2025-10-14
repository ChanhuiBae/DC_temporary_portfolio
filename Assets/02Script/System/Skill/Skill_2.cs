using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_2 : Skill
{
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        transform.position = target;
        transform.LeanScale(new Vector3(3, 3, 0), 0);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }
}
