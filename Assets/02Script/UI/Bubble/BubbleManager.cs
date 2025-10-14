using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    private List<Bubble> bubbles = new List<Bubble>();

    public void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Bubble bubble = transform.GetChild(i).GetComponent<Bubble>();
            bubbles.Add(bubble);
        }
    }
    private void Start()
    {
        for (int i = 0; i < bubbles.Count; i++)
        {
            bubbles[i].gameObject.SetActive(false);
        }
        for(int i = 0; i < GameManager.Inst.Exploration.bubbles.Count; i++)
        {
            Spawn(GameManager.Inst.Exploration.bubbles[i].ArtifactID, GameManager.Inst.Exploration.bubbles[i].Mergre);
        }
    }

    public bool Spawn(int artifactID)
    {
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (!bubbles[i].gameObject.activeSelf)
            {
                bubbles[i].gameObject.SetActive(true);
                bubbles[i].SetBubble(artifactID, 1);
                GameManager.Inst.Exploration.bubbles.Add(bubbles[i]);
                return true;
            }
        }
        return false;
    }
    public bool Spawn(int artifactID,int mergre)
    {
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (!bubbles[i].gameObject.activeSelf)
            {
                bubbles[i].gameObject.SetActive(true);
                bubbles[i].SetBubble(artifactID,mergre);
                return true;
            }
        }
        return false;
    }

}
