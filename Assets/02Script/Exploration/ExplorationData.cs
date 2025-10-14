using System.Collections.Generic;
using System.Linq;

public enum SatietyType
{
    Overeating = 91,
    Full = 70,
    Normal = 1,
    Hunger = 0
}

public enum FatigueType
{
    Healthy = 75,
    Average = 50,
    Tired = 25,
    Dizziness = 1,
    Almostfainting = 0
}

public enum LightType
{
    Intensity = 0,
    Weakness = 15,
    Faint = 30,
    None = 50
}

public enum SearchEventType
{
    None = 50,
    Done = 0,
    FindSecrect = 1,
    Chest = 30,
    Monster = 20
}
public class ExplorationData
{
    private int floor;
    public int Floor
    {
        set => floor = value;
        get => floor;
    }


    public Map map;
    private Entity_Squad squad;
    public bool squadClear;
    public void SetSquadID(TileType type)
    {
        List<Entity_Squad> list;
        switch (type)
        {
            case TileType.Boss:
                GameManager.Inst.GetSquadDatas(2, floor, out list);
                squad = list[UnityEngine.Random.Range(0, list.Count)];
                squadClear = false;
                break;
            case TileType.MiddleBoss:
                GameManager.Inst.GetSquadDatas(1, floor, out list);
                squad = list[UnityEngine.Random.Range(0, list.Count)];
                squadClear = false;
                break;
            case TileType.Monster:
                GameManager.Inst.GetSquadDatas(0, floor, out list);
                squad = list[UnityEngine.Random.Range(0, list.Count)];
                squadClear = false;
                break;
            default:
                squad = null;
                squadClear = false;
                break;
        }
    }

    public Entity_Squad GetSquad()
    {
        if (squad != null)
            return squad;
        return null;
    }

    public Entity_Monster GetBoss()
    {
        if (squad != null && squad.Type == 2)
        {
            Entity_SquadMember sm;
            if (GameManager.Inst.GetSquadMemberData(squad.ID, out sm))
            {
                Entity_Monster boss;
                if (GameManager.Inst.GetMonsterData(sm.MonsterID, out boss))
                {
                    return boss;
                }
            }
        }
        return null;
    }
    public PlayerData player;
    public float cast_length = 1;
    public float cast_speed = 1;
    public int miss = 0; // 0~100;
    public float damage_weight = 1;
    private float damage_normal = 1;
    private float damage_fire = 1;
    private float damage_water = 1;
    private float damage_thunder = 1;
    private float damage_earth = 1;

