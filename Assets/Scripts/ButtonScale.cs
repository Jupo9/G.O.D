using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour
{
    private Vector3 originalScale;
    public float scaleFactor = 1.2f;

    void Start()
    {
        originalScale = transform.localScale;

        if (GetComponent<EventTrigger>() == null)
        {
            gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };

        entryEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        entryExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

        eventTrigger.triggers.Add(entryEnter);
        eventTrigger.triggers.Add(entryExit);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}
