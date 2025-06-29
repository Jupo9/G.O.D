using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButtonManager : MonoBehaviour
{
    [SerializeField] private int objectID;
    [SerializeField] private Button button;

    private ObjectData objectData;
    private PreviewData previewData;

    private void Start()
    {
        objectData = PlacementSystem.Instance.GetDatabase().objectsData.Find(obj => obj.ID == objectID);
        previewData = objectData.previewData;

        CheckAvailability();
    }

    private void OnEnable()
    {
        Worlds.Instance.GetWorld().OnStateChanged += OnResourceChanged;
        CheckAvailability();
    }

    private void OnDisable()
    {
        Worlds.Instance.GetWorld().OnStateChanged -= OnResourceChanged;
    }

    private void OnResourceChanged(string key, int newValue)
    {
        if (key == "Res_fire" || key == "Res_light")
        {
            CheckAvailability();
        }
    }

    private void CheckAvailability()
    {
        var world = Worlds.Instance.GetWorld();
        var states = world.GetStates();

        states.TryGetValue("Res_fire", out int fire);
        states.TryGetValue("Res_light", out int light);

        bool hasEnoughFire = fire >= previewData.buildFireCosts;
        bool hasEnoughLight = light >= previewData.buildLightCosts;

        button.interactable = hasEnoughFire && hasEnoughLight;
    }

    public void SetObjectID(int id)
    {
        objectID = id;
    }
}
