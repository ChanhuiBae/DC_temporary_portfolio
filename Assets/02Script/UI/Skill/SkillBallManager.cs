using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayType
{
    OneClick,
    TwoClick,
}
public class SkillBallManager : MonoBehaviour
{
    [SerializeField]
    private PlayType playType;
    public PlayType PlayType
    {
        get => playType;
    }

    private bool deleting;
    public bool Deleting
    {
        get => deleting;
    }
    private int ballCount = 54;
    private float maxDistance = 4f;
    public float MaxDis
    {
        get => maxDistance;
    }

    private LineRenderer line;
    private TextMeshProUGUI toast;

    private List<SkillBall> skillBalls = new List<SkillBall>();
    private List<SkillBall> connectedBalls = new List<SkillBall>();
    private List<SkillBall> hintBalls = new List<SkillBall>();

    private List<SkillBall> type1 = new List<SkillBall>();
    private List<SkillBall> type2 = new List<SkillBall>();
    private List<SkillBall> type3 = new List<SkillBall>();
    private List<SkillBall> type4 = new List<SkillBall>();
    private List<SkillBall> type5 = new List<SkillBall>();

    private int startType; // id
    public int StartType
    {
        get => startType;
        set
        {
            if (startType == 0)
            {
                startType = value;
                dim.SetActive(true);
                foreach(SkillBall s in hintBalls)
                {
                    s.TurnOffLight();
                }
                foreach (SkillBall s in skillBalls)
                {
                    if(s.Type != startType)
                        s.Dimmed();
                }

            }
            else if (value == 0)
            {
                dim.SetActive(false);
                startType = 0;
            }
        }
    }
    private int preType;
    public int PreType
    {
        set => preType = value;
        get => preType;
    }

    private int countSkillBall;
    private float finalTime;
    private float fever; // start fever time. If it is not fever time, then fever is 0.
    [SerializeField]
    private float deltaTime = 5;

    private GameObject feverEffect;

    private GameObject dim;
    private GameObject warning;
    private SkillQueue queue;
    private SkillManager skillManager;

    private void Awake()
    {
        dim = GameObject.Find("Dim");
        dim.SetActive(false);
        warning = GameObject.Find("Warning");
        warning.SetActive(false);
        GameObject lr = GameObject.Find("Line");
        if(lr == null || !lr.transform.TryGetComponent<LineRenderer>(out line))
        {
            Debug.Log("SkillBallManager - Awake - LineRenderer");
        }
        else
        {
            line.enabled = false;
        }
        GameObject q = GameObject.Find("SkillQueue");
        if(q == null || !q.transform.TryGetComponent<SkillQueue>(out queue))
        {
            Debug.Log("SkillBallManager - Awake - SkillQueue");
        }
        GameObject sm = GameObject.Find("SkillManager");
        if(sm == null || !sm.transform.TryGetComponent<SkillManager>(out skillManager))
        {
            Debug.Log("SkillBallManager - Awake - SkillManager");
        }

        GameObject text = GameObject.Find("ToastMessage");
        if(text == null || !text.transform.TryGetComponent<TextMeshProUGUI>(out toast))
        {
            Debug.Log("SkillBallManager - Awake - TextMeshProUGUI");
        }
        else
        {
            toast.enabled = false;
        }

        feverEffect = GameObject.Find("Fever");
        if(feverEffect != null)
        {
            feverEffect.SetActive(false);
        }
        GameObject ft = GameObject.Find("FeverText");


        deleting = false;
        preType = 0;
        skillBalls.Clear();
        connectedBalls.Clear();
    }

    private void Start()
    {
        int i = 0;
        while (skillBalls.Count < ballCount)
        {
            SkillBall skill = transform.GetChild(i).GetComponent<SkillBall>();
            skillBalls.Add(skill);
            i++;
        }
        StartCoroutine(WaitAndCheckPlay());
        countSkillBall = 0;
        finalTime = Time.time;
        StartCoroutine(CheckGiveHint());
        fever = 0;
    }

