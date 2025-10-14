using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    private List<TextMeshProUGUI> damages = new List<TextMeshProUGUI>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            damages.Add(transform.GetChild(i).GetComponent<TextMeshProUGUI>());
        }
    }
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            damages[i].enabled = false;
        }
    }

    public void SetDamage(Vector3 position, int damage, Color color, bool critical)
    {
        for (int i = 0; i < damages.Count; i++)
        {
            if (!damages[i].enabled)
            {
                damages[i].enabled = true;
                damages[i].transform.position = position;
                if (damage == 0)
                    damages[i].text = "MISS";
                else
                    damages[i].text = damage.ToString();
                damages[i].color = color;
                if (critical)
                    damages[i].fontStyle = FontStyles.Bold;
                else
                    damages[i].fontStyle = FontStyles.Normal;
                StartCoroutine(DisableText(i));
                break;
            }
        }
    }

    private IEnumerator DisableText(int index)
    {
        void SetCallback(Color c)
        {
            damages[index].color = c;
        }

        transform.LeanMoveY(transform.position.y + 6, 1.5f);
        Color toColor = new Color(damages[index].color.r, damages[index].color.g, damages[index].color.b, 0);
        LeanTween.value(damages[index].gameObject, SetCallback, damages[index].color, toColor, 1.5f);
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        damages[index].enabled = false;

    }
}
