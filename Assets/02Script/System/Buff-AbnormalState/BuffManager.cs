using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BuffManager : MonoBehaviour
{
    // Buff
    private HighSpeedCasting casting;
    private Stead stead;
    private RedMoon redMoon;

    // Debuff
    private Burn burn;
    private Bleeding bleeding;


    private void Awake()
    {
        if(!TryGetComponent<HighSpeedCasting>(out casting))
        {
            Debug.Log("BuffManager - Awake - HighSpeedCasting");
        }
        if (!TryGetComponent<Stead>(out stead))
        {
            Debug.Log("BuffManager - Awake - Stead");
        }
        if (!TryGetComponent<RedMoon>(out redMoon))
        {
            Debug.Log("BuffManager - Awake - RedMoon");
        }
        if (!TryGetComponent<Burn>(out burn))
        {
            Debug.Log("BuffManager - Awake - Burn");
        }
        if (!TryGetComponent<Bleeding>(out bleeding))
        {
            Debug.Log("BuffManager - Awake - Bleeding");
        }
        GameManager.Inst.buffManager = this;
    }

    public void SetBuff(BuffType type, bool use, CharacterController character)
    {
        switch (type)
        {
            case BuffType.HighSpeedCasting:
                casting.StartBuff(use, character);
                break;
            case BuffType.Stead:
                stead.StartBuff(use, character);
                break;
            case BuffType.RedMoon:
                redMoon.StartBuff(use, character);
                break;
        }
    }

    public void SetAbnormalState(AbnormalStateType type, CharacterController character, float atk)
    {
        switch (type)
        {
            case AbnormalStateType.Burn:
                burn.StartAbnormalState(character, atk);
                break;
            case AbnormalStateType.Bleeding:
                bleeding.StartAbnormalState(character, atk);
                break;
        }
    }

    public int GetAbnormalCount()
    {
        return burn.GetCount();
    }
}
