using TMPro;
using UnityEngine;

public class CounterNumbers : MonoBehaviour
{
    // count Angel, Fire, Devil and lights 
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

    //show in UI counts 
    private void ShowCurrentAngelCount()
    {
        int angelCount = ShowCurrentAngelNumb();

        angelCountText.text = "" + angelCount;
    }

    private void ShowCurrentDevilCount()
    {
        int devilCount = ShowCurrentDevilNumb();

        devilCountText.text = "" + devilCount;
    }

    private void ShowCurrentFireCount()
    {
        int fireCount = ShowCurrentFireNumb();

        fireCountText.text = "" + fireCount;
    }

    private void ShowCurrentLightCount()
    {
        int lightCount = ShowCurrentLightNumb();

        lightCountText.text = "" + lightCount;
    }

    //Ask for World State and count them
    int ShowCurrentFireNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("Res_fire"))
        {
            return worldStates.GetStates()["Res_fire"];
        }

        return 0;
    }

    int ShowCurrentLightNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("Res_light"))
        {
            return worldStates.GetStates()["Res_light"];
        }

        return 0;
    }

    int ShowCurrentAngelNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("UI_Avail_angel")) 
        {
            return worldStates.GetStates()["UI_Avail_angel"];
        }

        return 0; 
    }

    int ShowCurrentDevilNumb()
    {
        WorldStates worldStates = Worlds.Instance.GetWorld();

        if (worldStates.HasState("UI_Avail_devil"))
        {
            return worldStates.GetStates()["UI_Avail_devil"];
        }

        return 0;
    }
}
