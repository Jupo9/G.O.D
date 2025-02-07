using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GOD : MonoBehaviour
{
    /// <summary>
    /// This Script represent the all mighty GOD with too many variables and if statements
    /// The idea here is that the GOD will give the player 5 minutes per wave, start with 4 ressources
    /// but the ressources get 4 higher with every wave so after. I tried 3 minutes ones but this was nearly impossible
    /// with the building logic because the angels and devils are to slow at the begin. espacially when they get more Action to care about
    /// </summary>
    public bool plusFire = false;
    public bool plusLight = false;
    public bool fullFire = false;
    public bool fullLight = false;

    public int needCounter = 0;
    public float timer = 0f;

    public TextMeshProUGUI needFire;
    public TextMeshProUGUI needLight;
    public TextMeshProUGUI waveCounterUI;
    public GameObject fireRessource;
    public GameObject lightRessource;
    [SerializeField] TextMeshProUGUI timerText;

    private bool getFire = false;
    private bool getLight = false;

    public int wantFire;
    public int wantLight;
    private int chargeRes;
    private int currentFire;
    private int currentLight;

    private int balanceFire = 0;
    private int waveCounter = 0;


    private void Start()
    {
        HappyGOD();
        StartCoroutine(TimerCountdown());

        fireRessource.SetActive(false);
        lightRessource.SetActive(false);
    }

    private void Update()
    {
        needFire.text = "" + wantFire;
        needLight.text = "" + wantLight;

        if (plusFire)
        {
            plusFire = false;
            IncreaseFireRessource();
        }
        if (plusLight) 
        {
            plusLight = false;
            IncreaseLightRessource();
        }

        if (currentFire >= 1 && getFire)
        {
            getFire = false;
            fireRessource.SetActive(true);
        }
        if (currentLight >= 1 && getLight)
        {
            getLight = false;
            lightRessource.SetActive(true);
        }
        if (currentFire == 0 && !getFire)
        {
            getFire = true;
            fireRessource.SetActive(false);
        }
        if (currentLight == 0 && !getLight)
        {
            getLight = true;
            lightRessource.SetActive(false);
        }

        if (wantFire == 0f && !fullFire)
        {
            fullFire = true;
        }

        if (wantLight == 0f && !fullLight)
        {
            fullLight = true;
        }

        if (fullFire && fullLight)
        {
            HappyGOD();
        }

        if (timer <= 0)
        {
            SceneManager.LoadScene("EndScreen");
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
        currentFire += 1;
        wantFire -= 1;
    }

    public void IncreaseLightRessource() 
    {
        currentLight += 1;
        wantLight -= 1;
    }

    /// <summary>
    /// i tried to balance the amount of lights and fire ressources that the GOD wants and also make it a little bit random
    /// i feels nice by the short test i did.
    /// the waveCounter provides at the beginning of the game that fire is 0, and if light get to high it will be less in the next wave.
    /// </summary>
    /// <returns></returns>
    private void HappyGOD()
    {
        fullFire = false;
        fullLight = false;

        currentFire = 0;
        currentLight = 0;

        waveCounter += 1;
        waveCounterUI.text = "" + waveCounter;

        timer = 300f;
        chargeRes = waveCounter * needCounter;

        int randomY = chargeRes / 2;

        wantFire = Random.Range(waveCounter + balanceFire, randomY + 2); 
        wantLight = chargeRes - wantFire;

        balanceFire = wantLight - waveCounter;
    }
}
