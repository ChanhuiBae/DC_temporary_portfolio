using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ArtifactManager : MonoBehaviour, IListener
{
    private FightSceneManager manager;
    public Shield shield;
    public bool shieldActive;
    private int precrtical; // -1: false, 0: none, 1: true;
    private bool usePotion = false;
    public void ResetPrecritical()
    {
        precrtical = 0;
    }

    private void Start()
    {
        GameManager.Inst.eventManager.AddListener(GameEventType.FightStart, this);
        GameManager.Inst.eventManager.AddListener(GameEventType.SkillUsed, this);
        GameManager.Inst.eventManager.AddListener(GameEventType.Heal, this);
        GameManager.Inst.eventManager.AddListener(GameEventType.PotionUsed, this);

    }

    #region Coroutine
    private Dictionary<int, Coroutine> coroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, bool> useValues = new Dictionary<int, bool>();
    public void SetUseValues(int key, bool value)
    {
        useValues[key] = value;
    }
    private void StartCoroutine(int id)
    {
        if (!useValues.ContainsKey(id))
        {
            useValues.Add(id, false);
            coroutines.Add(id, StartCoroutine(UseArtifact(id)));
        }
    }

    private void StopCoroutine(int id)
    {
        if (coroutines.ContainsKey(id))
            StopCoroutine(coroutines[id]);
        if (useValues.ContainsKey(id) && useValues[id])
        {
            MethodInfo method = this.GetType().GetMethod("Artifact" + id);
            if (method != null)
            {
                method.Invoke(this, new object[] { false });
            }
        }
        coroutines.Remove(id);
        useValues.Remove(id);
    }

    #region Check Until End Condition

    public IEnumerator UseArtifact(int id)
    {
        yield return null;
        switch (id)
        {
            // Check Using Only One
            case 8:
                StartCoroutine(CheckUseOnly(id));
                break;
            // Check Attribute
            case 13:
                StartCoroutine(CheckAttribute(id, 0));
                break;
            case 25:
                StartCoroutine(CheckAttribute(id, 1));
                break;
            case 26:
                StartCoroutine(CheckAttribute(id, 2));
                break;
            case 27:
                StartCoroutine(CheckAttribute(id, 3));
                break;
            // Check Cast Weight
            case 17:
                StartCoroutine(CheckCastWeight(id));
                break;
            // Check AllSkillType
            case 40:
                StartCoroutine(CheckAllSkillType(id));
                break;
            // Check Per HP
            case 49:
                StartCoroutine(CheckPerHP(id));
                break;
            // Check MaxHP
            case 57:
                StartCoroutine(CheckMaxHP(id));
                break;
            // Check DEF
            case 58:
                StartCoroutine(CheckDEF(id));
                break;

        }
        //StartCoroutine("UseArtifact" + id, artifact);
    }

    private IEnumerator CheckUseOnly(int id)
    {
        if (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            SetUseValues(id, true);
            ArtifactID(id, true);
            while (GameManager.Inst.Exploration.artifacts.Count == 1)
            {
                yield return null;
            }
            SetUseValues(id, false);
            ArtifactID(id, false);
            coroutines.Remove(id);
            useValues.Remove(id);
        }
    }
    private IEnumerator CheckAttribute(int id, int attribute)
    {
        while (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            SetUseValues(id, true);
            ArtifactID(id, true);
            while (GameManager.Inst.Exploration.CheckSkillAttribute(attribute))
            {
                yield return null;
            }
            SetUseValues(id, false);
            ArtifactID(id, false);
            while (!GameManager.Inst.Exploration.CheckSkillAttribute(attribute))
            {
                yield return null;
            }
        }
    }

    private IEnumerator CheckCastWeight(int id)
    {
        while (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            float castWeight = GameManager.Inst.Exploration.cast_speed;
            SetUseValues(id, true);
            ArtifactID(id, true, castWeight);
            while (castWeight == GameManager.Inst.Exploration.cast_speed)
            {
                yield return null;
            }
            SetUseValues(id, false);
            ArtifactID(id, false, castWeight);
        }
    }

    private IEnumerator CheckAllSkillType(int id)
    {
        while (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            SetUseValues(id, true);
            ArtifactID(id, true);
            while (GameManager.Inst.Exploration.CheckSkillType(SkillType.Attack)
                && GameManager.Inst.Exploration.CheckSkillType(SkillType.Buff)
                && GameManager.Inst.Exploration.CheckSkillType(SkillType.Defense))
            {
                yield return null;
            }
            SetUseValues(id, false);
            ArtifactID(id, false);
            while (!GameManager.Inst.Exploration.CheckSkillType(SkillType.Attack)
                || !GameManager.Inst.Exploration.CheckSkillType(SkillType.Buff)
                || !GameManager.Inst.Exploration.CheckSkillType(SkillType.Defense))
            {
                yield return null;
            }
        }
    }


    private IEnumerator CheckPerHP(int id)
    {
        float preHP = GameManager.Inst.Exploration.player.HP;
        if (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            SetUseValues(id, true);
            ArtifactID(id, true);
        }
        while (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            while (preHP == GameManager.Inst.Exploration.player.HP)
            {
                yield return null;
            }
            ArtifactID(id, false);
            preHP = GameManager.Inst.Exploration.player.HP;
            ArtifactID(id, true);
        }
        SetUseValues(id, false);
    }
    private IEnumerator CheckMaxHP(int id)
    {
        float preHP = GameManager.Inst.Exploration.player.MaxHP;
        if (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            SetUseValues(id, true);
            ArtifactID(id, true);
        }
        while (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            while (preHP == GameManager.Inst.Exploration.player.MaxHP)
            {
                yield return null;
            }
            ArtifactID(id, false);
            preHP = GameManager.Inst.Exploration.player.MaxHP;
            ArtifactID(id, true);
        }
        SetUseValues(id, false);
    }

    private IEnumerator CheckDEF(int id)
    {
        float preDEF = GameManager.Inst.Exploration.player.DEF;
        float delta = 0;
        SetUseValues(id, true);
        while (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            while (preDEF == GameManager.Inst.Exploration.player.DEF)
            {
                yield return null;
            }
            if(delta != 0)
                ArtifactID(id, false, -delta);
            delta += GameManager.Inst.Exploration.player.DEF - preDEF;
            ArtifactID(id, true, delta);
            preDEF = GameManager.Inst.Exploration.player.DEF;
        }
        SetUseValues(id, false);
    }
    #endregion
    #endregion
    public void CheckEachSkill(bool critical)
    {
        if (GameManager.Inst.Exploration.CheckArtifactID(44))
        {
            if (precrtical == 0)
            {
                if (critical)
                {
                    GameManager.Inst.Exploration.player.atk += 15;
                    GameManager.Inst.Exploration.player.critical += 15;
                }
            }
            else if (precrtical > 0)
            {
                if (!critical)
                {
                    GameManager.Inst.Exploration.player.atk -= 15;
                    GameManager.Inst.Exploration.player.critical -= 15;
                }
            }
            else
            {
                if (critical)
                {
                    GameManager.Inst.Exploration.player.atk += 15;
                    GameManager.Inst.Exploration.player.critical += 15;
                }
            }
        }
    }
    public void CheckAfterCritical(bool critical)
    {
        if (GameManager.Inst.Exploration.CheckArtifactID(16))
        {
            if (critical)
                GameManager.Inst.Exploration.damage_weight += 0.25f;
            else
                GameManager.Inst.Exploration.damage_weight -= 0.25f;
        }
    }
    public float CheckBeforeTakeDamage(float damage, MonsterController monster, float attribute, bool critical)
    {
        if (GameManager.Inst.Exploration.CheckArtifactID(45))
        {
            damage *= 0.55f;
            StartCoroutine(TakeDamageWithDelay(0.2f, damage, monster, attribute, critical));
        }
        return damage;
    }

    private IEnumerator TakeDamageWithDelay(float delay, float damage, MonsterController monster, float attribute, bool critical)
    {
        yield return YieldInstructionCache.WaitForSeconds(delay);
        monster.TakeDamage((int)damage, attribute, critical);
    }

    public void CheckEachBlow(bool critical, MonsterController monster)
    {
        if (GameManager.Inst.Exploration.CheckArtifactID(16))
        {
            if (critical)
                GameManager.Inst.Exploration.damage_weight -= 0.25f;
            else
                GameManager.Inst.Exploration.damage_weight += 0.25f;
        }
        if (GameManager.Inst.Exploration.CheckArtifactID(38))
        {
            GameManager.Inst.buffManager.SetAbnormalState(AbnormalStateType.Burn, monster, GameManager.Inst.PlayerData.ATK);
        }
        if (GameManager.Inst.Exploration.CheckArtifactID(41) && critical)
        {
            monster.StartCoroutine(monster.SetAtionSpeed(-0.3f, 5));
        }
        if (GameManager.Inst.Exploration.CheckArtifactID(42))
        {
            if (precrtical == 0)
            {
                if (critical)
                    GameManager.Inst.Exploration.player.criticalDamage += 30;
            }
            else if (precrtical > 0)
            {
                if (!critical)
                    GameManager.Inst.Exploration.player.criticalDamage -= 30;
            }
            else
            {
                if (critical)
                    GameManager.Inst.Exploration.player.criticalDamage += 30;
            }
        }
        if (GameManager.Inst.Exploration.CheckArtifactID(48))
        {
            UseArtifact48(true);
        }
        if (GameManager.Inst.Exploration.CheckArtifactID(53))
        {
            bool bleeding = Random.Range(0,100) < 20 ? true : false;
            if(bleeding)
                GameManager.Inst.buffManager.SetAbnormalState(AbnormalStateType.Bleeding, monster, GameManager.Inst.PlayerData.ATK);
        }
    }


    #region Event
    public void OnEvent(GameEventType eventType, object param = null)
    {
        switch (eventType)
        {
            case GameEventType.FightStart:
                if (GameManager.Inst.Exploration.CheckArtifactID(14))
                    StartCoroutine(StartFight(14));
                if (GameManager.Inst.Exploration.CheckArtifactID(15))
                    StartCoroutine(StartFight(15));
                if(GameManager.Inst.Exploration.CheckArtifactID(43))
                    StartCoroutine(StartFight(43, 10));
                if (GameManager.Inst.Exploration.CheckArtifactID(47))
                    StartCoroutine(StartFight(47, 3));
                if (GameManager.Inst.Exploration.CheckArtifactID(51))
                    StartCoroutine(StartFight(51));
                if (GameManager.Inst.Exploration.CheckArtifactID(54))
                    StartCoroutine(StartFight(54));
                if (GameManager.Inst.Exploration.CheckArtifactID(56))
                    StartCoroutine(StartFight(56));
                break;
            case GameEventType.SkillUsed:
                if (GameManager.Inst.Exploration.CheckArtifactID(39))
                    UseArtifact39(true);
                break;
            case GameEventType.PotionUsed:
                usePotion = true;
                break;
            case GameEventType.Heal:
                if (!usePotion && GameManager.Inst.Exploration.CheckArtifactID(36))
                {
                    float recoverValue = (float)param;
                    recoverValue *= 0.3f;
                    GameManager.Inst.PlayerData.HP = recoverValue;
                    GameManager.Inst.MonsterManager.GetTarget().TakeDamage(recoverValue, AttributeType.Fire, false);
                }
                usePotion = false;
                break;
        }
    }

    public void OnEvent(GameEventType eventType, object sender, object param = null)
    {
    }

    private IEnumerator StartFight(int id, int value = 0)
    {
        int delta = 0;
        int stack = 0;
        switch (id)
        {
            case 14:
                GameManager.Inst.Exploration.player.atk_weight += 1;
                while (GameManager.Inst.Exploration.TotalSkillUseCount < value)
                {
                    yield return null;
                }
                GameManager.Inst.Exploration.player.atk_weight -= 1;
                break;
            case 15:
                while (GameManager.Inst.Fight)
                {
                    yield return YieldInstructionCache.WaitForSeconds(1f); 
                    GameManager.Inst.Exploration.player.critical += 1;
                    delta++;
                }
                EndFight(id, delta);
                break;
            case 43:
                manager = (FightSceneManager)GameManager.Inst.manager;
                float preTime = Time.time;
                do
                {
                    int count = value - delta > 0 ? (int)(value - delta) : 0;
                    manager.SetArtifactValue(id, count);
                    yield return YieldInstructionCache.WaitForSeconds(1);
                    delta = (int)(Time.time - preTime);
                    if (!shieldActive && delta >= value)
                    {
                        shield.gameObject.SetActive(true);
                        shield.SetShield();
                        preTime = Time.time;
                        delta = 0;
                    }
                }
                while (GameManager.Inst.Fight);
                EndFight(id);
                break;
            case 47:
                    manager = (FightSceneManager)GameManager.Inst.manager;
                    manager.SetArtifactValue(id, 0);
                    stack = 0;
                    while (GameManager.Inst.Fight)
                    {
                        if (GameManager.Inst.Exploration.TotalSkillUseCount % value == stack)
                        {
                            yield return null;
                        }
                        else
                        {
                            stack = GameManager.Inst.Exploration.TotalSkillUseCount % value;
                            manager.SetArtifactValue(id, stack);
                            if (stack == value - 1)
                                GameManager.Inst.Exploration.cast_length -= 1;
                            else if (stack == 0)
                            {
                                if (GameManager.Inst.Exploration.cast_length < 1)
                                    GameManager.Inst.Exploration.cast_length += 1;
                            }
                        }
                    }
                    if (GameManager.Inst.Exploration.cast_length < 1)
                        GameManager.Inst.Exploration.cast_length += 1;
                break;
            case 51:
                while (GameManager.Inst.Fight)
                {
                    GameManager.Inst.Exploration.player.HP -= GameManager.Inst.Exploration.player.MaxHP * 0.02f;
                    GameManager.Inst.Exploration.player.HP += GameManager.Inst.Exploration.player.MaxHP * 0.01f * GameManager.Inst.buffManager.GetAbnormalCount();
                }
                break;
            case 54:
                stack = 0;
                float preHP = GameManager.Inst.Exploration.player.HP;
                while (GameManager.Inst.Fight)
                {
                    yield return null;
                    if (GameManager.Inst.Exploration.player.HP - preHP != delta)
                    {
                        delta = (int)(preHP - GameManager.Inst.Exploration.player.HP);
                        preHP = GameManager.Inst.Exploration.player.HP;
                        if (delta > 0)
                        {
                            GameManager.Inst.Exploration.player.criticalDamage += delta;
                            stack += delta;
                        }
                    }
                }
                EndFight(id, stack);
                break;
            case 56:
                while (GameManager.Inst.Fight)
                {
                    while (!GameManager.Inst.player.GetShield())
                    {
                        yield return null;
                    }
                    GameManager.Inst.Exploration.player.atk_weight += 0.2f;
                    while (GameManager.Inst.player.GetShield())
                    {
                        yield return null;
                        if (!GameManager.Inst.Fight)
                            break;
                    }
                    GameManager.Inst.Exploration.player.atk_weight -= 0.2f;
                }
                break;
        }
    }

    private void EndFight(int id, float value = 0)
    {
        switch (id)
        {
            case 15:
                GameManager.Inst.Exploration.player.critical -= (int)value;
                break;
            case 43:
                shield.gameObject.SetActive(false);
                shieldActive = false;
                break;
            case 54:
                GameManager.Inst.Exploration.player.criticalDamage -= (int)value;
                break;
        }
    }
    
    #endregion

    #region Equip or not
    public void EquipArtifact(Artifact artifact)
    {
        MethodInfo method = this.GetType().GetMethod("UseArtifact" + artifact.GetID(true));
        if (method != null)
        {
            method.Invoke(this, new object[] { true });
        }
    }

    public void UneuipArtifact(Artifact artifact)
    {
        MethodInfo method = this.GetType().GetMethod("UseArtifact" + artifact.GetID(true));
        if (method != null)
        {
            method.Invoke(this, new object[] { false });
        }
    }
    private void ArtifactID(int id, bool use)
    {
        MethodInfo method = this.GetType().GetMethod("Artifact" + id);
        if (method != null)
        {
            method.Invoke(this, new object[] { use });
        }
    }
    private void ArtifactID(int id, bool use, float value)
    {
        MethodInfo method = this.GetType().GetMethod("Artifact" + id);
        if (method != null)
        {
            method.Invoke(this, new object[] { use, value });
        }
    }

    public void UseArtifact1(bool equip)
    {
        if (equip)
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.None, 0.1f);
        else
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.None, -0.1f);
    }

    public void UseArtifact2(bool equip)
    {
        if (equip)
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Fire, 0.1f);
        else
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Fire, -0.1f);
    }
    public void UseArtifact3(bool equip)
    {
        if (equip)
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Water, 0.1f);
        else
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Water, -0.1f);
    }
    public void UseArtifact4(bool equip)
    {
        if (equip)
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Thunder, 0.1f);
        else
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Thunder, -0.1f);
    }
    public void UseArtifact5(bool equip)
    {
        if (equip)
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Earth, 0.1f);
        else
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Earth, -0.1f);
    }
    public void UseArtifact6(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.MaxHP += 20;
            GameManager.Inst.Exploration.player.HP += 20;
        }
        else
        {
            GameManager.Inst.Exploration.player.MaxHP -= 20;
            GameManager.Inst.Exploration.player.HP += 0;
        }
    }
    public void UseArtifact7(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.def += 10;
        }
        else
        {
            GameManager.Inst.Exploration.player.def -= 10;
        }
    }

    public void UseArtifact8(bool equip)
    {
        if (equip)
        {
            if (GameManager.Inst.Exploration.artifacts.Count == 1)
            {
                StartCoroutine(8);
            }
        }
    }

    public void Artifact8(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.atk += 5;
            GameManager.Inst.Exploration.player.def += 5;
            GameManager.Inst.Exploration.player.MaxHP += 10;
            GameManager.Inst.Exploration.player.HP += 10;
            GameManager.Inst.Exploration.player.critical += 5;
            GameManager.Inst.Exploration.player.criticalDamage += 10;
        }
        else
        {
            GameManager.Inst.Exploration.player.atk -= 5;
            GameManager.Inst.Exploration.player.def -= 5;
            GameManager.Inst.Exploration.player.MaxHP -= 10;
            GameManager.Inst.Exploration.player.HP -= 10;
            GameManager.Inst.Exploration.player.critical -= 5;
            GameManager.Inst.Exploration.player.criticalDamage -= 10;
        }
    }

    public void UseArtifact9(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.atk += 10;
        }
        else
        {
            GameManager.Inst.Exploration.player.atk -= 10;
        }
    }

    public void UseArtifact10(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.critical += 10;
        }
        else
        {
            GameManager.Inst.Exploration.player.critical -= 10;
        }
    }

    public void UseArtifact11(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.criticalDamage += 20;
        }
        else
        {
            GameManager.Inst.Exploration.player.criticalDamage -= 20;
        }
    }

    public void UseArtifact12(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.cast_speed += 0.1f;
        }
        else
        {
            GameManager.Inst.Exploration.cast_speed -= 0.1f;
        }
    }

    public void UseArtifact13(bool equip)
    {
        if (equip)
        {
            if (GameManager.Inst.Exploration.CheckSkillAttribute(0)) // normal check
            {
                StartCoroutine(13);
            }
        }
        else
        {
            StopCoroutine(13);
        }
    }

    public void Artifact13(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.None, 0.3f);
        }
        else
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.None, -0.3f);
        }
    }
    public void UseArtifact15(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.critical -= 10;
        }
        else
        {
            GameManager.Inst.Exploration.player.critical += 10;
        }
    }

    public void UseArtifact17(bool equip)
    {
        if (equip)
        {
            StartCoroutine(17);
        }
        else
        {
            StopCoroutine(17);
        }
    }
    public void Artifact17(bool equip, float value)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Fire, value);
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Water, value);
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Thunder, value);
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Earth, value);
        }
        else
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Fire, -value);
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Water, -value);
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Thunder, -value);
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Earth, -value);
        }
    }

    public void UseArtifact25(bool equip)
    {
        if (equip)
        {
            if (GameManager.Inst.Exploration.CheckSkillAttribute(1)) // fire check
            {
                StartCoroutine(25);
            }
        }
        else
        {
            StartCoroutine(25);
        }
    }

    public void Artifact25(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Fire, 0.3f);
        }
        else
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Fire, -0.3f);
        }
    }

    public void UseArtifact26(bool equip)
    {
        if (equip)
        {
            if (GameManager.Inst.Exploration.CheckSkillAttribute(2)) // water check
            {
                StartCoroutine(26);
            }
        }
        else
        {
            StartCoroutine(26);
        }
    }
    public void Artifact26(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Water, 0.3f);
        }
        else
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Water, -0.3f);
        }
    }

    public void UseArtifact27(bool equip)
    {
        if (equip)
        {
            if (GameManager.Inst.Exploration.CheckSkillAttribute(3)) // thunder check
            {
                StartCoroutine(27);
            }
        }
        else
        {
            StartCoroutine(27);
        }
    }

    public void Artifact27(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Thunder, 0.3f);
        }
        else
        {
            GameManager.Inst.Exploration.SetDamageMultiplier(AttributeType.Thunder, -0.3f);
        }
    }

    public void UseArtifact39(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.buffManager.SetBuff(BuffType.HighSpeedCasting, true, GameManager.Inst.player);
        }
        else
        {
            GameManager.Inst.buffManager.SetBuff(BuffType.HighSpeedCasting, false, GameManager.Inst.player);
        }
    }
    public void UseArtifact40(bool equip)
    {
        if (equip)
        {
            StartCoroutine(40);
        }
        else
        {
            StopCoroutine(40);
        }
    }

    public void Artifact40(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.damage_weight += 0.2f;
            GameManager.Inst.Exploration.cast_speed += 0.1f;
        }
        else
        {
            GameManager.Inst.Exploration.damage_weight -= 0.2f;
            GameManager.Inst.Exploration.cast_speed -= 0.1f;
        }
    }

    public void UseArtifact48(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.buffManager.SetBuff(BuffType.Stead, true, GameManager.Inst.player);
        }
        else
        {
            GameManager.Inst.buffManager.SetBuff(BuffType.Stead, false, GameManager.Inst.player);
        }
    }

    public void UseArtifact49(bool equip)
    {
        if (equip)
        {
            StartCoroutine(49);
        }
        else
        {
            StopCoroutine(49);
        }
    }

    public void Artifact49(bool equip)
    {
        float count = GameManager.Inst.Exploration.player.MaxHP - GameManager.Inst.Exploration.player.HP;
        count /= 2;
        if (equip)
        {
            GameManager.Inst.Exploration.player.def += 1 * (int)count;
            GameManager.Inst.Exploration.player.taken_damage_weight -= 0.01f * count;
        }
        else
        {
            GameManager.Inst.Exploration.player.def -= 1 * (int)count;
            GameManager.Inst.Exploration.player.taken_damage_weight += 0.01f * count;
        }
    }

    public void UseArtifact50(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.atk_weight += 0.6f;
            GameManager.Inst.Exploration.miss += 30;
        }
        else
        {
            GameManager.Inst.Exploration.player.atk_weight -= 0.6f;
            GameManager.Inst.Exploration.miss -= 30;
        }
    }
    public void UseArtifact52(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.atk_weight += 0.3f;
            GameManager.Inst.Exploration.player.heal_weight -= 0.5f;
        }
        else
        {
            GameManager.Inst.Exploration.player.atk_weight -= 0.3f;
            GameManager.Inst.Exploration.player.heal_weight += 0.5f;
        }
    }

    public void UseArtifact55(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.buffManager.SetBuff(BuffType.RedMoon, true, GameManager.Inst.player);
        }
    }
    public void UseArtifact57(bool equip)
    {
        if (equip)
        {
            StartCoroutine(57);
        }
        else
        {
            StopCoroutine(57);
        }
    }

    public void Artifact57(bool equip)
    {
        if (equip)
        {
            GameManager.Inst.Exploration.player.max_taken_damage = GameManager.Inst.Exploration.player.MaxHP * 0.2f;
        }
        else
        {
            GameManager.Inst.Exploration.player.max_taken_damage = GameManager.Inst.Exploration.player.MaxHP;
        }
    }

    public void UseArtifact58(bool equip)
    {
        if (equip)
        {
            StartCoroutine(58);
        }
        else
        {
            StopCoroutine(58);
        }
    }

    public void Artifact58(bool equip, float value)
    {
        GameManager.Inst.Exploration.player.criticalDamage += (int)value;
    }

    #endregion
}

