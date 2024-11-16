using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject DevilPrefab;
    public GameObject AngelPrefab;

    [SerializeField] private float spawnTimer = 5f;
    [SerializeField] private float maxOutnumbered = 5f;

    private int creatureType;
    private int createAngel = 0;
    private int createDevil = 0;


    private void Start()
    {
        Invoke("ChooseCreature", spawnTimer);
    }

    private void ChooseCreature()
    {
        if (createAngel - createDevil >= maxOutnumbered)
        {
            creatureType = 2;
        }
        else if (createDevil - createAngel >= maxOutnumbered)
        {
            creatureType = 1;
        }
        else
        {
            creatureType = Random.Range(1, 4);
        }



        Debug.Log("Number: " + creatureType);

        if (creatureType == 1 || creatureType == 3)
        {
            createAngel++;
        }
        if (creatureType == 2)
        {
            createDevil++;
        }

        Debug.Log($"Angel: {createAngel}, Devil: {createDevil}");

        SpawnCreature();
    }

    private void SpawnCreature()
    {
        if (creatureType == 2)
        {
            Instantiate(DevilPrefab, this.transform.position, Quaternion.identity);

        }
        else if (creatureType == 1 || creatureType == 3)
        {
            Instantiate(AngelPrefab, this.transform.position, Quaternion.identity);
        }

        Invoke("ChooseCreature", spawnTimer);
    }
}
