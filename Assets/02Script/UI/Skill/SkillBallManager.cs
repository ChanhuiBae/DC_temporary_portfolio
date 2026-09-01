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
    private int typeCount = 5;
    private int ballCount = 54;
    private float maxDistance = 4f;
    public float MaxDis
    {
        get => maxDistance;
    }

    private LineRenderer line;
    private TextMeshProUGUI toast;

    private List<SkillBall> skillBalls = new List<SkillBall>();
    private Dictionary<int, List<SkillBall>> typeDic = new Dictionary<int, List<SkillBall>>();
    private List<SkillBall> connectedBalls = new List<SkillBall>();
    private List<SkillBall> hintBalls = new List<SkillBall>();

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
        ResetAllSkillBalls();
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
        List<SkillBall> list = new List<SkillBall>();
        typeDic.TryGetValue(type, out list);
        if(list.Count < 14)
        {
            list.Add(skill);
            return true;
        }
        return false;
    }

    private void RemoveSkillType(int type, SkillBall skill)
    {
        List<SkillBall> list = new List<SkillBall>();
        typeDic.TryGetValue(type, out list);
        list.Remove(skill);
    }

    private void CheckConnectable(SkillBall startSkill)
    {
        if (!typeDic.TryGetValue(startType, out List<SkillBall> currentTypeList))
            return;

        Queue<SkillBall> queue = new Queue<SkillBall>();
        HashSet<SkillBall> visited = new HashSet<SkillBall>();

        queue.Enqueue(startSkill);
        visited.Add(startSkill);
        startSkill.SetColorWhite();

        while (queue.Count > 0)
        {
            SkillBall current = queue.Dequeue();

            foreach (SkillBall candidate in current.Neighbors) // 캐싱한 이웃 선회
            {
                // 같은 타입이 아니거나, 이미 연결된 목록에 있거나, 이미 방문했다면 스킵
                if (candidate.Type != startType || connectedBalls.Contains(candidate) || visited.Contains(candidate))
                    continue;

                candidate.SetColorWhite();
                visited.Add(candidate);
                queue.Enqueue(candidate);
            }
        }

        foreach (SkillBall s in currentTypeList)
        {
            if (!connectedBalls.Contains(s) && !visited.Contains(s))
            {
                s.Dimmed();
            }
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
        List<SkillBall> list = new List<SkillBall>();
        typeDic.TryGetValue(startType, out list);
        foreach (SkillBall s in list)
        {
            if (!connectedBalls.Contains(s))
            {
                s.Dimmed();
            }
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
        typeDic.Clear();
        for (int i = 0; i < typeCount; i++)
        {
            typeDic.Add(i + 1, new List<SkillBall>());
        }
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

        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        CacheAllNeighbors();
    }

    public void CacheAllNeighbors() // 모든 스킬볼을 대상으로 거리(maxDistance) 이내에 있는 이웃 스킬볼 리스트를 미리 계산하여 캐싱
    {
        float maxDistanceSqr = maxDistance * maxDistance;

        for (int i = 0; i < skillBalls.Count; i++)
        {
            SkillBall ballA = skillBalls[i];
            List<SkillBall> currentNeighbors = new List<SkillBall>();

            for (int j = 0; j < skillBalls.Count; j++)
            {
                if (i == j) continue; // 자신 제외

                SkillBall ballB = skillBalls[j];

                // 거리 제곱(sqrMagnitude)으로 인접 여부 판별
                float sqrDist = (ballA.transform.position - ballB.transform.position).sqrMagnitude;
                if (sqrDist < maxDistanceSqr)
                {
                    currentNeighbors.Add(ballB);
                }
            }

            ballA.SetNeighbors(currentNeighbors);
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
