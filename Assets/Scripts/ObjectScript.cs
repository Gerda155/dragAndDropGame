using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static GameObject lastDragged = null;
    public static bool drag = false;


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

}