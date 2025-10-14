using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_13_Healing : Skill
{
    private FightSceneManager f_manager;
    protected override void Awake()
    {
        base.Awake();
        attribute = AttributeType.None;
        anim = SkillAnimType.Point;
        type = SkillType.Buff;
    }
    protected override IEnumerator PlaySkill(Vector3 target)
    {
        manager.AddAttribute(AttributeType.Water);
        GameManager.Inst.Exploration.player.HP += GameManager.Inst.Exploration.player.MaxHP * ((17.5f + 2.5f * level) + 5 * (SkillBall - 3)) / 100; 
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        manager.ResetUseSkill();
        gameObject.SetActive(false);
    }
}
