using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stead : Buff
{
    private FightSceneManager manager;
    private int target = 0;

    protected virtual void Awake()
    {
        maxStack = 10;
    }

    public override void StartBuff(bool use, CharacterController character)
    {
        if (coroutines.ContainsKey(character))
        {
            if (use)
            {   
                if(coroutines[character] != null)
                    StopCoroutine(coroutines[character]); 
                coroutines[character] = StartCoroutine(PlayBuff(character));
            }
            else
            {
                if (coroutines[character] != null)
                    StopCoroutine(coroutines[character]);
                GameManager.Inst.Exploration.player.atk_weight -= stack[character] * 0.02f; 
                stack[character] = 0;
                coroutines.Remove(character);
                stack.Remove(character);
            }
        }
        else
        {
            if (use)
            {
                stack.Add(character, 0);
                coroutines.Add(character, StartCoroutine(PlayBuff(character)));
            }
        }
    }

    protected override IEnumerator PlayBuff(CharacterController character)
    {
        stack[character]++;
        if (stack[character] > maxStack)
            stack[character] = maxStack;
        if (stack[character] <= maxStack)
        {
            GameManager.Inst.Exploration.player.atk_weight += 0.02f;
        }
        manager = (FightSceneManager)GameManager.Inst.manager;
        manager.SetArtifactValue(48, stack[character]);
        if (target == 0)
            target = GameManager.Inst.MonsterManager.Target;
        if (stack[character] == maxStack)
        {
            Debug.Log("Attack");
        }
        else
        {
            while (target == GameManager.Inst.MonsterManager.Target)
            {
                yield return null;
            }
            target = GameManager.Inst.MonsterManager.Target; 
        }
        GameManager.Inst.Exploration.player.atk_weight -= stack[character] * 0.02f;
        stack[character] = 0;
    }
}
