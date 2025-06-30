using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelection : MonoBehaviour, IPointerClickHandler
{
    private Agents agent;

    private void Awake()
    {
        agent = GetComponent<Agents>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            UnitActionUIManager.Instance.SetSelectedAgent(agent);
        }
    }
}
