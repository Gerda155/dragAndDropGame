using UnityEngine;

public class FlyingObjectsSpawnScript : MonoBehaviour
{
    private ScreenBoundriesScript screenBoundriesScript;
    public GameObject[] cloudsPrefabs;
    public GameObject[] objectPrefabs;
    public Transform spawnPoint;

    public float cloudSpawnInterval = 2f;
    public float objectSpawnInterval = 3f;
    private float minY, maxY;

    private float cloudMinSpeed = 1.5f;
    private float cloudMaxSpeed = 150f;
    private float objectMinSpeed = 2f;
    private float objectMaxSpeed = 200f;

    void Start()
    {
        screenBoundriesScript = FindAnyObjectByType<ScreenBoundriesScript>();
        minY = screenBoundriesScript.minY;
        maxY = screenBoundriesScript.maxY;

        InvokeRepeating(nameof(SpawnCloud), 0f, cloudSpawnInterval);
        InvokeRepeating(nameof(SpawnObject), 0f, objectSpawnInterval);
    }

    void SpawnCloud()
    {
        if (cloudsPrefabs.Length == 0) return;

        GameObject cloudPrefab = cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length)];
        float y = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, y, spawnPoint.position.z);

        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity, spawnPoint.parent);
        cloud.transform.SetAsLastSibling();

        float movementSpeed = Random.Range(cloudMinSpeed, cloudMaxSpeed);
        var controller = cloud.GetComponent<FlyingObjectsScript>();
        if (controller != null)
            controller.speed = movementSpeed;
    }

    void SpawnObject()
    {
        if (objectPrefabs.Length == 0) return;

        GameObject objectPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        float y = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(-spawnPoint.position.x, y, spawnPoint.position.z);

        GameObject flyObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity, spawnPoint.parent);
        flyObject.transform.SetAsLastSibling();

        float movementSpeed = Random.Range(objectMinSpeed, objectMaxSpeed);
        var controller = flyObject.GetComponent<FlyingObjectsScript>();
        if (controller != null)
            controller.speed = -movementSpeed;
    }
}
