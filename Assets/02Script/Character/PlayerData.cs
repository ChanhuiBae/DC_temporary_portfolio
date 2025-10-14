using System.Collections.Generic;
using System.Diagnostics;

public class PlayerData: CharacterData
{
    public override float HP
    {
        get => currentHP;
        set
        {
            if (currentHP < value)
            {
                float delta = (value - currentHP) * heal_weight;
                base.HP = currentHP + delta;
                GameManager.Inst.eventManager.PostNotification(GameEventType.Heal, delta);
                GameManager.Inst.player.Heal();
            }
            else
                base.HP = value;
            GameManager.Inst.manager.SetHP();
        }
    }

    public float atk_weight = 1;
    public int ATK
    {
        get => (int)(atk * atk_weight);
    }
    public float def_weight = 1;
    public int DEF
    {
        get => (int)(def * def_weight);
    }
    public float fire_weight = 1;
    public int Fire
    {
        get => (int)(fire * fire_weight);
    }
    public float water_weight = 1;
    public int Water
    {
        get => (int)(water * water_weight); 
    }
    public float thunder_weight = 1;
    public int Thunder
    {
        get => (int)(thunder * thunder_weight);
    }
    public float earth_weight = 1;
    public int Earth
    {
        get => (int)(earth * earth_weight);
    }

    public float taken_damage_weight = 1;
    public float max_taken_damage;
    public float heal_weight = 1;

    public int critical;
    public int Critical
    {
        get => critical;
    }

    public int criticalDamage;
    public int CriticalDamage 
    {
        get => criticalDamage;
    }

    private int pearl;
    public int Pearl
    {
        set
        {
            pearl = value;
            if (pearl < 0)
                pearl = 0;
        }
        get => pearl;
    }

    public int reconnaissance;

    public Inventory invenory;

    private List<int> metlist;

    public bool DidMet(int id)
    {
        return metlist.Contains(id);
    }

    public void Meet(int id)
    {
        metlist.Add(id);
    }

    public PlayerData()
    {
        id = GameManager.Inst.IDMaker;
        name = "name" + id;
        level = 1;
        exp = 0;
        invenory = new Inventory();
        pearl = 0;
        metlist = new List<int>();
    }

    public PlayerData(int ID)
    {
        id = ID;
        name = "name" + id;
        maxHP = 100;
        currentHP = maxHP;
        level = 1;
        exp = 0;
        atk = 50;
        def = 0;
        critical = 25;
        criticalDamage = 30;
        fire = 0;
        water = 0;
        thunder = 0;
        earth = 0;
        atk_weight = 1;
        def_weight = 1; 
        fire_weight = 1;
        water_weight = 1;
        thunder_weight = 1;
        earth_weight = 1;
        taken_damage_weight = 1;
        max_taken_damage = maxHP;
        heal_weight = 1;
        reconnaissance = 30;
        invenory = new Inventory(5);
        pearl = 0;
        metlist = new List<int>();
    }

    public void Copy(PlayerData data)
    {
        base.Copy(data);
        level = data.level;
        exp = data.exp;
        atk = data.atk;
        def = data.def;
        critical = data.critical;
        criticalDamage = data.criticalDamage;
        fire = data.fire;
        water = data.water;
        thunder = data.thunder;
        earth = data.earth;
        atk_weight = data.def_weight;
        def_weight = data.def_weight;
        fire_weight = data.fire_weight;
        water_weight = data.water_weight;
        thunder_weight= data.thunder_weight;
        earth_weight= data.earth_weight;
        taken_damage_weight = data.taken_damage_weight;
        max_taken_damage = data.max_taken_damage;
        heal_weight= data.heal_weight;
        reconnaissance = data.reconnaissance;
        invenory.Copy(data.invenory);
        pearl = data.pearl;
        metlist = data.metlist;
    }
}
