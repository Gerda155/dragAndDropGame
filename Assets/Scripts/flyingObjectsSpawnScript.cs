using UnityEngine;

public class flyingObjectsSpawnScript : MonoBehaviour
{
    ScreenBoundriesScript screenBoundriesScript;
    public GameObject[] cloudsPrefabs;
    public GameObject[] objectPrefabs;
    public Transform spawnPoint;

    public float cloudSpawnInterval = 2f;
    public float objectSpawnInterval = 3f;
    private float minY, maxY;
    private float cloudMinSpeed = 1.5f;
    private float objectMinSpeed = 2f;
    private float cloudMaxSpeed = 150f;
    private float objectMaxSpeed = 200f;

    void Start()
    {
        screenBoundriesScript = FindAnyObjectByType<ScreenBoundriesScript>();
        minY = screenBoundriesScript.minY;
        maxY = screenBoundriesScript.maxY;
        InvokeRepeating(nameof(SpawnCloud), 0f, cloudSpawnInterval);
        InvokeRepeating(nameof(SpawnObject), 0f, cloudSpawnInterval);

    }

    void SpawnCloud()
    {
        if (cloudsPrefabs.Length == 0)
            return;

        GameObject cloudPrefab = cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length)];
        float y = Random.Range(minY, maxY);

        Vector3 spawPosition = new Vector3(spawnPoint.position.x, y, spawnPoint.position.z);

        GameObject cloud = Instantiate(cloudPrefab, spawPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(cloudMinSpeed, cloudMaxSpeed);
        flyingObjectsScript controller = cloud.GetComponent<flyingObjectsScript>();
        controller.speed = movementSpeed;
    }
    
    void SpawnObject()
    {
        if (objectPrefabs.Length == 0)
            return;

        GameObject objectPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        float y = Random.Range(minY, maxY);

        Vector3 spawPosition = new Vector3(-spawnPoint.position.x, y, spawnPoint.position.z);

        GameObject flyObject = Instantiate(objectPrefab, spawPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(objectMinSpeed, objectMaxSpeed);
        flyingObjectsScript controller = flyObject.GetComponent<flyingObjectsScript>();
        controller.speed = -movementSpeed;
    }
}
