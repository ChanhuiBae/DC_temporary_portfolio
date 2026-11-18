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

    #region Artifact condition monitors
    // 아티팩트마다 실제 실행 핸들과 실제로 적용한 값을 함께 보관합니다.
    private class ArtifactMonitor
    {
        public Coroutine coroutine;
        public bool applied;
        public float value;
        public int initialDEF;
    }

    private readonly Dictionary<int, ArtifactMonitor> monitors
        = new Dictionary<int, ArtifactMonitor>();

    private void StartArtifactMonitor(int id)
    {
        // 중복 장착 알림으로 감시 코루틴이 여러 개 실행되지 않도록 합니다.
        if (monitors.ContainsKey(id))
            return;

        ArtifactMonitor monitor = new ArtifactMonitor();
        monitor.initialDEF = GameManager.Inst.Exploration.player.DEF;
        monitors.Add(id, monitor);

        // 자식 코루틴을 따로 시작하지 않고 실제 감시 루프의 핸들을 저장합니다.
        monitor.coroutine = StartCoroutine(MonitorArtifact(id, monitor));
    }

    private void StopArtifactMonitor(int id)
    {
        ArtifactMonitor monitor;
        if (!monitors.TryGetValue(id, out monitor))
            return;

        if (monitor.coroutine != null)
            StopCoroutine(monitor.coroutine);

        // StopCoroutine만으로는 스탯이 원상 복구되지 않으므로 명시적으로 해제합니다.
        RemoveMonitoredEffect(id, monitor);
        monitors.Remove(id);
    }

    private IEnumerator MonitorArtifact(int id, ArtifactMonitor monitor)
    {
        // 장착 목록 갱신 및 코루틴 핸들 저장을 마친 뒤 조건을 확인합니다.
        yield return null;

        while (GameManager.Inst.Exploration.CheckArtifactID(id))
        {
            float value;
            bool shouldApply = GetMonitorCondition(id, monitor, out value);

            // 조건이 해제되거나 적용 기준값이 변했으면 이전 효과부터 제거합니다.
            if (monitor.applied && (!shouldApply || monitor.value != value))
                RemoveMonitoredEffect(id, monitor);

            if (shouldApply && !monitor.applied)
            {
                monitor.value = value;
                ApplyMonitoredEffect(id, true, value);
                monitor.applied = true;
            }

            // 모든 대기는 이 한 곳에서 수행하므로 매 프레임 장착 여부도 확인합니다.
            yield return null;
        }

        // 명시적인 해제 호출 없이 장착 목록에서 빠졌을 때도 정리합니다.
        RemoveMonitoredEffect(id, monitor);
        monitors.Remove(id);
    }

    private bool GetMonitorCondition(int id, ArtifactMonitor monitor, out float value)
    {
        var exploration = GameManager.Inst.Exploration;
        value = 0f;

        switch (id)
        {
            case 8:
                return exploration.artifacts.Count == 1;
            case 13:
                return exploration.CheckSkillAttribute(0);
            case 25:
                return exploration.CheckSkillAttribute(1);
            case 26:
                return exploration.CheckSkillAttribute(2);
            case 27:
                return exploration.CheckSkillAttribute(3);
            case 17:
                value = exploration.cast_speed;
                return true;
            case 40:
                return exploration.CheckSkillType(SkillType.Attack)
                    && exploration.CheckSkillType(SkillType.Buff)
                    && exploration.CheckSkillType(SkillType.Defense);
            case 49:
                // HP가 달라져도 해제할 때 이때 적용한 값을 그대로 사용합니다.
                value = (exploration.player.MaxHP - exploration.player.HP) / 2f;
                return true;
            case 57:
                value = exploration.player.MaxHP;
                return true;
            case 58:
                // 기존 기획 유지: 장착 시점 이후의 방어력 변화량을 반영합니다.
                value = exploration.player.DEF - monitor.initialDEF;
                return true;
            default:
                return false;
        }
    }

    private void RemoveMonitoredEffect(int id, ArtifactMonitor monitor)
    {
        if (!monitor.applied)
            return;

        // 중복 해제 시 같은 스탯을 두 번 차감하지 않습니다.
        monitor.applied = false;
        ApplyMonitoredEffect(id, false, monitor.value);
    }

    private void ApplyMonitoredEffect(int id, bool apply, float value)
    {
        var player = GameManager.Inst.Exploration.player;
        switch (id)
        {
            case 8: Artifact8(apply); break;
            case 13: Artifact13(apply); break;
            case 25: Artifact25(apply); break;
            case 26: Artifact26(apply); break;
            case 27: Artifact27(apply); break;
            case 17:
                // Artifact17은 bool과 float 두 인수가 모두 필요합니다.
                Artifact17(apply, value);
                break;
            case 40: Artifact40(apply); break;
            case 49:
                // 해제 시 현재 HP로 다시 계산하면 이전 증가분이 남을 수 있습니다.
                // 감시 상태에 저장한 적용 당시의 값으로 정확히 되돌립니다.
                if (apply)
                {
                    player.def += (int)value;
                    player.taken_damage_weight -= 0.01f * value;
                }
                else
                {
                    player.def -= (int)value;
                    player.taken_damage_weight += 0.01f * value;
                }
                break;
            case 57:
                player.max_taken_damage = apply ? value * 0.2f : player.MaxHP;
                break;
            case 58:
                // 기존 Artifact58은 bool을 사용하지 않고 전달값을 더합니다.
                Artifact58(apply, apply ? value : -value);
                break;
        }
    }
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
            bool bleeding = Random.Range(0, 100) < 20 ? true : false;
            if (bleeding)
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
                if (GameManager.Inst.Exploration.CheckArtifactID(43))
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
                    yield return YieldInstructionCache.WaitForSeconds(0.1f);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(8);
        else
            StopArtifactMonitor(8);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(13);
        else
            StopArtifactMonitor(13);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(17);
        else
            StopArtifactMonitor(17);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(25);
        else
            StopArtifactMonitor(25);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(26);
        else
            StopArtifactMonitor(26);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(27);
        else
            StopArtifactMonitor(27);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(40);
        else
            StopArtifactMonitor(40);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(49);
        else
            StopArtifactMonitor(49);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(57);
        else
            StopArtifactMonitor(57);
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
        // 조건이 아직 맞지 않아도 감시하며, 이후 조건을 만족하면 적용합니다.
        if (equip)
            StartArtifactMonitor(58);
        else
            StopArtifactMonitor(58);
    }

    public void Artifact58(bool equip, float value)
    {
        GameManager.Inst.Exploration.player.criticalDamage += (int)value;
    }

    #endregion
}

