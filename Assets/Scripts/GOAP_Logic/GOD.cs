using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GOD : MonoBehaviour
{
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
