using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : CharacterController
{
    private FightSceneManager manager;
    private MapManager mapManager;

    private bool isControl;
    public bool Control
    {
        set => isControl = value;
        get => isControl;
    }

    private CharacterInfo info;

    private ParticleSystem heal;

    private PlayerData data = GameManager.Inst.Exploration.player;

    private float lastDamageTime;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Inst.player = this;
        isControl = false;

        GameObject m = GameObject.Find("MapManager");
        if (m == null || !m.TryGetComponent<MapManager>(out mapManager))
        {
            Debug.Log("PlayerController - Awake - MapManager");
        }
        GameObject pinfo = GameObject.Find("PlayerInfo");
        if (pinfo == null || !pinfo.TryGetComponent<CharacterInfo>(out info))
        {
            Debug.Log("PlayerController - Awake - PlayerInfo");
        }
        Transform effect = transform.Find("Heal");
        if (effect == null || !effect.TryGetComponent<ParticleSystem>(out heal))
        {
            Debug.Log("playerController - Awake - ParticleSystem");
        }
        else
        {
            heal.Stop();
        }
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "FightScene") 
            manager = (FightSceneManager)GameManager.Inst.manager;
        lastDamageTime = Time.time;
    }

    public void Move(Direction direction)
    {
        if (isControl && mapManager != null && GameManager.Inst.Exploration.map.Access)
        {
            mapManager.MovePlayer(direction);
        }
    }

    public void Move(Vector3 goal,float time)
    {
        transform.LeanMove(goal, time);
    }

    public void Heal()
    {
        heal.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Skill")
        {
            Skill skill;
            if (collision.transform.TryGetComponent<Skill>(out skill))
            {
                float damage = skill.GetDamage();
                Color color = Color.white;
                float DEF = data.DEF;
                switch (skill.Attribute)
                {
                    case AttributeType.None:
                        damage *= (100 / (100 + DEF));
                        break;
                    case AttributeType.Fire:
                        damage *= (100 / (100 + DEF)) * (1 - data.Fire / 100);
                        if (data.fire >= 30)
                            color = Color.gray;
                        else if (data.fire <= -30)
                            color = new Color(255, 128, 0, 1);
                        break;
                    case AttributeType.Water:
                        damage *= (100 / (100 + DEF)) * (1 - data.Water / 100);
                        if (data.water >= 30)
                            color = Color.gray;
                        else if (data.water <= -30)
                            color = new Color(255, 128, 0, 1);
                        break;
                    case AttributeType.Thunder:
                        damage *= (100 / (100 + DEF)) * (1 - data.Thunder / 100);
                        if (data.thunder >= 30)
                            color = Color.gray;
                        else if (data.thunder <= -30)
                            color = new Color(255, 128, 0, 1);
                        break;
                    case AttributeType.Earth:
                        damage *= (100 / (100 + DEF)) * (1 - data.Earth / 100);
                        if (data.earth >= 30)
                            color = Color.gray;
                        else if (data.earth <= -30)
                            color = new Color(255, 128, 0, 1);
                        break;
                }

                damage *= GameManager.Inst.Exploration.player.taken_damage_weight;

                if(damage > GameManager.Inst.Exploration.player.max_taken_damage)
                    damage = GameManager.Inst.Exploration.player.max_taken_damage;

                if (damage < 1)
                    damage = 1;
                
                if(Time.time - lastDamageTime < 0.1f)
                {
                    StartCoroutine(SetDelayBeforeTakeDamage((int)damage, color));
                }
                else
                {
                    TakeDamage((int)damage);
                    manager.ShowDamage(transform.position + new Vector3(2, 4, 0), (int)damage, color, false);
                }
                lastDamageTime = Time.time;

                if (GameManager.Inst.Exploration.CheckArtifactID(59))
                {
                    skill.User.TakeDamage(damage * 0.5f, AttributeType.None, false);
                }
            }
        }
    }

    private IEnumerator SetDelayBeforeTakeDamage(int damage, Color color)
    {
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        TakeDamage((int)damage);
        manager.ShowDamage(transform.position + new Vector3(2, 4, 0), (int)damage, color, false);
    }

    public override void TakeDamage(float damage, AttributeType attributeType, bool critical)
    {
        Color color = Color.white;
        float DEF = data.DEF;
        switch (attributeType)
        {
            case AttributeType.None:
                damage *= (100 / (100 + DEF));
                break;
            case AttributeType.Fire:
                damage *= (100 / (100 + DEF)) * (1 - data.Fire / 100);
                if (data.fire >= 30)
                    color = Color.gray;
                else if (data.fire <= -30)
                    color = new Color(255, 128, 0, 1);
                break;
            case AttributeType.Water:
                damage *= (100 / (100 + DEF)) * (1 - data.Water / 100);
                if (data.water >= 30)
                    color = Color.gray;
                else if (data.water <= -30)
                    color = new Color(255, 128, 0, 1);
                break;
            case AttributeType.Thunder:
                damage *= (100 / (100 + DEF)) * (1 - data.Thunder / 100);
                if (data.thunder >= 30)
                    color = Color.gray;
                else if (data.thunder <= -30)
                    color = new Color(255, 128, 0, 1);
                break;
            case AttributeType.Earth:
                damage *= (100 / (100 + DEF)) * (1 - data.Earth / 100);
                if (data.earth >= 30)
                    color = Color.gray;
                else if (data.earth <= -30)
                    color = new Color(255, 128, 0, 1);
                break;
        }

        damage *= GameManager.Inst.Exploration.player.taken_damage_weight;

        if (damage > GameManager.Inst.Exploration.player.max_taken_damage)
            damage = GameManager.Inst.Exploration.player.max_taken_damage;

        if (damage < 1)
            damage = 1;

        TakeDamage((int)damage);
        manager.ShowDamage(transform.position + new Vector3(2, 4, 0), (int)damage, color, false);
    }

    private void TakeDamage(int damage)
    {
        PlayAnimation(CharacterStateDC.TakeDamage);
        float hp = GameManager.Inst.Exploration.player.HP;
        hp -= damage;
        if (hp < 0f)
        {
            hp = 0f;
            isAlive = false;
        }
        else
        {
            isAlive = true;
        }
        info.SetHP(hp / GameManager.Inst.Exploration.player.MaxHP);
        GameManager.Inst.Exploration.player.HP = hp;
    }
}
