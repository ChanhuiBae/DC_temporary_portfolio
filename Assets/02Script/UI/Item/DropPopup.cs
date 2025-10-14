using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropPopup : MonoBehaviour
{
    public Popup popup;
    private Button yes;
    private Button no;
    private TextMeshProUGUI sub;

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent<Button>(out yes))
        {
            Debug.Log("DropPopup - Awake - Button");
        }
        if (!transform.GetChild(1).TryGetComponent<Button>(out no))
        {
            Debug.Log("DropPopup - Awake - Button");
        }
        if(!transform.GetChild(3).TryGetComponent<TextMeshProUGUI>(out sub))
        {
            Debug.Log("DropPopup - Awake - TextMeshProUGUI");
        }
    }
    private void Start()
    {
        yes.onClick.AddListener(ClosePopup);
        no.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    public void SetText(bool isLoot)
    {
        if (isLoot)
            sub.text = "획득하지 않은 아이템은 사라집니다.";
        else
            sub.text = "획득하지 않은 아이템은 판매됩니다.";
    }


    private void ClosePopup()
    {
        if(popup != null) 
            popup.Close();
        gameObject.SetActive(false);
    }
}
