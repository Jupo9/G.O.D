using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CounterNumbers : MonoBehaviour
{
    public TextMeshProUGUI angelCountText;
    public TextMeshProUGUI devilCountText;
    public TextMeshProUGUI fireCountText;
    public TextMeshProUGUI lightCountText;


    private void Update()
    {
        ShowCurrentAngelCount();
        ShowCurrentDevilCount();
        ShowCurrentFireCount();
        ShowCurrentLightCount();
    }

    private void ShowCurrentAngelCount()
    {
        int angelCount = ShowCurrentAngelNumb();

        angelCountText.text = "Angels: " + angelCount;
    }

    private void ShowCurrentDevilCount()
    {
        int devilCount = ShowCurrentDevilNumb();

        devilCountText.text = "Devils: " + devilCount;
    }

    private void ShowCurrentFireCount()
    {
        int fireCount = ShowCurrentFireNumb();

        fireCountText.text = "Fire: " + fireCount;
    }

    private void ShowCurrentLightCount()
    {
        int lightCount = ShowCurrentLightNumb();

        lightCountText.text = "Light: " + lightCount;
    }

    int ShowCurrentFireNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("Res_fire"))
        {
            return worldStates.GetStates()["Res_fire"];
        }

        Debug.LogWarning("State 'UI_Avail_devil' does not exist in WorldStates.");
        return 0;
    }

    int ShowCurrentLightNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("Res_light"))
        {
            return worldStates.GetStates()["Res_light"];
        }

        Debug.LogWarning("State 'UI_Avail_devil' does not exist in WorldStates.");
        return 0;
    }

    int ShowCurrentAngelNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("UI_Avail_angel")) 
        {
            return worldStates.GetStates()["UI_Avail_angel"];
        }

        Debug.LogWarning("State 'UI_Avail_angel' does not exist in WorldStates.");
        return 0; 
    }

    int ShowCurrentDevilNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("UI_Avail_devil"))
        {
            return worldStates.GetStates()["UI_Avail_devil"];
        }

        Debug.LogWarning("State 'UI_Avail_devil' does not exist in WorldStates.");
        return 0;
    }
}
