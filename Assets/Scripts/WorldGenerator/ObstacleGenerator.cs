using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    /// <summary>
    /// this script should be changed to obstacles, it is an old version of the WorldGenerator
    /// this should place random obstacles around the World after rework!
    /// So for now it is just a oldScript!!!
    /// </summary>
    [SerializeField] private GameObject fieldObject;

    [SerializeField] private int maxWSxPlus = 50;
    [SerializeField] private int maxWSxMinus = 50;
    [SerializeField] private int maxWSzPlus = 50;
    [SerializeField] private int maxWSzMinus = 50;

    [SerializeField] private int minWSxPlus = 15;
    [SerializeField] private int minWSxMinus = 15;
    [SerializeField] private int minWSzPlus = 15;
    [SerializeField] private int minWSzMinus = 15;

    [SerializeField] private int gridOffset = 1;

    [SerializeField] private float filedRemoveChance = 0.3f;

    public bool createNew = false;

    private int worldSizeXPlus;
    private int worldSizeXMinus;
    private int worldSizeZPlus;
    private int worldSizeZMinus;

    private void Start()
    {
        CreateWorld();
    }

    private void Update()
    {
        if (createNew)
        {
            createNew = false;
            GenerateNewMap();
        }
    }

    public void GenerateNewMap()
    {
        ClearWorld();
        CreateWorld();
    }

    private void CreateWorld()
    {
        worldSizeXPlus = Random.Range(minWSxPlus, maxWSxPlus);
        worldSizeXMinus = Random.Range(minWSxMinus, maxWSxMinus);
        worldSizeZPlus = Random.Range(minWSzPlus, maxWSzPlus);
        worldSizeZMinus = Random.Range(minWSzMinus, maxWSzMinus);

        for (int x = -worldSizeXMinus; x <= worldSizeXPlus; x++)
        {
            for (int z = -worldSizeZMinus; z <= worldSizeZPlus; z++)
            {
                Vector3 pos = new Vector3(x * gridOffset, 0, z * gridOffset);

                GameObject field = Instantiate(fieldObject, pos, Quaternion.identity) as GameObject;

                field.transform.SetParent(this.transform);

                if (x >= -minWSxMinus && x <= minWSxPlus && z >= -minWSzMinus && z <= minWSzPlus)
                {
                    field.GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    if (Random.value < filedRemoveChance)
                    {
                        Destroy(field);
                    }
                }
            }
        }
    }

    private void ClearWorld()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
