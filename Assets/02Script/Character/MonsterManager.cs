using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterPosition
{
    middle = 0,
    leftup = 1,
    leftcenter = 2,
    leftdown = 3,
    rightup = 4,
    rightcenter = 5,
    rightdown = 6
}
public class MonsterManager : MonoBehaviour
{
    private List<Entity_SquadMember> spawnSquad;
    private Entity_Monster[] monsterDatas = new Entity_Monster[4];
    private MonsterController[] monsterControllers = new MonsterController[4];
    private CharacterInfo[] infos = new CharacterInfo[4];
    private Image[] gages = new Image[4];
    private Button[] buttons = new Button[4];

    private Image targetCheck;
    private Image targetInfo;
    private int target;
    public int Target
    {
        set => target = value;
        get => target;
    }

    public MonsterController GetTarget()
    {
        return monsterControllers[target];
    }

    public Vector3 GetRandomTarget()
    {
        if (monsterControllers[0].IsAlive 
            || monsterControllers[1].IsAlive
            || monsterControllers[2].IsAlive
            || monsterControllers[3].IsAlive)
        {
            int index;
            do
            {
                index = Random.Range(0, 4);
            }
            while (!monsterControllers[index].IsAlive);

            return monsterControllers[index].transform.position;
        }
        return Vector3.zero;
    }

