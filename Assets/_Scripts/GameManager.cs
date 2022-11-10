using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool dontStartWithTutorial;
    [SerializeField] private bool includeRescueMisions;
    [SerializeField] private int startAtRound;
    private bool forceStartRound = true;

    [Header ("Score")]
    public int desviatedMeteors;
    public int savedFriends;
    public int roundsSurvived;
    private bool perdio = false;
    private bool gano = false;

    [Header("Obstacles")]
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private GameObject roundAlert;
    [SerializeField] private GameObject looseAlert;
    [SerializeField] private GameObject winAlert;
    [SerializeField] private GameObject creditsAlert;

    [Header("References")]
    [SerializeField] private RadarManager radarManager;
    [SerializeField] private CharacterController characterController;
    private Transform currentPlayerBodyTransform;

    [Header("Tutorial References")]
    [SerializeField] private GameObject canvasAlert;
    [SerializeField] private Image alertInfo;
    [SerializeField] private Button alertButton;
    [SerializeField] private List<Sprite> alertsInfoOrder;
    [SerializeField] private int buttonPressedCounter = 1;
    [SerializeField] private float delayTime = 1;
    [SerializeField] private bool buttonIsActive = true;
    [SerializeField] private bool nextButtonPressedIsDelayed = false;
    [SerializeField] private bool nextButtonPressedIsUndefined = false;
    [SerializeField] private GameObject tutorialMeteor;

    private void Start()
    {
        currentPlayerBodyTransform = characterController.physicalGO.transform;

        //NewRoundOfAsteroids(5);

        if (dontStartWithTutorial) buttonPressedCounter = 6;

        StartCoroutine(nameof(ShowTutorialScreen), new Vector2(0, 1f));
    }

    private void Update()
    {
        
    }

    public void NewRoundOfAsteroids(int asteroidNumber)
    {
        StartCoroutine(nameof(RoundOfAsteroids), asteroidNumber);
    }
    public void NewRoundOfAsteroids(int asteroidNumber, int asd)
    {
        StartCoroutine(nameof(RoundOfAsteroids));
    }
    private IEnumerator RoundOfAsteroids(int asteroidNumber)
    {
        yield return new WaitForSeconds(2f);
        roundAlert.SetActive(true);
        yield return new WaitForSeconds(2.7f);
        roundAlert.SetActive(false);

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < asteroidNumber; i++)
        {
            GameObject newMeteor = Instantiate(meteorPrefab);
            newMeteor.transform.position = Random.insideUnitCircle.normalized * Random.Range(15, 17);
            newMeteor.GetComponent<AsteroidBehaviour>().playerTransform = currentPlayerBodyTransform;
            radarManager.CreateArrowForTarget(newMeteor);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }

        switch (roundsSurvived)
        {
            //Si estamos en la ronda
            case 1:
                //Debug.Log("Checkeando Ronda 1");
                StartCoroutine(nameof(CheckIfRoundEnded), 15f);
                break;
            case 2:
                //Debug.Log("Checkeando Ronda 2");
                StartCoroutine(nameof(CheckIfRoundEnded), 20f);
                break;
            case 3:
                StartCoroutine(nameof(CheckIfRoundEnded), 23f);
                break;
            case 4:
                StartCoroutine(nameof(CheckIfRoundEnded), 23f);
                break;

            case 5:
                StartCoroutine(nameof(CheckIfRoundEnded), 23f);
                break;
        }
        
    }
    private IEnumerator CheckIfRoundEnded(float timeDelay)
    {
        //Debug.Log("Checking if round ended");
        yield return new WaitForSeconds(timeDelay);
        
        if (!perdio && !includeRescueMisions)
        {
            roundsSurvived++;
            
            if (forceStartRound)
            {
                roundsSurvived = startAtRound;
                forceStartRound = false;
            }
            
            switch (roundsSurvived)
            {
                //Empezar nivel:
                case 1:
                    NewRoundOfAsteroids(2);
                    break;
                case 2:
                    NewRoundOfAsteroids(3);
                    break;
                case 3:
                    NewRoundOfAsteroids(4);
                    break;

                case 4: //Ronda de rescate
                    Debug.Log("Ronda normal");
                    NewRoundOfAsteroids(4);
                    break;

                case 5: //Ganaste
                    Win();
                    break;
            }
        }

        if (!perdio && includeRescueMisions)
        {
            
        }
    }

    private IEnumerator ShowTutorialScreen(Vector2 infoNumberAndDelayTime)
    {
        yield return new WaitForSeconds(infoNumberAndDelayTime.y);

        int infoNumber = Mathf.FloorToInt(infoNumberAndDelayTime.x); //Debug.Log(infoNumber);
        alertInfo.sprite = alertsInfoOrder[infoNumber];
        canvasAlert.SetActive(true);

        buttonIsActive = true;
        if (infoNumberAndDelayTime.y > 1)
        {
            nextButtonPressedIsDelayed = false;
            delayTime = 1f;
        }
    }
    private IEnumerator HideTutorialScreen() //StartCoroutine(nameof(HideTutorialScreen));
    {
        yield return new WaitForSeconds(0.5f);
        canvasAlert.SetActive(false);
    }
    private IEnumerator TutotialAsteroid()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 1; i++)
        {
            GameObject newMeteor = Instantiate(meteorPrefab);
            newMeteor.transform.position = new Vector2(13, 0);
            newMeteor.GetComponent<AsteroidBehaviour>().playerTransform = currentPlayerBodyTransform;
            radarManager.CreateArrowForTarget(newMeteor);
            yield return new WaitForSeconds(3.5f);
            newMeteor.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            tutorialMeteor = newMeteor;
        }
    }

    public void ButtonPressed()
    {
        if (buttonIsActive)
        {
            buttonPressedCounter++;

            switch (buttonPressedCounter)
            {
                case 2:
                    StartCoroutine(nameof(HideTutorialScreen));
                    nextButtonPressedIsDelayed = true;
                    delayTime = 10f;
                    break;
                case 4:
                    StartCoroutine(nameof(HideTutorialScreen));
                    nextButtonPressedIsDelayed = true;
                    delayTime = 10f;
                    break;
                case 5:
                    StartCoroutine(nameof(HideTutorialScreen));
                    nextButtonPressedIsDelayed = true;
                    delayTime = 5f;
                    StartCoroutine(nameof(TutotialAsteroid));                  
                    break;
                case 6:
                    StartCoroutine(nameof(HideTutorialScreen));
                    tutorialMeteor.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    tutorialMeteor.GetComponent<AsteroidBehaviour>().NewImpulse();
                    nextButtonPressedIsDelayed = true;
                    delayTime = 15f;
                    break;
                case 7:
                    StartCoroutine(nameof(HideTutorialScreen));
                    StartCoroutine(nameof(CheckIfRoundEnded), 1f);
                    //NewRoundOfAsteroids(2);
                    nextButtonPressedIsUndefined = true;
                    break;
            }

            if (nextButtonPressedIsUndefined == false)
            {
                if (nextButtonPressedIsDelayed)
                    StartCoroutine(nameof(ShowTutorialScreen), new Vector2(buttonPressedCounter, delayTime));
                else
                    StartCoroutine(nameof(ShowTutorialScreen), new Vector2(buttonPressedCounter, 1f));
                buttonIsActive = false;
            }
        }
    }

    private Vector2 RandomPointOrbitingPosition(Vector2 origin, float minRadius, float maxRadius)
    {
        var randomDirection = (Random.insideUnitCircle * origin).normalized;
        var randomDistance = Random.Range(minRadius, maxRadius);
        var point = origin + randomDirection * randomDistance;
        return point;
    }

    public void Loose()
    {
        Debug.Log("Perdiste");
        perdio = true;
        StartCoroutine(nameof(Perdiste));
    }
    private IEnumerator Perdiste()
    {
        yield return new WaitForSeconds(5.5f);
        looseAlert.SetActive(true);
        yield return new WaitForSeconds(2.7f);
        looseAlert.SetActive(false);
        creditsAlert.SetActive(true);
        yield return new WaitForSeconds(7.5f);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void Win()
    {
        Debug.Log("Ganaste");
        StartCoroutine(nameof(Ganaste));
    }
    private IEnumerator Ganaste()
    {
        yield return new WaitForSeconds(1f);
        winAlert.SetActive(true);
        yield return new WaitForSeconds(2.7f);
        winAlert.SetActive(false);
        creditsAlert.SetActive(true);
        yield return new WaitForSeconds(7.5f);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }


}
