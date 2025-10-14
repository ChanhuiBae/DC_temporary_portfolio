using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedCasting : Buff
{
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
                if (coroutines[character] != null)
                    StopCoroutine(coroutines[character]);
                coroutines[character] = StartCoroutine(PlayBuff(character));
            }
            else
            {
                if (coroutines[character] != null)
                    StopCoroutine(coroutines[character]);
                GameManager.Inst.Exploration.cast_speed -= stack[character] * 0.05f;
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
        float time = Time.time;
        if (stack[character] <= maxStack)
        {
            GameManager.Inst.Exploration.cast_speed += 0.05f;
        }
        while(GameManager.Inst.Exploration.SkillQueueCount > 0 || GameManager.Inst.Exploration.UsingSkill)
        {
            yield return null;
        }
        GameManager.Inst.Exploration.cast_speed -= stack[character] * 0.05f;
        stack[character] = 0;
    }
}
