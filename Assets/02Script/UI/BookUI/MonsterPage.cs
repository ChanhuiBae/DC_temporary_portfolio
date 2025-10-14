using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPage : PageUI
{
    private Image profile;
    private TextMeshProUGUI name;
    private TextMeshProUGUI description;
    private Image[] fire = new Image[3];
    private Image[] water = new Image[3];
    private Image[] thunder = new Image[3];
    private Image[] earth = new Image[3];

    private void Awake()
    {
        Transform t = transform.Find("Character");
        if(t == null || !t.TryGetComponent<Image>(out profile))
        {
            Debug.Log("MonsterPage - Awake - Image");
        }
        t = transform.Find("Name");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out name))
        {
            Debug.Log("MonsterPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Description");
        if (t == null || !t.TryGetComponent<TextMeshProUGUI>(out description))
        {
            Debug.Log("MonsterPage - Awake - TextMeshProUGUI");
        }
        t = transform.Find("Fire");
        for (int i = 0; i < 3; i++)
        {
            if(!t.GetChild(i).TryGetComponent<Image>(out fire[i]))
            {
                Debug.Log("MonsterPage - Awake - Image");
            }
        }
        t = transform.Find("Water");
        for (int i = 0; i < 3; i++)
        {
            if (!t.GetChild(i).TryGetComponent<Image>(out water[i]))
            {
                Debug.Log("MonsterPage - Awake - Image");
            }
        }
        t = transform.Find("Thunder");
        for (int i = 0; i < 3; i++)
        {
            if (!t.GetChild(i).TryGetComponent<Image>(out thunder[i]))
            {
                Debug.Log("MonsterPage - Awake - Image");
            }
        }
        t = transform.Find("Earth");
        for (int i = 0; i < 3; i++)
        {
            if (!t.GetChild(i).TryGetComponent<Image>(out earth[i]))
            {
                Debug.Log("MonsterPage - Awake - Image");
            }
        }
    }

    public void SetPage(Entity_Monster data)
    {
        profile.sprite = Resources.Load<Sprite>("Profile/" + data.Path);
        name.text = data.Name;
        description.text = data.Description;

        if (data.Fire < -20)
            SetStar(fire, 3);
        else if (data.Fire < 1)
            SetStar(fire, 2);
        else if(data.Fire < 21)
            SetStar(fire, 1);
        else 
            SetStar(fire, 0);

        if (data.Water < -20)
            SetStar(water, 3);
        else if (data.Water < 1)
            SetStar(water, 2);
        else if (data.Water < 21)
            SetStar(fire, 1);
        else
            SetStar(water, 0);

        if (data.Thunder < -20)
            SetStar(thunder, 3);
        else if (data.Thunder < 1)
            SetStar(thunder, 2);
        else if (data.Thunder < 21)
            SetStar(thunder, 1);
        else
            SetStar(thunder, 0);

        if (data.Earth < -20)
            SetStar(earth, 3);
        else if (data.Earth < 1)
            SetStar(earth, 2);
        else if (data.Earth < 21)
            SetStar(earth, 1);
        else
            SetStar(earth, 0);
    }

    private void SetStar(Image[] images, int value)
    {
        for(int i = 0; i < value; i++)
        {
            images[i].color = Color.yellow;
        }
        for(int i = value; i < images.Length; i++)
        {
            images[i].color = Color.white;
        }
    }
}