    public void Stop()
    {
        deleting = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(startType != 0 && !deleting)
            {
                ResetInput();
                line.enabled = false;
                StartCoroutine(CheckGiveHint());
            }
        }
    }

    private void DrawLine()
    {
        line.enabled = true;
        line.positionCount = connectedBalls.Count;

        for(int i =0; i < connectedBalls.Count; i++)
        {
            line.SetPosition(i, connectedBalls[i].transform.position + Vector3.back);
        }
    }

    public bool AddSkillType(int type, SkillBall skill)
    {
        switch (type)
        {
            case 1:
                if (type1.Count < 14)
                {
                    type1.Add(skill);
                    return true;
                }
                else
                    return false;
            case 2:
                if (type2.Count < 14)
                {
                    type2.Add(skill);
                    return true;
                }
                else
                    return false;
            case 3:
                if (type3.Count < 14)
                {
                    type3.Add(skill);
                    return true;
                }
                else
                    return false;
            case 4:
                if (type4.Count < 14)
                {
                    type4.Add(skill);
                    return true;
                }
                else
                    return false;
            case 5:
                if (type5.Count < 14)
                {
                    type5.Add(skill);
                    return true;
                }
                else
                    return false;
            default:
                return false;
        }
    }

    private void RemoveSkillType(int type, SkillBall skill)
    {
        switch (type)
        {
            case 1:
                type1.Remove(skill);
                break;
            case 2:
                type2.Remove(skill);
                break;
            case 3:
                type3.Remove(skill);
                break;
            case 4:
                type4.Remove(skill);
                break;
            case 5:
                type5.Remove(skill);
                break;
        }
    }

    private void CheckConnectable(SkillBall skill)
    {
        skill.SetColorWhite();
        switch ((int)startType)
        {
            case 1:
                foreach (SkillBall s in type1)
                {
                    if (!connectedBalls.Contains(s) && s.CheckDimmed())
                    {
                        float distance = Vector3.Distance(skill.transform.position, s.transform.position);
                        if (distance < maxDistance)
                        {
                            CheckConnectable(s);
                        }
                        else
                        {
                            s.Dimmed();
                        }
                    }
                }
                break;
            case 2:
                foreach (SkillBall s in type2)
                {
                    if (!connectedBalls.Contains(s) && s.CheckDimmed())
                    {
                        float distance = Vector3.Distance(skill.transform.position, s.transform.position);
                        if (distance < maxDistance)
                        {
                            CheckConnectable(s);
                        }
                        else
                        {
                            s.Dimmed();
                        }
                    }
                }
                break;
            case 3:
                foreach (SkillBall s in type3)
                {
                    if (!connectedBalls.Contains(s) && s.CheckDimmed())
                    {
                        float distance = Vector3.Distance(skill.transform.position, s.transform.position);
                        if (distance < maxDistance)
                        {
                            CheckConnectable(s);
                        }
                        else
                        {
                            s.Dimmed();
                        }
                    }
                }
                break;
            case 4:
                foreach (SkillBall s in type4)
                {
                    if (!connectedBalls.Contains(s) && s.CheckDimmed())
                    {
                        float distance = Vector3.Distance(skill.transform.position, s.transform.position);
                        if (distance < maxDistance)
                        {
                            CheckConnectable(s);
                        }
                        else
                        {
                            s.Dimmed();
                        }
                    }
                }
                break;
            case 5:
                foreach (SkillBall s in type5)
                {
                    if (!connectedBalls.Contains(s) && s.CheckDimmed())
                    {
                        float distance = Vector3.Distance(skill.transform.position, s.transform.position);
                        if (distance < maxDistance)
                        {
                            CheckConnectable(s);
                        }
                        else
                        {
                            s.Dimmed();
                        }
                    }
                }
                break;
        }
    }

    public bool CheckCanPlay()
    {
        for (int i = 0; i < skillBalls.Count; i++)
        {
            hintBalls.Clear();
            CheckConnect(skillBalls[i]);
            if (hintBalls.Count > 2)
            {
                break;
            }
        }
        if (hintBalls.Count > 2)
            return true;
        else
            return false;
    }
    private void CheckConnect(SkillBall skill)
    {
        hintBalls.Add(skill);
        foreach (SkillBall s in skillBalls)
        {
            if (skill.Type == s.Type && !hintBalls.Contains(s))
            {
                float distance = Vector3.Distance(skill.transform.position, s.transform.position);
                if (distance < maxDistance)
                {
                    CheckConnect(s);
                }
            }
        }
    }

    public Vector3 GetPreSkillPosition()
    {
        if (connectedBalls.Count > 0)
        {
            return connectedBalls[connectedBalls.Count - 1].transform.position;
        }
        else
            return Vector3.zero;
    }

    private void SetTypeDimmed()
    {
        switch ((int)startType)
        {
            case 1:
                foreach (SkillBall s in type1)
                {
                    if (!connectedBalls.Contains(s))
                    {
                        s.Dimmed();
                    }
                }
                break;
            case 2:
                foreach (SkillBall s in type2)
                {
                    if (!connectedBalls.Contains(s))
                    {
                        s.Dimmed();
                    }
                }
                break;
            case 3:
                foreach (SkillBall s in type3)
                {
                    if (!connectedBalls.Contains(s))
                    {
                        s.Dimmed();
                    }
                }
                break;
            case 4:
                foreach (SkillBall s in type4)
                {
                    if (!connectedBalls.Contains(s))
                    {
                        s.Dimmed();
                    }
                }
                break;
            case 5:
                foreach (SkillBall s in type5)
                {
                    if (!connectedBalls.Contains(s))
                    {
                        s.Dimmed();
                    }
                }
                break;
        }
    }

    public void AddConnected(SkillBall skill)
    {
        if (!deleting && !connectedBalls.Contains(skill))
        {
            connectedBalls.Add(skill);
            SetTypeDimmed();
            CheckConnectable(skill);
            DrawLine();
        }
    }
    public bool PopSkillBalls(SkillBall skill)
    {
        if (deleting)
            return false;

        int index = -1;
        index = connectedBalls.IndexOf(skill);
        if (index != -1)
        {
            for (int i = connectedBalls.Count - 1; i > index; i--)
            {
                connectedBalls[i].TurnOffLight();
                connectedBalls.RemoveAt(i);
            }
            SetTypeDimmed();
            CheckConnectable(skill);
            DrawLine();
            return false;
        }
        return true;
    }
    
    public void TakeSkillBalls()
    {
        deleting = true;
        line.enabled = false;
        if (connectedBalls.Count > 2)
        {
            if (queue.AddReadySkill(GameManager.Inst.Exploration.skills[startType], connectedBalls.Count))
            {
                SetCountAndTime();
                skillManager.AddFill(connectedBalls.Count);
                CheckFever();
                StartCoroutine(DeleteSkillBalls());
                StartCoroutine(CheckGiveHint());
            }
            else
            {
                StartCoroutine(ShowFullQueue());
            }
            
        }
        else
        {
            ReadyNewInput();
            StartCoroutine(CheckGiveHint());
        }
    }

    private IEnumerator DeleteSkillBalls()
    {
        while (connectedBalls.Count > 0)
        {
            connectedBalls[0].StartCoroutine(connectedBalls[0].DisableSkillBall());
            RemoveSkillType((int)startType, connectedBalls[0]);
            connectedBalls.RemoveAt(0);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        ReadyNewInput();
    }

    private IEnumerator ShowFullQueue()
    {
        StartCoroutine(ShowToastMessage(connectedBalls[connectedBalls.Count - 1].transform.position));
        foreach(SkillBall s in skillBalls)
        {
            s.SetColorWhite();
        }
        foreach(SkillBall s in connectedBalls)
        {
            s.SetLightColor(Color.red);
        }
        yield return YieldInstructionCache.WaitForSeconds(1f);
        foreach(SkillBall s in connectedBalls)
        {
            s.SetLightColor(Color.white);
            s.TurnOffLight();
        }
        connectedBalls.Clear();
        StartType = 0;
        preType = 0;
        StartCoroutine(WaitAndCheckPlay());
    }

    private void ResetSkillBalls()
    {
        for(int i = 0; i < skillBalls.Count; i++)
        {
            skillBalls[i].SetColorWhite();
            skillBalls[i].TurnOffLight();
        }
    }

    private void ReadyNewInput()
    {
        ResetInput();
        StartCoroutine(WaitAndCheckPlay());
    }

    private void ResetInput()
    {
        ResetSkillBalls();
        connectedBalls.Clear();
        StartType = 0;
        preType = 0;
    }

    private IEnumerator WaitAndCheckPlay()
    {
        if (CheckCanPlay())
            deleting = false;
        else
        {
            warning.SetActive(true);
            yield return YieldInstructionCache.WaitForSeconds(1f);
            ResetAllSkillBalls();
            yield return YieldInstructionCache.WaitForSeconds(3f);
            while (!CheckCanPlay())
            {
                ResetAllSkillBalls();
                yield return YieldInstructionCache.WaitForSeconds(3f);
            }
            foreach (SkillBall s in skillBalls)
            {
                s.SetColorWhite();
            }
            warning.SetActive(false);
            deleting = false;
        }

    }

    private void ResetAllSkillBalls()
    {
        type1.Clear();
        type2.Clear();
        type3.Clear();
        type4.Clear();
        type5.Clear();
        StartCoroutine(RespownSkillBalls());
    }

    private IEnumerator RespownSkillBalls()
    {
        foreach (SkillBall s in skillBalls)
        {
            s.StartCoroutine(s.RespawnAllEffect());
            yield return null;
            yield return null;
        }
    }

    private IEnumerator ShowToastMessage(Vector3 pos)
    {
        toast.enabled = true;
        toast.transform.position = pos;
        toast.color = new Color(1, 1, 1, 1);
        yield return YieldInstructionCache.WaitForSeconds(0.3f);
        float alpha = 1f;
        while (toast.color.a > 0.01f)
        {
            toast.transform.position += new Vector3(0, 0.01f, 0);
            toast.color = new Color(1, 1, 1, alpha);
            alpha -= 0.01f;
            yield return null;
        }
        toast.enabled = false;
        yield return null;
    }

    private void SetCountAndTime()
    {
        if(fever == 0)
        {
            float currentTime = Time.time;
            if (currentTime - finalTime > deltaTime)
            {
                countSkillBall = connectedBalls.Count;
            }
            else
            {
                countSkillBall += connectedBalls.Count;
            }
            finalTime = currentTime;
        }
    }

    private void CheckFever()
    {
        if(fever == 0 && countSkillBall >= 40) // fever cut
        {
            countSkillBall = 0;
            StartCoroutine(FeverTime());
        }
    }

    private IEnumerator FeverTime()
    {
        fever = Time.time;
        queue.Fever = true;
        feverEffect.SetActive(true);
        FightSceneManager manager = (FightSceneManager)GameManager.Inst.manager;
        manager.Fever();
        float current = Time.time;
        while(current - fever < 10)
        {
            yield return null;
            current = Time.time;
        }
        feverEffect.SetActive(false);
        queue.Fever = false;
        fever = 0;
    }

    private IEnumerator CheckGiveHint()
    {
        if (finalTime + deltaTime < Time.time)
        {
            GiveHint();
        }
        else
        {
            yield return YieldInstructionCache.WaitForSeconds(deltaTime);
            if (startType == 0 && finalTime + deltaTime < Time.time)
            {
                GiveHint();
            }
        }
           
    }

    private void GiveHint()
    {
        for (int i = 0; i < skillBalls.Count; i++)
        {
            hintBalls.Clear();
            CheckConnect(skillBalls[i]);
            if (hintBalls.Count > 2)
            {
                break;
            }
        }
        if (hintBalls.Count > 2)
        {
            foreach (SkillBall s in hintBalls)
            {
                s.SetLightColor(Color.white);
            }
        }
    }

}
