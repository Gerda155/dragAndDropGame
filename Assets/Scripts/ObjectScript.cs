using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectScript : MonoBehaviour
{
    public GameObject[] vehicles;
    public Transform[] spawnPoints;
    [HideInInspector]
    public Vector2[] startCoordinates;
    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;
    [HideInInspector]
    public bool rightPlace = false;
    public bool[] onRightPlaces;
    public static GameObject lastDragged = null;
    public static bool drag = false;
    public GameObject winPanel;
    public float gameTime;
    private bool timerRunning = false;
    public Text timeText;
    public GameObject[] stars; 

    void Start()
    {
        onRightPlaces = new bool[vehicles.Length];
        winPanel.SetActive(false);
        gameTime = 0f;
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
            gameTime += Time.deltaTime;
    }



    void Awake()
    {
        startCoordinates = new Vector2[vehicles.Length];

        Transform[] availablePoints = spawnPoints.Clone() as Transform[];

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (availablePoints.Length == 0)
            {
                Debug.LogWarning("Nau brivas vietas lol!");
                break;
            }

            int randomIndex = Random.Range(0, availablePoints.Length);
            Transform point = availablePoints[randomIndex];

            vehicles[i].GetComponent<RectTransform>().localPosition = point.localPosition;
            startCoordinates[i] = point.localPosition;

            availablePoints = RemoveAt(availablePoints, randomIndex);
        }
    }

    public void CheckWin()
    {
        foreach (bool placed in onRightPlaces)
        {
            if (!placed)
                return;
        }

        timerRunning = false;

        Debug.Log("Uzvara!");
        winPanel.SetActive(true);
        effects.PlayOneShot(audioCli[10]);

        int hours = Mathf.FloorToInt(gameTime / 3600);
        int minutes = Mathf.FloorToInt((gameTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        float t = gameTime; 
        int earnedStars = 0;

        if (t <= 30f) earnedStars = 3;
        else if (t <= 60f) earnedStars = 2;
        else earnedStars = 1;

        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(i < earnedStars);
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

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene"); 
    }


}
