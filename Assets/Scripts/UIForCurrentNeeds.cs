using UnityEngine;
using UnityEngine.UI;

public class UIForCurrentNeeds : MonoBehaviour
{
    public GameObject devilUI;
    public GameObject angelUI;

    [Header("Devil Need Bars")]
    public Image stainBar;
    public Image summonBar;
    public Image evilBar;
    public Image heatBar;

    [Header("Angel Need Bars")]
    public Image spiritBar;
    public Image socialBar;
    public Image purityBar;
    public Image believeBar;

    private Devil currentDevil;
    private Angel currentAngel;

    public void SetTarget(MonoBehaviour forCurrentNeeds)
    {
        currentDevil = null;
        currentAngel = null;
        devilUI.SetActive(false);
        angelUI.SetActive(false);

        if (forCurrentNeeds is Devil devil)
        {
            currentDevil = devil;
            devilUI.SetActive(true);
        }
        if (forCurrentNeeds is Angel angel)
        {
            currentAngel = angel;
            angelUI.SetActive(true);
        }
    }

    private void Update()
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        if (currentDevil != null)
        {
            stainBar.fillAmount = currentDevil.stain;
            stainBar.color = GetNeedColor(currentDevil.stain);

            summonBar.fillAmount = currentDevil.summon;
            summonBar.color = GetNeedColor(currentDevil.summon);

            evilBar.fillAmount = currentDevil.evil;
            evilBar.color = GetNeedColor(currentDevil.evil);

            heatBar.fillAmount = currentDevil.heat; 
            heatBar.color = GetNeedColor(currentDevil.heat);
        }
        else if (currentAngel != null)
        {
            purityBar.fillAmount = currentAngel.purity;
            purityBar.color = GetNeedColor(currentAngel.purity);

            socialBar.fillAmount = currentAngel.social;
            socialBar.color = GetNeedColor(currentAngel.social);

            spiritBar.fillAmount = currentAngel.spirit;
            spiritBar.color = GetNeedColor(currentAngel.spirit);

            believeBar.fillAmount = currentAngel.believe;
            believeBar.color = GetNeedColor(currentAngel.believe);
        }
    }

    private Color GetNeedColor(float value)
    {
        value = Mathf.Clamp01(value);

        if (value > 0.5f)
        {
            return Color.Lerp(Color.yellow, Color.green, (value - 0.5f) * 2f);
        }
        else
        {
            return Color.Lerp(Color.red, Color.yellow, value * 2f);
        }
    }

}
