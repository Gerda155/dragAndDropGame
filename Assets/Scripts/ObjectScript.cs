using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public GameObject[] vehicles;
    [HideInInspector]
    public Vector2[] startCoor;
    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;
    [HideInInspector]
    public bool rightPlace = false;
    public GameObject lastDragged = null;

    void Awake()
    {
        startCoor = new Vector2[vehicles.Length];
        for (int i = 0; i < vehicles.Length; i++)
        {
            startCoor[i] = vehicles[i].GetComponent<RectTransform>().localPosition;
        }
        
    }
}
