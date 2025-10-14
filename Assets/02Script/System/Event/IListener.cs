using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEventType
{
    FightStart,
    Heal,
    PotionUsed,
    SkillUsed,
    MonsterDie,
    MonsterTakeDamage,
}


public interface IListener
{
    // When event occured, a listener call this function.
    void OnEvent(GameEventType eventType, object param = null);
    void OnEvent(GameEventType eventType, object sender, object param = null);
}
