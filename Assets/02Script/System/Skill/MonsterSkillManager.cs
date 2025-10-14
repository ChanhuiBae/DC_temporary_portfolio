using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillManager : SkillManager
{
    protected void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Skill s = transform.GetChild(i).GetComponent<Skill>();
            skills.Add(i, s);
        }
    }

    

    public SkillAnimType UseSkill(MonsterController user, Vector3 start, float ATK)
    {
        if (user.Priority > skills.Count || user.Priority < 0)
            return SkillAnimType.Attack;
        else if(GameManager.Inst.Exploration.player.HP > 0)
        {
            skills[user.Priority].gameObject.SetActive(true);
            skills[user.Priority].ATK = ATK;
            skills[user.Priority].StartPlay(user , start, player.transform.position);
            return skills[user.Priority].Anim;
        }
        return SkillAnimType.None;
    }

}
