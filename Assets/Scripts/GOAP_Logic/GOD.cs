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


    private float currentTime;


    private void Start()
    {
        currentTime = timer;

        StartCoroutine("TimerCountdown");

    }

    private void Update()
    {
        needFire.text = "Fire: " + wantFire;
        needLight.text = "Light: " + wantLight;
    }

    IEnumerator TimerCountdown() 
    {
        while(currentTime > 0)
        {
            timer -= Time.deltaTime;

            yield return null;
        }
    }

}
