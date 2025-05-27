using UnityEngine;

public class Spawner : MonoBehaviour
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Spawn devil and angel
    /// Timer for spawn 
    /// Randomiize spawn type (devil or angel)
    /// Balance spawn to prevent to many spawns fromn one type
    /// Spawn points for devil and angel
    /// </summary>

    [Header("SpawnArea")]
    public GameObject DevilPrefab;
    public GameObject AngelPrefab;

    [Header("SpawnSettings")]
    [SerializeField] private float spawnTimer = 5f;
    [SerializeField] private float maxOutnumbered = 5f;

    [Header("SpawnPoints")]
    [SerializeField] private Transform devilSpawnPoint;
    [SerializeField] private Transform angelSpawnPoint;

    [Header("Animations")]
    [SerializeField] private Animator devilDoorAnimator;
    [SerializeField] private Animator angelDoorAnimator;

    [Header("ParticleEffects")]
    [SerializeField] private ParticleSystem devilSpawnSmoke;
    [SerializeField] private ParticleSystem angelSpawnSmoke;

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
            devilDoorAnimator.Play("A_DevilSpawnDoor", 0, 0f);
            devilSpawnSmoke.Play();
            Invoke("DevilSpawn", 1f);
        }
        else if (creatureType == 1 || creatureType == 3)
        {
            angelDoorAnimator.Play("A_AngelSpawnDoor", 0, 0f);
            angelSpawnSmoke.Play();
            Invoke("AngelSpawn", 1f);
        }

        Invoke("ChooseCreature", spawnTimer);
    }

    private void DevilSpawn()
    {
        Instantiate(DevilPrefab, devilSpawnPoint.position, Quaternion.identity);
        Invoke("StopParticle", 4f);
    }

    private void AngelSpawn()
    {
        Instantiate(AngelPrefab, angelSpawnPoint.position, Quaternion.Euler(0, 180, 0));
        Invoke("StopParticle", 4f);
    }

    private void StopParticle()
    {
        devilSpawnSmoke.Stop();
        angelSpawnSmoke.Stop();
    }
}