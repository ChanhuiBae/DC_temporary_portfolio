using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    // 현재 씬에서만 사용하는 UI 풀입니다. 보유 데이터와 구분합니다.
    private readonly List<Bubble> bubbles = new List<Bubble>();

    public void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Bubble bubble = transform.GetChild(i).GetComponent<Bubble>();
            if (bubble != null)
                bubbles.Add(bubble);
        }
    }

    private void Start()
    {
        for (int i = 0; i < bubbles.Count; i++)
            bubbles[i].gameObject.SetActive(false);

        // 씬 복귀: 기존 데이터를 새 UI에 연결합니다.
        // Spawn을 사용하지 않으므로 데이터가 중복 추가되지 않습니다.
        foreach (ArtifactData data in GameManager.Inst.Exploration.bubbles)
        {
            if (!ShowBubble(data))
                Debug.LogError("BubbleManager: 보유 버블 표시 실패. 풀 크기와 테이블을 확인하세요.");
        }
    }

    public bool Spawn(int artifactID)
    {
        return Spawn(artifactID, 1);
    }

    // 신규 획득 전용입니다. 복원은 ShowBubble로 별도 처리합니다.
    public bool Spawn(int artifactID, int mergre)
    {
        if (!GameManager.Inst.Exploration.CheckAddArtifact())
            return false;

        ArtifactData data = new ArtifactData(
            GameManager.Inst.IDMaker, artifactID, mergre);

        // 표시할 공간이 없으면 보유 데이터도 추가하지 않습니다.
        if (!ShowBubble(data))
            return false;

        GameManager.Inst.Exploration.bubbles.Add(data);
        return true;
    }

    private bool ShowBubble(ArtifactData data)
    {
        if (data == null || data.level < 1)
            return false;

        Entity_Artifact artifact;
        if (!GameManager.Inst.GetArtifactData(data.UniqueID, out artifact)
            || artifact == null)
            return false;

        foreach (Bubble bubble in bubbles)
        {
            if (!bubble.gameObject.activeSelf)
            {
                // Awake가 실행되어 컴포넌트 참조가 준비된 뒤 데이터를 연결합니다.
                bubble.gameObject.SetActive(true);
                bubble.Bind(data);
                return true;
            }
        }
        return false;
    }
}
