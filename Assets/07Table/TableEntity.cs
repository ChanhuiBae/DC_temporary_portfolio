using JetBrains.Annotations;


[System.Serializable]
public class Entity_Character
{
    public int ID;
    public string Name;
}

[System.Serializable]
public class Entity_Monster
{
    public int ID;
    public string Name;
    public string Path;
    public float MinHP;
    public float MaxHP;
    public int MinGage;
    public int MaxGage;
    public string Animator;
    public int Magnification;
    public int ATK;
    public int DEF;
    public int Fire;
    public int Water;
    public int Thunder;
    public int Earth;
    public int EXP;
    public string Skill_ID;
    public string Description;
    public int DropItem;
    public int DropPercent;
}
[System.Serializable]
public class Entity_Squad
{
    public int ID;
    public int Type; // nomal = 0, middle boss = 1, boss = 2
    public int Floor;
}
[System.Serializable]
public class Entity_SquadMember
{
    public int SquadID;
    public int MonsterID;
    public bool Front;
    public int Position;
}

public enum ReinforceType
{
    ATK,
    DEF,
    HP,
    Speed,
    Critical,
    CriticalDamage
}

[System.Serializable]
public class Entity_Item
{
    public int ID;
    public string Type;
    public string Name;
    public string Icon;
    public int MaxCount;
    public int MinFloor;
    public int Sale;
    public int Purchase;
    public string Explanation;
}

[System.Serializable]
public class Entity_Artifact
{
    public int ID;
    public string Name;
    public string Icon;
    public int Grade;
    public int Type;
    public string Explanation;
    public string Effect;
}
[System.Serializable]
public class Entity_AritifactCombination
{
    public int Result;
    public int First;
    public int Second;
}
[System.Serializable]
public class Entity_Skill
{
    public int ID;
    public string Name;
    public string Bubble;
    public string Icon;
    public int Attribute;
    public int Type;
    public float Cast;
    public int Area;
    public int Delete;
    public int D_Amount;
    public int Add;
    public int A_Amount;
    public string AttackArea;
    public string Exploration;
}

[System.Serializable]
public class Entity_Script
{
    public int ID;
    public int CharacterID;
    public string Text;
}