using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private List<TextMeshProUGUI> word = new List<TextMeshProUGUI>();

    private void Awake()
    {
        if(transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                word.Add(transform.GetChild(i).GetComponent<TextMeshProUGUI>());
            }
            foreach (TextMeshProUGUI t in word)
            {
                t.color = new Color(1, 1, 1, 0.0f);
            }
        }
        else
        {
            if (!transform.TryGetComponent<TextMeshProUGUI>(out text))
            {
                Debug.Log("StartText - Awake - TextMeshProUGUI");
            }
            transform.LeanScale(Vector3.zero, 0);
        }
    }

    public IEnumerator ShowAndDisappear()
    {
        transform.LeanScale(Vector3.one, 1f).setEase(LeanTweenType.easeOutSine);
        for (float a = 0f; a < 1; a += 0.01f)
        {
            text.color = new Color(1, 1, 1, a);
            yield return null;
        }
        yield return YieldInstructionCache.WaitForSeconds(0.25f);
        transform.LeanScale(Vector3.zero, 0.5f);
        for (float a = 1f; a > 0; a -= 0.02f)
        {
            text.color = new Color(1, 1, 1, a);
            yield return null;
        }
    }

    public IEnumerator ShowSequentially()
    {
        for (int i = 0; i < word.Count; i++)
        {
            StartCoroutine(ShowChar(i));
            yield return YieldInstructionCache.WaitForSeconds(0.2f);
        }
    }

    private IEnumerator ShowChar(int index)
    {
        for (float a = 0; a < 1.1f; a += 0.01f)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.001f);
            word[index].color = new Color(1, 1, 1, a);
        }
    }

    public IEnumerator ShowAndDisappearSequentially()
    {
        for (int i = 0; i < word.Count; i++)
        {
            StartCoroutine(ShowAndDisappearChar(i));
            yield return YieldInstructionCache.WaitForSeconds(0.4f);
        }
    }

    private IEnumerator ShowAndDisappearChar(int index)
    {
        for (float a = 0; a < 1.1f; a += 0.01f)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.001f);
            word[index].color = new Color(1, 1, 1, a);
        }
        yield return YieldInstructionCache.WaitForSeconds(0.002f);
        for (float a = 1; a > -0.1f; a -= 0.02f)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.001f);
            word[index].color = new Color(1, 1, 1, a);
        }
    }
}