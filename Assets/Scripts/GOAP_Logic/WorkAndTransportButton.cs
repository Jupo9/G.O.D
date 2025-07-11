using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WorkAndTransportButton : MonoBehaviour
{
    public Button transportButton;
    public Button workButton;
    public Button preferClosestButton;

    public TextMeshProUGUI preferenceButtonText;

    private Agents selectedAgent;

    private void Start()
    {
        SetButtonsInteractable(false); 
    }

    private void Update()
    {
        if (selectedAgent != null)
        {
            bool isBusy = selectedAgent.hasPendingPlayerTempAction;
            SetButtonsInteractable(!isBusy);
        }
    }

    public void SetSelectedAgent(Agents agent)
    {
        selectedAgent = agent;
        SetButtonsInteractable(agent != null && !agent.HasActiveTemporaryAction);
        UpdatePreferenceButtonUI();
    }

    public void OnTransportClicked()
    {
        if (selectedAgent == null)
        {
            return;
        }

        var action = selectedAgent.actions.Find(a => a is GA_TransportLogic);

        if (action != null)
        {
            selectedAgent.TemporaryAction(action);
            SetButtonsInteractable(false);
        }
    }

    public void OnWorkClicked()
    {
        if (selectedAgent == null)
        {
            return;
        }

        var action = selectedAgent.actions.Find(a => a is GA_Working);
        if (action != null)
        {
            selectedAgent.TemporaryAction(action);
            SetButtonsInteractable(false);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        transportButton.interactable = interactable;
        workButton.interactable = interactable;
    }

    public void OnPreferClosestClicked()
    {
        if (selectedAgent == null)
        {
            return;
        }

        if (selectedAgent is Angel angel)
        {
            angel.TogglePreferClosest();
            UpdatePreferenceButtonUI();
            Debug.Log($"{angel.name}: PreferClosest = {angel.PreferClosest}");
        }
        else if (selectedAgent is Devil devil)
        { 
            devil.TogglePreferClosest();
            UpdatePreferenceButtonUI();
        }
    }

    private void UpdatePreferenceButtonUI()
    {
        if (selectedAgent is Angel angel && preferenceButtonText != null)
        {
            preferenceButtonText.text = angel.PreferClosest ? "Prefers Closest" : "Prefers Furthest";
        }
        else if (selectedAgent is Devil devil && preferenceButtonText != null)
        {
            preferenceButtonText.text = devil.PreferClosest ? "Prefers Closest" : "Prefers Furthest";
        }
    }
}