    public void SetDamageMultiplier(AttributeType type, float value)
    {
        switch (type)
        {
            case AttributeType.Fire:
                damage_fire += value;
                break;
            case AttributeType.Water:
                damage_water += value; 
                break;
            case AttributeType.Thunder:
                damage_thunder += value;
                break;
            case AttributeType.Earth:
                damage_earth += value;
                break;
            default:
                damage_normal += value;
                break;
        }
    }
    public float GetDamageMultiplier(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.Fire:
                return damage_fire;
            case AttributeType.Water:
                return damage_water;
            case AttributeType.Thunder:
                return damage_thunder;
            case AttributeType.Earth:
                return damage_earth;
            default:
                return damage_normal;
        }
    }
    private SatietyType satietyType;
    private SatietyType SetSatietyType
    {
        set 
        {
            if(satietyType == value) 
                return;

            switch (satietyType)
            {
                case SatietyType.Overeating:
                    cast_speed -= 0.25f;
                    break;
                case SatietyType.Full:
                    player.atk_weight -= 0.1f;
                    player.def_weight -= 0.1f;
                    cast_speed += 0.1f;
                    break;

            }
            satietyType = value;
            switch (satietyType)
            {
                case SatietyType.Overeating:
                    cast_speed += 0.25f;
                    break;
                case SatietyType.Full:
                    player.atk_weight += 0.1f;
                    player.def_weight += 0.1f;
                    cast_speed -= 0.1f;
                    break;
            }
        }
    }
   
    private int satiety;
    public int Satiety
    {
        get => satiety;
        set
        {
            if (satiety == 0 && value < 0)
                player.HP += value;

            satiety = value;
            if (satiety > 100)
                satiety = 100;
            else if (satiety < 0)
                satiety = 0;

            if (satiety > 90)
                SetSatietyType = SatietyType.Overeating;
            else if (satiety > 69)
                SetSatietyType = SatietyType.Full;
            else if (satiety == 0)
                SetSatietyType = SatietyType.Normal;
            else
                SetSatietyType = SatietyType.Hunger;
        }
    }


    private FatigueType fatigueType;
    private FatigueType SetFatigueType
    {
        set
        {
            if (fatigueType == value)
                return;

            switch (fatigueType)
            {
                case FatigueType.Healthy:
                    player.atk_weight -= 0.15f;
                    player.def_weight -= 0.15f;
                    cast_speed += 0.15f;
                    break;
                case FatigueType.Tired:
                    player.atk_weight += 0.15f;
                    player.def_weight += 0.15f;
                    cast_speed -= 0.15f;
                    break;
                case FatigueType.Dizziness:
                    player.atk_weight += 0.3f;
                    player.def_weight += 0.3f;
                    cast_speed -= 0.3f;
                    break;
                case FatigueType.Almostfainting:
                    player.atk_weight += 0.5f;
                    player.def_weight += 0.5f;
                    cast_speed -= 0.5f;
                    break;
            }
            fatigueType = value;
            switch (fatigueType)
            {
                case FatigueType.Healthy:
                    player.atk_weight += 0.15f;
                    player.def_weight += 0.15f;
                    cast_speed -= 0.15f;
                    break;
                case FatigueType.Tired:
                    player.atk_weight -= 0.15f;
                    player.def_weight -= 0.15f;
                    cast_speed += 0.15f;
                    break;
                case FatigueType.Dizziness:
                    player.atk_weight -= 0.3f;
                    player.def_weight -= 0.3f;
                    cast_speed += 0.3f;
                    break;
                case FatigueType.Almostfainting:
                    player.atk_weight -= 0.5f;
                    player.def_weight -= 0.5f;
                    cast_speed += 0.5f;
                    break;
            }
        }
    }
    private int fatigue;
    public int Fatigue
    {
        get => fatigue;
        set
        {
            fatigue = value;
            if (fatigue > 100)
                fatigue = 100;
            else if (fatigue < 0)
                fatigue = 0;

            if (fatigue == 0)
                SetFatigueType = FatigueType.Almostfainting;
            else if (fatigue < 25)
                SetFatigueType = FatigueType.Dizziness;
            else if (fatigue < 50)
                SetFatigueType = FatigueType.Tired;
            else if (fatigue < 75)
                SetFatigueType = FatigueType.Average;
            else
                SetFatigueType = FatigueType.Healthy;

        }
    }
    private float monster_weight = 1;
    public float MonsterWeight
    {
        get => monster_weight;
    }

    private LightType light;
    public LightType Light
    {
        set
        {
            if(light == value)
                return;

            switch (light)
            {
                case LightType.Weakness:
                    monster_weight -= 0.15f;
                    break;
                case LightType.Faint:
                    monster_weight -= 0.3f;
                    break;
                case LightType.None:
                    monster_weight -= 0.5f;
                    break;
            }
            light = value;
            switch (light)
            {
                case LightType.Weakness:
                    monster_weight += 0.15f;
                    break;
                case LightType.Faint:
                    monster_weight += 0.3f;
                    break;
                case LightType.None:
                    monster_weight += 0.5f;
                    break;
            }
        }

        get => light;
    }
    private int lantern;
    public int Lantern
    {
        get => lantern;
        set
        {
            lantern = value;
            if (lantern > 150)
                lantern = 150;
            else if(lantern < 0)
                lantern = 0;

            if (lantern == 0)
                Light = LightType.None;
            else if (lantern < 51)
                Light = LightType.Faint;
            else if (lantern < 101)
                Light = LightType.Weakness;
            else
                Light = LightType.Intensity;
        }
    }

    private int allPearl;

    public int AllPearl
    {
        get => allPearl;
    }
    public void PlusPearl(int value)
    {
        player.Pearl += value;
        allPearl += value;
    }

    public void MinusPearl(int value)
    {
        player.Pearl -= value;
    }

    public Dictionary<int, int> skills; // key = index
    public List<Bubble> bubbles; // having artifact
    public Dictionary<int, Artifact> artifacts; // equiped artifact

    public bool CheckAddArtifact()
    {
        if (bubbles.Count < 15)
            return true;
        return false;
    }

    private void AddBase()
    {
        for(int i = 1; i < 6; i++)
        {
            skills.Add(i, i);
            skillUseCount.Add(i, 0);
        }
    }

    public bool CheckArtifactID(int id)
    {
        for(int i = 1; i < 6; i++)
        {
            if (artifacts.ContainsKey(i) && artifacts[i].GetID(true) == id)
            {
                return true;
            }
        }
        return false;
    }

    public bool SetArtifact(int index, int id) //index : 1~5
    {
        if (index > 0 && index < 6)
        {
            ExplorationManager manager = (ExplorationManager)GameManager.Inst.manager;
            if (manager.SetArtifact(index, id))
            {

                if (artifacts.ContainsKey(index))
                {
                    if (artifacts[index].GetID(true) != id)
                    {
                        artifacts[index].UneuipArtifact();
                        artifacts[index].SetArtifact(id);
                    }
                }
                else
                {
                    artifacts.Add(index, new Artifact(id));
                }
                artifacts[index].EquipArtifact();
                return true;    
            }
        }
        return false;
    }


    public void SetSkill(int index, int id) //index : 1~5
    {
        if(index > 0 && index < 6)
        {
            int preID = skills[index];
            skillUseCount.Remove(preID);
            skills[index] = id;
            ExplorationManager manager = (ExplorationManager)GameManager.Inst.manager;
            manager.SetSkill(index, id);
            skillUseCount.Add(id, 0);
        }
    }

    public bool CheckSkillAttribute(int attribute)
    {
        for (int i = 1; i < skills.Count+1; i++)
        {
            Entity_Skill skill;
            GameManager.Inst.GetSkillData(skills[i], out skill);
            if (skill.Attribute != attribute)
                return false;
        }
        return true;
    }

    public bool CheckSkillType(SkillType type)
    {
        for(int i = 1; i < skills.Count+1; i++)
        {
            Entity_Skill skill;
            GameManager.Inst.GetSkillData(skills[i], out skill);
            if(skill.Type == (int)type)
                return true;
        }
        return false;
    }

    private int totalSkillUseCount;
    public int TotalSkillUseCount
    {
        get => totalSkillUseCount;
    }
    private Dictionary<int, int> skillUseCount = new Dictionary<int, int>(); // key = id , value = use count
    public void SetSkillUseCount(int id)
    {
        skillUseCount[id]++;
        totalSkillUseCount++;
    }
    public int GetSkillUseCount(int id)
    {
        return skillUseCount[id];
    }
    public void InitSkillUseCount()
    {
        totalSkillUseCount = 0;
        for(int i =0; i < skillUseCount.Count; i++)
        {
            skillUseCount[skillUseCount.ElementAt(i).Key] = 0;
        }
    }

    private int skillQueueCount;
    public int SkillQueueCount
    {
        get => skillQueueCount;
        set => skillQueueCount = value;
    }

    private bool usingSkill;
    public bool UsingSkill
    {
        get => usingSkill;
        set => usingSkill = value;
    }

    public CharacterData partner;
    public int partnerMax;
    private int partnerValue;
    public int PartnerValue
    {
        get => partnerValue;
        set
        {
            partnerValue = value;
            if (partnerValue > partnerMax)
                partnerValue = partnerMax;
            else if (partnerValue < 0)
                partnerValue = 0;
        }
    }
    #region exploration data
    public ExplorationData(PlayerData data)
    {
        floor = 1; 
        player = new PlayerData(GameManager.Inst.IDMaker);
        player.Copy(data);
        allPearl = 0;
        Satiety = 90;
        Fatigue = 100;
        lantern = 150;
        partner = new CharacterData();
        partnerMax = 100;
        bubbles = new List<Bubble>();
        skills = new Dictionary<int, int>();
        artifacts = new Dictionary<int, Artifact>();
        AddBase();
    }
    #endregion
}
