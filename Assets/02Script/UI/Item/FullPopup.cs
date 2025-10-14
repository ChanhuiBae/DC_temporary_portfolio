using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullPopup : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI message;

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<Button>(out button))
        {
            Debug.Log("FullPopup - Awake - Button");
        }
        if (!transform.GetChild(1).TryGetComponent<TextMeshProUGUI>(out message))
        {
            Debug.Log("FullPopup - Awake - TextMeshProUGUI");
        }
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        gameObject.SetActive(false);
    }

    public void SetFullArtifact()
    {
        message.text = "아티팩트는 15개 까지만\n보유할 수 있습니다..";
    }

    public void SetFullItem()
    {
        message.text = "가방 용량의 한계에 도달하여\n더 이상 획득할 수 없습니다.";
    }

    private void OnClick()
    {
        gameObject.SetActive(false );
    }
}
