using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_9_LightningRain : Skill
{
    private MonsterManager monsterManager; 
    private BoxCollider2D col;
    private int count;
    private Coroutine play;
    protected override void Awake()
    {
        base.Awake();
        GameObject obj = GameObject.Find("MonsterManager");
        if(obj == null || !obj.TryGetComponent<MonsterManager>(out monsterManager))
        {
            Debug.Log("Skill_9 - Awake - MonsterManager");
        }
        if (!TryGetComponent<BoxCollider2D>(out col))
        {
            Debug.Log("Skill_9 - Awake - BoxCollider2D");
        }
        attribute = AttributeType.Thunder;
        anim = SkillAnimType.Point;
        type = SkillType.Attack;
        count = 0;
        play = null;
    }

    protected override IEnumerator PlaySkill(Vector3 target)
    {
        a_reenforce = manager.DeleteAttribute(AttributeType.Fire);
        yield return null;
        manager.AddAttribute(AttributeType.Thunder);
        count += skillBall - 2;
        if(play == null)
        {
            play = StartCoroutine(Play());
        }
        manager.ResetUseSkill();
    }

    private IEnumerator Play()
    {
        transform.LeanScale(new Vector3(22, 22, 1), 0);
        while (count > 0)
        {
            col.enabled = false;
            transform.position = monsterManager.GetRandomTarget() + new Vector3(0,10,0);
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            col.enabled = true;
            yield return YieldInstructionCache.WaitForSeconds(0.3f);
            count--;
        }
        play = null;
        gameObject.SetActive(false);
    }
    

    public override float GetDamage()
    {
        if(f_reenforce)
            return 0.3f * attack * (0.9f + 0.1f * level) *1.3f;
        return  0.3f * attack * (0.9f + 0.1f * level);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Monster")
        {
            if (a_reenforce)
            {
                //지속 시간 동안 적들의 방어력 - 50 %
            }
        }
    }
}
