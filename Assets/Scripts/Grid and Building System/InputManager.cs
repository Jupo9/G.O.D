using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Input System for keyboard and mouse logics
    /// </summary>
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;

    public event Action OnClicked;
    public event Action OnExit;
    public event Action OnRotate;

    private Vector3 lastPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            OnExit?.Invoke();
        }

        if (Input.GetMouseButtonDown(2)) 
        {
           //Debug.Log("notice Rotation");
            OnRotate?.Invoke();
        }
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
