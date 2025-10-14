using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine("LoadAsyncScene");
    }

    IEnumerator LoadAsyncScene()
    {
        yield return null;
        yield return YieldInstructionCache.WaitForSeconds(1f);

        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(GameManager.Inst.NextScene.ToString());
        asyncScene.allowSceneActivation = false;
        float timeC = 0f;

        while (!asyncScene.isDone)
        {
            yield return null;
            timeC += Time.deltaTime;

            if (asyncScene.progress >= 0.9f)
            {
                asyncScene.allowSceneActivation = true;
            }
            else
            {
                timeC = 0f;
            }
        }

    }
}