    private void Awake()
    {
        GameManager.Inst.MonsterManager = this;

        GameObject targetImage = GameObject.Find("Target");
        if (targetImage == null || !targetImage.TryGetComponent<Image>(out targetCheck))
        {
            Debug.Log("MonsterManager - Awake - Image");
        }
        targetImage = GameObject.Find("TargetInfo");
        if (targetImage == null || !targetImage.TryGetComponent<Image>(out targetInfo))
        {
            Debug.Log("MonsterManager - Awake - Image");
        }
        targetInfo.enabled = false;
        targetCheck.enabled = false;

        for (int i = 0; i < 4; i++)
        {
            if (!transform.GetChild(i).TryGetComponent<MonsterController>(out monsterControllers[i]))
            {
                Debug.Log("MonsterManger - Awake - MonsterController");
            }
            else
            {
                monsterControllers[i].Priority = i;
            }
        }

        GameObject infoArea = GameObject.Find("MonsterInfoArea");
        if (infoArea != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!infoArea.transform.GetChild(i).TryGetComponent<CharacterInfo>(out infos[i]))
                {
                    Debug.Log("MonsterManger - Awake - CharacterInfo");
                }
                else
                {
                    infos[i].gameObject.SetActive(false);
                }
            }
        }

        GameObject gageArea = GameObject.Find("GageArea");
        if (gageArea != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!gageArea.transform.GetChild(i).GetChild(0).TryGetComponent<Image>(out gages[i]))
                {
                    Debug.Log("MonsterManager - Awake - Image");
                }
            }
        }

        GameObject targetArea = GameObject.Find("TargetArea");
        if (targetArea != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!targetArea.transform.GetChild(i).TryGetComponent<Button>(out buttons[i]))
                {
                    Debug.Log("MonsterManager - Awake - Button");
                }
            }
            buttons[0].onClick.AddListener(() => SetTarget(0));
            buttons[1].onClick.AddListener(() => SetTarget(1));
            buttons[2].onClick.AddListener(() => SetTarget(2));
            buttons[3].onClick.AddListener(() => SetTarget(3));
        }


        // active fuction
        void Active(int i)
        {
            monsterControllers[i].IsAlive = true;
            monsterControllers[i].gameObject.SetActive(true);
            gages[i].transform.parent.gameObject.SetActive(true);
            buttons[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < 4; i++)
        {
            monsterControllers[i].IsAlive = false;
            monsterControllers[i].gameObject.SetActive(false);
            gages[i].transform.parent.gameObject.SetActive(false);
            buttons[i].gameObject.SetActive(false);
        }

        int id = GameManager.Inst.Exploration.GetSquad().ID;
        GameManager.Inst.GetSquadMemberDatas(id, out spawnSquad);
        for (int i = 0; i < spawnSquad.Count; i++)
        {
            if (i < 4)
            {
                GameManager.Inst.GetMonsterData(spawnSquad[i].MonsterID, out monsterDatas[i]);
                int index = i;
                switch (spawnSquad[i].Position)
                {
                    case 0:
                        Active(2);
                        index = 2;
                        monsterControllers[index].transform.position = new Vector3(14.5f, -4.25f, 0);
                        monsterControllers[index].transform.LeanMoveX(4.5f, 1.7f);
                        gages[index].transform.parent.LeanMoveX(4.5f, 1.7f);
                        buttons[index].transform.position = new Vector3(4.5f, -4.25f, 0);
                        infos[index].SetBoss();
                        break;
                    case 1:
                        Active(0);
                        index = 0;
                        monsterControllers[index].transform.position = new Vector3(10, -2, 0);
                        monsterControllers[index].transform.LeanMoveX(0, 1.7f);
                        gages[index].transform.parent.LeanMoveX(0, 1.7f);
                        buttons[index].transform.position = new Vector3(0, -2, 0);
                        break;
                    case 2:
                        Active(0);
                        index = 0;
                        monsterControllers[index].transform.position = new Vector3(11.5f, -4.25f, 0);
                        monsterControllers[index].transform.LeanMoveX(1.5f, 1.7f);
                        gages[index].transform.parent.LeanMoveX(1.5f, 1.7f);
                        buttons[index].transform.position = new Vector3(1.5f, -4.25f, 0);
                        break;
                    case 3:
                        Active(1);
                        index = 1;
                        monsterControllers[index].transform.position = new Vector3(13, -6.5f, 0);
                        monsterControllers[index].transform.LeanMoveX(3, 1.7f);
                        gages[index].transform.parent.LeanMoveX(3, 1.7f);
                        buttons[index].transform.position = new Vector3(3, -6.5f, 0);
                        break;
                    case 4:
                        Active(2);
                        index = 2;
                        monsterControllers[index].transform.position = new Vector3(16, -2, 0);
                        monsterControllers[index].transform.LeanMoveX(6, 1.7f);
                        gages[index].transform.parent.LeanMoveX(6, 1.7f);
                        buttons[index].transform.position = new Vector3(6, -2, 0);
                        break;
                    case 5:
                        Active(2);
                        index = 2;
                        monsterControllers[index].transform.position = new Vector3(17.5f, -4.25f, 0);
                        monsterControllers[index].transform.LeanMoveX(7.5f, 1.7f);
                        gages[index].transform.parent.LeanMoveX(7.5f, 1.7f);
                        buttons[index].transform.position = new Vector3(7.5f, -4.25f, 0);
                        break;
                    case 6:
                        Active(3);
                        index = 3;
                        monsterControllers[index].transform.position = new Vector3(19, -6.5f, 0);
                        monsterControllers[index].transform.LeanMoveX(9, 1.7f);
                        gages[index].transform.parent.LeanMoveX(9, 1.7f);
                        buttons[index].transform.position = new Vector3(9, -6.5f, 0);
                        break;
                }
                infos[index].gameObject.SetActive(true);
                infos[index].SetHP(Random.Range(monsterDatas[i].MinHP, monsterDatas[i].MaxHP + 1));
                infos[index].SetName(monsterDatas[i].Name);
                infos[index].SetCharacterImage(Resources.Load<Sprite>("Profile/" + monsterDatas[i].Path));
                monsterControllers[index].Data = monsterDatas[i];
                monsterControllers[index].SetAnimator(monsterDatas[i].Animator);
                monsterControllers[index].FullHP = Random.Range(monsterDatas[i].MinHP, monsterDatas[i].MaxHP + 1);
                monsterControllers[index].FullGage = Random.Range(monsterDatas[i].MinGage, monsterDatas[i].MaxGage + 1);
                monsterControllers[index].SetSprite(monsterDatas[i].Path);
                if (monsterDatas[i].ID > 900)
                {
                    monsterControllers[index].SetScale(false, monsterDatas[i].Magnification);
                    gages[index].transform.parent.position = monsterControllers[index].transform.position + new Vector3(0, monsterControllers[index].GetScale().y * 1.5f, 0);
                }
                else
                {
                    monsterControllers[index].SetScale(true, monsterDatas[i].Magnification);
                    gages[index].transform.parent.position = monsterControllers[index].transform.position + new Vector3(0, monsterControllers[index].GetScale().y * 2, 0);
                }
                monsterControllers[index].StartAction();
                buttons[index].transform.LeanScale(monsterControllers[index].GetScale() / 2, 0);
            }
            else
            {
                break;
            }
        }

        if (spawnSquad.Count == 2 && spawnSquad[0].Position == 1 && spawnSquad[1].Position == 3)
        {
            infos[0].transform.position = infos[2].transform.position;
            infos[1].transform.position = infos[3].transform.position;
        }
    }

    private void Start()
    {
        StartCoroutine(WaitAndSetTarget());
    }

    private IEnumerator WaitAndSetTarget()
    {
        yield return YieldInstructionCache.WaitForSeconds(1.7f);
        SetTarget(0);
    }


    private void SetTarget(int value) // value=>position | 0=>1,2 | 1=>3 | 2=>0,4,5 | 3=>6
    {
        if(value > 1) // not front
        {
            if (value ==2 && monsterControllers[0].IsAlive)
            {
                return;
            }
            else if(value == 2 && spawnSquad[spawnSquad.Count-1].Position == 5 && monsterControllers[1].IsAlive)
            {
                return;
            }
            else if(value == 3)
            {
                if ((spawnSquad[0].Position == 2 && monsterControllers[0].IsAlive)
                    || (spawnSquad[1].Position == 3 && monsterControllers[1].IsAlive))
                {
                    return;
                }
            }
        }
        if (monsterControllers[value].IsAlive)
        {
            target = value;
            targetInfo.enabled = true;
            targetCheck.enabled = true;
            targetCheck.transform.position = buttons[target].transform.position;
            targetInfo.transform.position = infos[target].GetPosition();
            targetCheck.transform.LeanScale(monsterControllers[target].GetScale() / 2, 0);
        }
        else
        {
            SetTarget(++value);
        }
    }

    public Vector3 GetTargetCenter()
    {
        return monsterControllers[target].transform.position + new Vector3(0, monsterControllers[target].GetScale().y / 2);
    }

    public Vector3 GetTargetGround()
    {
        return monsterControllers[target].transform.position;
    }

    public void SetHP(int index, float value)
    {
        infos[index].SetHP(value);
    }

    public void ReTarget()
    {
        bool allDie = true;
        for (int i = 0; i < 4; i++)
        {
            if (monsterControllers[i].IsAlive)
            {
                SetTarget(i);
                allDie = false;
                break;
            }
        }
        if (allDie)
        {
            FightSceneManager manager = (FightSceneManager)GameManager.Inst.manager;
            targetInfo.enabled = false;
            targetCheck.enabled = false;
            GameManager.Inst.Exploration.squadClear = true;
            manager.Victory();
        }
    }

    public void SetGage(int index, float value)
    {
        gages[index].fillAmount = value;
    }

    public void DisableGage(int index)
    {
        gages[index].transform.parent.gameObject.SetActive(false);
    }
}
