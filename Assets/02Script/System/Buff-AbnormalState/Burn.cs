using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : AbnormalState
{
    protected override void Awake()
    {
        maxStack = 6;
        fullTime = 3;
        timedelay = 1;
    }

    protected virtual IEnumerator PlayAbnormalState(CharacterController character, float atk)
    {
        stack[character]++;
        float time = Time.time;
        while (Time.time < time + fullTime)
        {
            character.TakeDamage(0.05f * atk * stack[character], AttributeType.Fire, false);
            yield return YieldInstructionCache.WaitForSeconds(timedelay);
        }
        stack[character] = 0;
    }
}
