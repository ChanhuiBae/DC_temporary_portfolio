using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    StartScene,
    LoadingScene,
    ExplorationScene,
    FightScene,
}
public class GameManager : Singleton<GameManager>
{
    #region Data
    private int idCounter = 0;
    private PlayerData playerData;
    private ExplorationData exploration;
    private SettingData setData;
    
    private DC table;
    private Dictionary<int, Entity_Monster> monsterData = new Dictionary<int, Entity_Monster>();
    public bool GetMonsterData(int id, out Entity_Monster data)
    {
        return monsterData.TryGetValue(id, out data);
    }
    private List<Entity_Squad> squadList = new List<Entity_Squad>();

    public bool GetSquadDatas(int type, int floor, out List<Entity_Squad> datas)
    {
        datas = squadList.FindAll(delegate (Entity_Squad squad)
        {
            return squad.Type == type && squad.Floor == floor;
        });
        if (datas.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private List<Entity_SquadMember> squadMemberList = new List<Entity_SquadMember>();
    public bool GetSquadMemberData(int squadID, out Entity_SquadMember data)
    {
        data = squadMemberList.Find(delegate (Entity_SquadMember squad)
        {
            return squad.SquadID == squadID;
        });
        if (data != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetSquadMemberDatas(int squadID, out List<Entity_SquadMember> datas)
    {
        datas = squadMemberList.FindAll(delegate (Entity_SquadMember squad)
        {
            return squad.SquadID == squadID;
        });
        if (datas.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private Dictionary<int,Entity_Item> itemData = new Dictionary<int, Entity_Item>();
    public bool GetItemData(int id, out Entity_Item data)
    {
        return itemData.TryGetValue(id, out data);
    }

    private Dictionary<int, Entity_Artifact> artifactData = new Dictionary<int, Entity_Artifact>();
    public bool GetArtifactData(int id, out Entity_Artifact data)
    {
        return artifactData.TryGetValue(id, out data);
    }

    private Dictionary<int, Entity_Artifact> artifactLowerMaterialData = new Dictionary<int, Entity_Artifact>();
    private Dictionary<int, Entity_Artifact> artifactLowerItemData = new Dictionary<int, Entity_Artifact>();
    private Dictionary<int, Entity_Artifact> artifactIntermediateMaterialData = new Dictionary<int, Entity_Artifact>();
    private Dictionary<int, Entity_Artifact> artifactIntermediateItemData = new Dictionary<int, Entity_Artifact>();
    private Dictionary<int, Entity_Artifact> artifactIntermediateProductData = new Dictionary<int, Entity_Artifact>();
    private Dictionary<int, Entity_Artifact> artifactAdvancedProductData = new Dictionary<int, Entity_Artifact>();

    private Dictionary<int, Entity_AritifactCombination> artifctCombinationData = new Dictionary<int, Entity_AritifactCombination>();
    public bool GetArtifactCombination(int result, out Entity_AritifactCombination data)
    {
        return artifctCombinationData.TryGetValue(result, out data);
    }

    private Dictionary<int, Entity_Skill> skillData = new Dictionary<int, Entity_Skill>();
    public bool GetSkillData(int id, out Entity_Skill data)
    {
        return skillData.TryGetValue(id, out data);
    }

    private Dictionary<int, string[]> scriptData = new Dictionary<int, string[]>();
    public bool GetScriptData(int id, int index, out string data)
    {
        string[] arr;
        if(scriptData.TryGetValue(id, out arr))
        {
            if (arr.Length > index)
            {
                data = arr[index];
                return true;
            }
        }
        data = "";
        return false;
    }

    private bool fight;
    public bool Fight
    {
        set
        {
            if(SceneManager.GetActiveScene().name == "FightScene")
                fight = value;
        }
        get => fight;
    }
    #endregion

    public PlayerController player;
    public MainManager manager;
    public ItemManager itemManager;
    public ArtifactManager artifactManager;
    public EventManager eventManager;
    public BuffManager buffManager;
    private MonsterManager monsterManager;
    public MonsterManager MonsterManager
    {
        get => monsterManager;
        set
        {
            if(SceneManager.GetActiveScene().name == "FightScene")
            {
                monsterManager = value;
            }
        }
    }

    private void Awake()
    {
        base.Awake();
        if(!TryGetComponent<ItemManager>(out itemManager))
        {
            Debug.Log("GameManager - Awake - ItemManager");
        }
        if (!TryGetComponent<ArtifactManager>(out artifactManager))
        {
            Debug.Log("GameManager - Awake - ArtifactManager");
        }
        if (!TryGetComponent<EventManager>(out eventManager))
        {
            Debug.Log("GameManager - Awake - EventManager");
        }

        dataPath = Application.persistentDataPath + "/save";
        playerData = new PlayerData();
        setData = new SettingData();
        CreateExplorationData();

        if (!TryGetComponent<Map>(out exploration.map))
        {
            Debug.Log("GameManager - Awake - Map");
        }

        //File.Delete(dataPath + "/exploration/map");
        //File.Delete(dataPath + "/exploration/data");
        //File.Delete(dataPath + "/exploration/player");

        #region TableData
        table = Resources.Load<DC>("DC");

        for(int i = 0; i < table.Monster.Count; i++)
        {
            monsterData.Add(table.Monster[i].ID, table.Monster[i]);
        }
        for (int i = 0; i < table.Squad.Count; i++)
        {
            squadList.Add(table.Squad[i]);
        }
        for (int i = 0; i < table.SquadMember.Count; i++)
        {
            squadMemberList.Add(table.SquadMember[i]);
        }
        for (int i = 0; i < table.Item.Count; i++)
        {
            itemData.Add(table.Item[i].ID, table.Item[i]);
        }
        for(int i = 0; i < table.Artifact.Count; i++)
        {
            artifactData.Add(table.Artifact[i].ID, table.Artifact[i]);
            switch (table.Artifact[i].Grade)
            {
                case 1:
                    switch (table.Artifact[i].Type)
                    {
                        case 1:
                            artifactLowerMaterialData.Add(table.Artifact[i].ID, table.Artifact[i]);
                            break;
                        case 2:
                            artifactLowerItemData.Add(table.Artifact[i].ID, table.Artifact[i]);
                            break;
                    }
                    break;
                case 2:
                    switch (table.Artifact[i].Type)
                    {
                        case 1:
                            artifactIntermediateMaterialData.Add(table.Artifact[i].ID, table.Artifact[i]);
                            break;
                        case 2:
                            artifactIntermediateItemData.Add(table.Artifact[i].ID, table.Artifact[i]);
                            break;
                        case 3:
                            artifactIntermediateProductData.Add(table.Artifact[i].ID, table.Artifact[i]);
                            break;
                    }
                    break;
                case 3:
                    artifactAdvancedProductData.Add(table.Artifact[i].ID, table.Artifact[i]);
                    break;
            }
        }
        for(int i = 0; i < table.ArtifactCombination.Count; i++)
        {
            artifctCombinationData.Add(table.ArtifactCombination[i].Result, table.ArtifactCombination[i]);
        }
        for (int i = 0; i < table.Skill.Count; i++)
        {
            skillData.Add(table.Skill[i].ID, table.Skill[i]);
        }
        for(int i = 0; i < table.Script.Count; i++)
        {
            scriptData.Add(table.Script[i].ID, table.Script[i].Text.Split('\"').Where(t => t != "").ToArray());
        }
        #endregion

        if (CheckData())
        {
            LoadData();
        }
        else
        {
            CreateUserData();
        }
    }

    public void CreateUserData()
    {
        playerData = new PlayerData(IDMaker);
        SaveData();
    }

    #region Save&Load
    private string dataPath;
    public void SaveData()
    {
        string path = dataPath + "/player";
        string data = JsonUtility.ToJson(playerData);
        File.WriteAllText(path, data);

        path = dataPath + "/setting";
        data = JsonUtility.ToJson(setData);
        File.WriteAllText(path, data);
    }

    public void SaveExploration()
    {
        string path = dataPath + "/exploration/data";
        string data = JsonUtility.ToJson(exploration);
        File.WriteAllText(path, data);

        path = dataPath + "/exploration/player";
        data = JsonUtility.ToJson(exploration.player);
        File.WriteAllText(path, data);

        path = dataPath + "/exploration/map";
        data = JsonUtility.ToJson(exploration.map.Mapdata);
        File.WriteAllText(path, data);

        string mapArr = "";
        string findArr = "";
        string knownArr = "";
        string goneArr = "";
        string blownArr = "";
        string searchArr = "";

        for (int i = 0; i < exploration.map.MapSize; i++)
        {
            for (int j = 0; j < exploration.map.MapSize; j++)
            {
                mapArr += exploration.map.Mapdata.map[i, j] + ",";
                if (exploration.map.Mapdata.findMap[i, j]) findArr += "1,"; else findArr += "0,";
                if (exploration.map.Mapdata.knownMap[i, j]) knownArr += knownArr + "1,"; else knownArr += "0,";
                goneArr += exploration.map.Mapdata.goneMap[i, j] + ",";
                if (exploration.map.Mapdata.blownupMap[i, j]) blownArr += "1,"; else blownArr += "0,";
                if (exploration.map.Mapdata.searchMap[i, j]) searchArr += "1,"; else searchArr += "0,";
            }
        }
        mapArr = mapArr.Substring(0, mapArr.Length - 1);
        findArr = findArr.Substring(0, findArr.Length - 1);
        knownArr = knownArr.Substring(0, knownArr.Length - 1);
        goneArr = goneArr.Substring(0, goneArr.Length - 1);
        blownArr = blownArr.Substring(0, blownArr.Length - 1);
        searchArr = searchArr.Substring(0, searchArr.Length - 1);

        PlayerPrefs.SetString("map", mapArr);
        PlayerPrefs.SetString("find", findArr);
        PlayerPrefs.SetString("known", knownArr);
        PlayerPrefs.SetString("gone", goneArr);
        PlayerPrefs.SetString("blown", blownArr);
        PlayerPrefs.SetString("search", searchArr);
    }

    public bool LoadData()
    {
        if (File.Exists(dataPath + "/player"))
        {
            string data = File.ReadAllText(dataPath + "/player");
            playerData = JsonUtility.FromJson<PlayerData>(data);

            if (File.Exists(dataPath + "/setting"))
            {
                data = File.ReadAllText(dataPath + "/setting");
                setData = JsonUtility.FromJson<SettingData>(data);

                return true;
            }
        }

        return false;
    }

    public bool LoadExplorationData()
    {
        if (File.Exists(dataPath + "/exploration/data"))
        {
            string data = File.ReadAllText(dataPath + "/exploration/data");
            exploration = JsonUtility.FromJson<ExplorationData>(data);

            data = File.ReadAllText(dataPath + "/exploration/player");
            exploration.player.Copy(JsonUtility.FromJson<CharacterData>(data));

            data = File.ReadAllText(dataPath + "/exploration/map");
            exploration.map.Mapdata = JsonUtility.FromJson<MapData>(data);

            bool result = exploration.map.Mapdata.Copy(PlayerPrefs.GetString("map").Split(','),
            PlayerPrefs.GetString("find").Split(','),
            PlayerPrefs.GetString("known").Split(','),
            PlayerPrefs.GetString("gone").Split(','),
            PlayerPrefs.GetString("blown").Split(','),
            PlayerPrefs.GetString("search").Split(','));

            return result;
        }
        return false;
    }

    public void DeleteData()
    {
        File.Delete(dataPath);
    }

    public bool CheckData()
    {
        if (File.Exists(dataPath))
        {
            return LoadData();
        }
        return false;
    }

    #endregion

    #region updateUserData

    public int IDMaker
    {
        get
        {
            return idCounter++;
        }
    }

    public void CreateUserData(int playerUid)
    {
        SaveData();
    }
    private void CreateExplorationData()
    {
        if (LoadExplorationData())
        {
            exploration.map.Access = true;
        }
        else
        {
            exploration = new ExplorationData(new PlayerData(IDMaker));
        }
    }

    public bool CreateMap()
    {
        if (LoadExplorationData())
        {
            exploration.map.Access = true;
            return true;
        }
        else
        {
            exploration.map.CreateMap(exploration.Floor);
            return false;
        }
    }

    #endregion

    #region Setter & Getter

    public bool CheckNewMap()
    {
        if(exploration.map.Turn == 0)
            return true;
        exploration.map.Access = true;
        return false;
    }

    /*    public void SetVolumBGM(int value)
        {
            setData.bgm = value;
            soundManager.SetVolumBGM(value);
        }

        public void SetVolumSFX(int value)
        {
            setData.sfx = value;
            soundManager.SetVolumSFX(value);
        }
    */
    public PlayerData PlayerData
    {
        get => playerData;
    }

    public ExplorationData Exploration
    {
        get => exploration;
    }

    public SettingData GetSettingData
    {
        get => setData;
    }


    public int GetArtifactCombinationResult(int first, int second)
    {
        if(first >  second)
        {
            int temp = first;
            first = second;
            second = temp;
        }
        foreach (var data in artifctCombinationData)
        {
            if(data.Value.First == first && data.Value.Second == second)
                return data.Key;
        }
        return 0;
    }

    #endregion

    #region Get Random Data
    public int GetRuneCount()
    {
        int value = UnityEngine.Random.Range(0, 100);

        if (exploration.Floor == 1)
        {
            if (value < 20)
                value = 2;
            else
                value = 1;
        }
        else if (exploration.Floor % 2 == 0)
        {
            if (value < 10)
                value = 2;
            else if (value < 50)
                value = 1;
            else
                value = 0;
            value += (int)(exploration.Floor / 2);
        }
        else
        {
            if (value < 5)
                value = 3;
            else if (value < 30)
                value = 2;
            else if (value < 70)
                value = 1;
            else
                value = 0;
            value += (int)(exploration.Floor / 2);
        }
        return value;
    }

    public int GetEssence()
    {
        if (exploration.Floor < 3)
            return 40;
        if (exploration.Floor < 5)
            return 41;
        return 42;
    }

    public bool GetReinforcementItem(out Entity_Item data)
    {
        return itemData.TryGetValue(Random.Range(9, 28), out data);
    }

    public bool GetConsumableItem(out Entity_Item data)
    {
        return itemData.TryGetValue(Random.Range(1, 7), out data);
    }


    public int GetSkillCount()
    {
        return skillData.Count;
    }

    public int GetArtifactCount()
    {
        return artifactData.Count;
    }
    public int GetRandomArtifact(int grade, int type)
    {
        int result = 0;
        switch (grade)
        {
            case 1:
                switch (type)
                {
                    case 1:
                        result = Random.Range(0, artifactLowerMaterialData.Count());
                        result = artifactLowerMaterialData.ElementAt(result).Key;
                        break;
                    case 2:
                        result = Random.Range(0, artifactLowerItemData.Count());
                        result = artifactLowerItemData.ElementAt(result).Key;
                        break;
                }
                break;
            case 2:
                switch (type)
                {
                    case 1:
                        result = Random.Range(0, artifactIntermediateMaterialData.Count());
                        result = artifactIntermediateMaterialData.ElementAt(result).Key;
                        break;
                    case 2:
                        result = Random.Range(0, artifactIntermediateItemData.Count());
                        result = artifactIntermediateItemData.ElementAt(result).Key;
                        break;
                    case 3:
                        result = Random.Range(0, artifactIntermediateProductData.Count());
                        result = artifactIntermediateProductData.ElementAt(result).Key;
                        break;
                }
                break;
            case 3:
                result = Random.Range(0, artifactAdvancedProductData.Count());
                result = artifactAdvancedProductData.ElementAt(result).Key;
                break;
        }
        return result;
    }

    public int GetRandomArtifact()
    {
        int result = Random.Range(0,100);
        switch (exploration.Floor)
        {
            case 1:
                if(result < 50)
                    result = GetRandomArtifact(1, 1);
                else
                    result = GetRandomArtifact(1, 2);
                break;
            case 2:
                if (result < 40)
                    result = GetRandomArtifact(1, 1);
                else if (result < 80)
                    result = GetRandomArtifact(1, 2);
                else if (result < 90)
                    result = GetRandomArtifact(2, 1);
                else
                    result = GetRandomArtifact(2, 2);
                break;
            case 3:
                if (result < 30)
                    result = GetRandomArtifact(1, 1);
                else if (result < 60)
                    result = GetRandomArtifact(1, 2);
                else if (result < 80)
                    result = GetRandomArtifact(2, 1);
                else
                    result = GetRandomArtifact(2, 2);
                break;
            case 4:
                if (result < 20)
                    result = GetRandomArtifact(1, 1);
                else if (result < 40)
                    result = GetRandomArtifact(1, 2);
                else if (result < 70)
                    result = GetRandomArtifact(2, 1);
                else
                    result = GetRandomArtifact(2, 2);
                break;
            case 5:
                if (result < 10)
                    result = GetRandomArtifact(1, 1);
                else if (result < 20)
                    result = GetRandomArtifact(1, 2);
                else if (result < 60)
                    result = GetRandomArtifact(2, 1);
                else
                    result = GetRandomArtifact(2, 2);
                break;
            case 6:
                if (result < 50)
                    result = GetRandomArtifact(2, 1);
                else
                    result = GetRandomArtifact(2, 2);
                break;
        }
        return result;
    }

    public int GetMonsterCount()
    {
        return monsterData.Count;
    }

    public bool GetMonsterSearchByIndex(int id, out Entity_Monster data)
    {
        if (id < monsterData.Count) 
        {
            data = monsterData.ElementAt(id).Value;
            return true;
        }
        data = null;
        return false;
    }

    #endregion

    #region LoadingLogic
    private SceneName nextScene;
    public SceneName NextScene
    {
        get => nextScene;
    }

    public void AsyncLoadNextScene(SceneName scene)
    {
        SaveData();
        nextScene = scene;
        SceneManager.LoadScene("LoadingScene");
    }
    #endregion

    public IEnumerator WaitAndQuitGame(float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        //if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // else 
        // Application.Quit();
    }
}
