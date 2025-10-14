using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    private Button start;

    private void Awake()
    {
        GameObject s = GameObject.Find("Start");
        if(s == null || !s.TryGetComponent<Button>(out start))
        {
            Debug.Log("StartSceneManager - Awake - Button");
        }
        else
        {
            start.onClick.AddListener(StartExploration);
        }
    }

    private void StartExploration()
    {
        GameManager.Inst.AsyncLoadNextScene(SceneName.ExplorationScene);
    }
}
