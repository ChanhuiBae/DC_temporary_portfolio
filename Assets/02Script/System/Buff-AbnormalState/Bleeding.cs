using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleeding : AbnormalState
{
    protected override void Awake()
    {
        maxStack = 10;
        timedelay = 1;
    }

    protected override IEnumerator PlayAbnormalState(CharacterController character, float atk)
    {
        stack[character]++;
        while (character.IsAlive)
        {
            yield return YieldInstructionCache.WaitForSeconds(timedelay);
            character.TakeDamage(1, AttributeType.None, false);
        }
        stack[character] = 0;
    }
}
