using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GOD : MonoBehaviour
{
    public float wantFire = 0f;
    public float wantLight = 0f;
    public float timer = 0f;
    public TextMeshProUGUI needFire;
    public TextMeshProUGUI needLight;
    public GameObject fireRessource;
    public GameObject lightRessource;
    [SerializeField] TextMeshProUGUI timerText;

    private float currentFire = 0f;
    private float currentLight = 0f;

    private void Start()
    {
       StartCoroutine(TimerCountdown());

        fireRessource.SetActive(false);
        lightRessource.SetActive(false);

    }

    private void Update()
    {
        needFire.text = "Fire: " + wantFire;
        needLight.text = "Light: " + wantLight;

        if (currentFire >= 1)
        {
            fireRessource.SetActive(true);
        }
        if (currentFire == 0)
        {
            fireRessource.SetActive(false);
        }
        if (currentLight >= 1)
        {
            lightRessource.SetActive(true);
        }
        if (currentLight == 0)
        {
            lightRessource.SetActive(false);
        }
        
        if (wantFire == 0f && wantLight == 0f)
        {
            HappyGOD();
        }
    }

    IEnumerator TimerCountdown() 
    {
        while(timer > 0)
        {
            timer -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return null;
        }

        timerText.color = Color.red;
        timerText.text = "00:00";
        StopAllCoroutines();
        ///Game OVER
    }

    public void IncreaseFireRessource() 
    {
        currentFire += 1f;
        wantFire -= 1f;
    }

    public void IncreaseLightRessource() 
    {
        currentLight += 1f;
        wantLight -= 1f;
    }

    private void HappyGOD()
    {
        timer = 300f;
        wantFire = 5f;
        wantLight = 5f;
        currentFire = 0f;
        currentLight = 0f;
    }
}
