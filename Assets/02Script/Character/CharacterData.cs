using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    protected int id;
    public string name;
    public int level;
    public int exp;
    public int atk;
    public int def;
    public int fire;
    public int water;
    public int thunder;
    public int earth;
    public Appearance appearance;


    protected float maxHP;
    public float MaxHP
    {
        get => maxHP;
        set => maxHP = value;
    }
    protected float currentHP;

    public virtual float HP
    {
        get => currentHP;
        set
        {
            currentHP = value;
            if (currentHP > maxHP)
                currentHP = maxHP;
            if (currentHP < 0f)
                currentHP = 0f;
        }
    }

    public CharacterData()
    {
        maxHP = 100f;
        currentHP = maxHP;
    }
    public void Copy(CharacterData data)
    {
        id = data.id;
        name = data.name;
        maxHP = data.MaxHP;
        currentHP = data.currentHP;
    }
}



public struct Appearance
{
    public string Head;
    public string Ears;
    public string Eyes;
    public string Body;
    public string Hair;
    public string Armor;
    public string Helmet;
    public string Weapon;
    public string Firearm;
    public string Shield;
    public string Cape;
    public string Back;
    public string Mask;
    public string Horns;
}

public enum Race
{
    Human,
    Elf,
}

public enum Gender
{
    Male = 0,
    Female = 1
}

public enum Hair
{
    Dracula,
    Hair1,
    Hair2,
    Hair3,
    Hair4,
    Hair5,
    Hair6,
    Hair7,
    Hair8,
    Hair9,
    Hair10,
    Hair11,
    Hair12,
    Hair13,
    Hair14,
    Hair15,
    Rambo
}


public class Human
{
    public enum Skin
    {
        Chocolate,
        Almond,
        Siena,
        Bege,
        Rorcelain
    }

    public Skin skin;
    public Gender gender;
    public Hair hair;
}

public class Elf
{
    public string skin = "Rorcelain";
    public Gender gender;
    public Hair hair;
}
