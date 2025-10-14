using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private PlayerController player;
    private Camera renderCamera;

    private EventTrigger trigger;
    private EventTrigger.Entry drag;
    private EventTrigger.Entry dragEnd;

    private Vector2 dragStart;
    private bool draging;
    [SerializeField]
    private float speed = 0.05f;

    private void Awake()
    {
        GameObject p = GameObject.Find("Player");
        if(p == null || !p.transform.TryGetComponent<PlayerController>(out player))
        {
            Debug.Log("CameraController - Awake - PlayerController");
        }

        GameObject cam = GameObject.Find("MapCamera");
        if (cam == null || !cam.transform.TryGetComponent<Camera>(out renderCamera))
        {
            Debug.Log("CameraController - Awake -  Camera");
        }
        if (!TryGetComponent<EventTrigger>(out trigger))
        {
            Debug.Log("CameraController - Awake -  EventTrigger");
        }

        drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(drag);
        dragEnd = new EventTrigger.Entry();
        dragEnd.eventID = EventTriggerType.EndDrag;
        dragEnd.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        trigger.triggers.Add(dragEnd);

        dragStart = Vector3.zero;
        draging = false;
    }

    private void Start()
    {
        enabled = false;   
    }
    private void Update()
    {
        if (player.Control)
        {
            float scroll = Input.mouseScrollDelta.y;

            if (renderCamera.fieldOfView < 30)
            {
                renderCamera.fieldOfView = 30;
            }
            else if (renderCamera.fieldOfView > 140)
            {
                renderCamera.fieldOfView = 140;
            }
            else
            {
                renderCamera.fieldOfView -= scroll * 2;
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        renderCamera.transform.position = position + new Vector3(6,0,0);
    }

    private void OnDrag(PointerEventData data)
    {
        if(player.Control && data.button == PointerEventData.InputButton.Middle)
        {
            if (!draging)
            {
                dragStart = data.position;
                draging = true;
            }
            else
            {
                Vector2 delta = data.position - dragStart;
                renderCamera.transform.position -= new Vector3(delta.x, delta.y, 0) * speed;
                dragStart = data.position;
            }
        }
    }
    private void OnEndDrag(PointerEventData data)
    {
        draging = false;
    }

    public void OnEnable()
    {
        if (trigger != null)
            trigger.enabled = true;
    }

    public void OnDisable()
    {
        if (trigger != null)
            trigger.enabled = false;
    }
}
