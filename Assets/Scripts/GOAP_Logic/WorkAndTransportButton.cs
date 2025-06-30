using UnityEngine.UI;
using UnityEngine;

public class WorkAndTransportButton : MonoBehaviour
{
    public Button transportButton;
    public Button workButton;

    private Agents selectedAgent;

    private void Start()
    {
        SetButtonsInteractable(false); 
    }

    private void Update()
    {
        if (selectedAgent != null)
        {
            bool isBusy = selectedAgent.HasActiveTemporaryAction;
            SetButtonsInteractable(!isBusy);
        }
    }

    public void SetSelectedAgent(Agents agent)
    {
        selectedAgent = agent;
        SetButtonsInteractable(agent != null && !agent.HasActiveTemporaryAction);
    }

    public void OnTransportClicked()
    {
        if (selectedAgent == null) return;

        var action = selectedAgent.actions.Find(a => a is GA_TransportLogic);
        if (action != null)
        {
            selectedAgent.TemporaryAction(action);
            SetButtonsInteractable(false);
        }
    }

    public void OnWorkClicked()
    {
        if (selectedAgent == null) return;

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
}
