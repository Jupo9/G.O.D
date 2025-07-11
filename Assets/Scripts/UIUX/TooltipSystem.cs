using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;

    public Tooltip tooltip;

    private void Awake()
    {
        current = this;
    }

    public static void Show(string header, string contentA, string contentB = "")
    {
        current.tooltip.SetText(header, contentA, contentB);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
