using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    protected void Awake()
    {
        GameManager.Inst.manager = this;
    }

    protected IEnumerator WaitLoadScene(SceneName scene, float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        GameManager.Inst.AsyncLoadNextScene(scene);
    }

    public virtual void SetHP() { }

    protected delegate void CallBack();

    protected CallBack callBack = null;
}
