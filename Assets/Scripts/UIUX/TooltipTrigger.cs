using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string contentA;
    public string contentB;

    private Coroutine tooltipCoroutine;
    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        tooltipCoroutine = StartCoroutine(DelayedTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
            tooltipCoroutine = null;
        }

        TooltipSystem.Hide();
    }

    private IEnumerator DelayedTooltip()
    {
        float delay = 1.5f;
        float elapsed = 0f;

        while (elapsed < delay)
        {
            if (!isHovering)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        TooltipSystem.Show(header, contentA, contentB);
    }
}
