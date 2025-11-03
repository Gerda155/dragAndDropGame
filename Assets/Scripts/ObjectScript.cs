using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectScript : MonoBehaviour
{
    public GameObject[] vehicles;
    public Transform[] dropPlaces;
    public Transform[] spawnPoints;
    [HideInInspector] public Vector2[] startCoordinates;
    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;
    [HideInInspector] public Vector2[] dropStartCoordinates;
    [HideInInspector] public bool rightPlace = false;
    public bool[] onRightPlaces;
    public static GameObject lastDragged = null;
    public static bool drag = false;
    public GameObject winPanel;
    public GameObject losePanel;
    public float gameTime;
    private bool timerRunning = false;
    public Text timeText;
    public GameObject[] stars;
    public Transform starsParent;
    public bool gameEnded = false;

    void Awake()
    {
        startCoordinates = new Vector2[vehicles.Length];
        Transform[] availablePoints = spawnPoints.Clone() as Transform[];

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (availablePoints.Length == 0) break;

            int randomIndex = Random.Range(0, availablePoints.Length);
            Transform point = availablePoints[randomIndex];

            RectTransform vehicleRect = vehicles[i].GetComponent<RectTransform>();
            vehicleRect.localPosition = point.localPosition;
            startCoordinates[i] = point.localPosition;

            float randomRotation = Random.Range(-180f, 180f);
            vehicleRect.localRotation = Quaternion.Euler(0f, 0f, randomRotation);

            float randomScale = Random.Range(0.8f, 1.1f);
            vehicleRect.localScale = new Vector3(randomScale, randomScale, 1f);

            availablePoints = RemoveAt(availablePoints, randomIndex);
        }

        dropStartCoordinates = new Vector2[dropPlaces.Length];
        Transform[] availableDropPoints = spawnPoints.Clone() as Transform[];

        for (int i = 0; i < dropPlaces.Length; i++)
        {
            if (availableDropPoints.Length == 0) break;

            int randomIndex = Random.Range(0, availableDropPoints.Length);
            Transform point = availableDropPoints[randomIndex];

            dropPlaces[i].localPosition = point.localPosition;
            dropStartCoordinates[i] = point.localPosition;

            availableDropPoints = RemoveAt(availableDropPoints, randomIndex);
        }
    }

    void Start()
    {
        onRightPlaces = new bool[vehicles.Length];
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        gameTime = 0f;
        timerRunning = true;
        gameEnded = false;
    }

    void Update()
    {
        if (timerRunning && !gameEnded)
            gameTime += Time.deltaTime;
    }

    public void CheckWin()
    {
        if (gameEnded) return;

        for (int i = 0; i < onRightPlaces.Length; i++)
        {
            if (!onRightPlaces[i]) return;
        }

        timerRunning = false;
        gameEnded = true;

        Camera camScriptCamera = Camera.main;
        if (camScriptCamera != null)
        {
            CameraScript camScript = camScriptCamera.GetComponent<CameraScript>();
            if (camScript != null)
                camScript.MoveToCenterSmooth(0.7f, true);
        } 

        if (winPanel != null) winPanel.SetActive(true);
        if (effects != null && audioCli.Length > 10) effects.PlayOneShot(audioCli[16]);

        int hours = Mathf.FloorToInt(gameTime / 3600);
        int minutes = Mathf.FloorToInt((gameTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        string timeStr = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        if (timeText != null) timeText.text = timeStr;
        Debug.Log($"Spele pavaditais laiks: {timeStr}");

        int earnedStars = (gameTime < 120f) ? 3 : (gameTime < 180f) ? 2 : 1;
        Debug.Log($"Iegutas zvaigznes: {earnedStars}");

        if (starsParent != null)
        {
            foreach (Transform child in starsParent)
                Destroy(child.gameObject);
        }

        if (stars != null && stars.Length > 0)
        {
            float spacing = 100f;
            Vector3 startPos = starsParent.localPosition - new Vector3(spacing * (earnedStars - 1) / 2f, 0f, 0f);
            for (int i = 0; i < earnedStars; i++)
            {
                if (i >= stars.Length) break;
                if (stars[i] == null) continue;

                GameObject star = Instantiate(stars[i], starsParent);
                star.transform.localScale = Vector3.zero;
                star.transform.localPosition = startPos + new Vector3(spacing * i, 0f, 0f);
                StartCoroutine(AnimateStar(star.transform));
            }
        }

        FlyingObjectsScript[] flyingObjects = FindObjectsOfType<FlyingObjectsScript>();
        foreach (var obj in flyingObjects)
        {
            Destroy(obj.gameObject);
        }
    }


    public void VehicleDestroyed(GameObject vehicle)
    {
        if (gameEnded) return;

        timerRunning = false;
        gameEnded = true;
        Debug.Log("Game Over — vehicle destroyed");
        effects.PlayOneShot(audioCli[0]);
        Camera camScriptCamera = Camera.main;
        if (camScriptCamera != null)
        {
            CameraScript camScript = camScriptCamera.GetComponent<CameraScript>();
            if (camScript != null)
                camScript.MoveToCenterSmooth(0.7f, true);
        }
        losePanel.SetActive(true);

        foreach (var v in vehicles)
        {
            v.SetActive(false);
        }

        FlyingObjectsScript[] flyingObjects = FindObjectsOfType<FlyingObjectsScript>();
        foreach (var obj in flyingObjects)
        {
            Destroy(obj.gameObject);
        }
    }

    private Transform[] RemoveAt(Transform[] array, int index)
    {
        Transform[] newArray = new Transform[array.Length - 1];
        for (int i = 0, j = 0; i < array.Length; i++)
        {
            if (i == index) continue;
            newArray[j++] = array[i];
        }
        return newArray;
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void BackToMenu() => SceneManager.LoadScene("StartScene");

    IEnumerator AnimateStar(Transform star)
    {
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1.2f, t / 0.4f);
            star.localScale = Vector3.one * scale;
            yield return null;
        }
        star.localScale = Vector3.one;
    }
}
