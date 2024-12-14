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
        GameObject[] targetTag = GameObject.FindGameObjectsWithTag("FIRE");
        HashSet<GameObject> target = new HashSet<GameObject>();

        foreach (GameObject go in targetTag)
        {
            Transform root = go.transform.root;
            target.Add(root.gameObject);
        }

        return target.Count;
    }

    int ShowCurrentLightNumb()
    {
        GameObject[] targetTag = GameObject.FindGameObjectsWithTag("LIGHT");
        HashSet<GameObject> target = new HashSet<GameObject>();

        foreach (GameObject go in targetTag)
        {
            Transform root = go.transform.root;
            target.Add(root.gameObject);
        }

        return target.Count;
    }

    int ShowCurrentAngelNumb()
    {
        GameObject[] targetTag = GameObject.FindGameObjectsWithTag("Angel");
        HashSet<GameObject> target = new HashSet<GameObject>();

        foreach (GameObject go in targetTag) 
        {
            Transform root = go.transform.root;
            target.Add(root.gameObject);
        }

        return target.Count;
    }

    int ShowCurrentDevilNumb()
    {
        GameObject[] targetTag = GameObject.FindGameObjectsWithTag("Devil");
        HashSet<GameObject> target = new HashSet<GameObject>();

        foreach (GameObject go in targetTag)
        {
            Transform root = go.transform.root;
            target.Add(root.gameObject);
        }

        return target.Count;
    }
}
