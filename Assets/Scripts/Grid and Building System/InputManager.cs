using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Pause game with key: Space
    /// Events for Buidling System ()
    /// Events for CameraMovement (Zoom, Movement, Rotation, Reset)
    /// Selection feature for Devils and Angels with connection to SelectionOutline Script 
    /// </summary>

    public static InputManager Instance { get; private set; }

    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;
    [SerializeField] private LayerMask creatureMask;

    private GameObject selectedCreature;
    private string selectedTag;
    private SelectionOutline selectedOutline;

    //Events for Building System
    public event Action OnClicked;
    public event Action OnExit;
    public event Action OnRotate;

    [SerializeField] private GameObject escapeBuildingMenu;
    [SerializeField] private GameObject[] disableBuildingUI;

    //Events for CameraMovement
    public event Action<Vector2> OnCameraMove;
    public event Action<float> OnCameraZoom;
    public event Action<float> OnCameraRotate;
    public event Action OnCameraResetRotation;

    [SerializeField] private WorkAndTransportButton workAndTransportUI;

    private Vector3 lastPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        //CameraMovement Inputs
        MoveAround();
        CameraZoom();
        RotateCamera();
        ResetCamera();

        //Building Inputs
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            OnExit?.Invoke();
            DisableBuildingUI();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRotate?.Invoke();
        }

        PauseGame();

        HandleCreatureSelection();
    }

    // ------------- Building Grid Controll -------------

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
            return hit.point;
        }
        return Vector3.negativeInfinity;
    }

    private void DisableBuildingUI()
    {
        if (escapeBuildingMenu != null)
        {
            escapeBuildingMenu.SetActive(true);
        }

        if (disableBuildingUI != null)
        {
            foreach (var ui in disableBuildingUI)
            {
                if (ui != null)
                {
                    ui.SetActive(false);
                }
            }
        }
    }


    // ------------- Camera Movement -------------

    // camera movement with WASD or Arrow-Keys
    private void MoveAround()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveInput != Vector2.zero)
        {
            OnCameraMove?.Invoke(moveInput);
        }
    }

    // zoom for Camera
    private void CameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            OnCameraZoom?.Invoke(scroll);
        }
    }

    // rotation for Camera
    private void RotateCamera()
    {
        if (Input.GetMouseButton(2))
        {
            float rotateInput = Input.GetAxis("Mouse X");
            OnCameraRotate?.Invoke(rotateInput);
        }
    }

    // double click for Camera Reset
    private void ResetCamera()
    {
        if (Input.GetMouseButtonDown(2))
        {
            OnCameraResetRotation?.Invoke();
        }
    }

    // ------------- Pause Game -------------

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
        }
    }

    // ------------- Select Unit -------------

    private void HandleCreatureSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, creatureMask))
            {
                GameObject target = hit.collider.gameObject;

                if (target.layer == LayerMask.NameToLayer("Creature"))
                {
                    SelectCreature(target);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectCreature();
        }
    }

    private void SelectCreature(GameObject creature)
    {
        if (selectedCreature == creature)
        {
            return;
        }

        DeselectCreature();

        selectedCreature = creature;
        selectedTag = creature.tag;

        // activate outline
        selectedOutline = creature.GetComponent<SelectionOutline>();
        if (selectedOutline != null)
        {
            selectedOutline.SetOutline(true);
        }

        // activate Canvasgroup
        CanvasGroup cg = creature.GetComponentInChildren<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
        }

        Agents agent = creature.GetComponent<Agents>();
        if (agent != null && workAndTransportUI != null)
        {
            workAndTransportUI.SetSelectedAgent(agent);
        }
    }

    private void DeselectCreature()
    {
        if (selectedCreature == null)
        {
            return;
        }

        // deactivate outline
        if (selectedOutline != null)
        {
            selectedOutline.SetOutline(false);
        }

        // deactivate Canvasgroup
        CanvasGroup cg = selectedCreature.GetComponentInChildren<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
        }

        selectedCreature = null;
        selectedOutline = null;
        selectedTag = null;

        if (workAndTransportUI != null)
        {
            workAndTransportUI.SetSelectedAgent(null);
        }
    }
}
