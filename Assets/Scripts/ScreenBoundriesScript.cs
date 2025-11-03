using UnityEngine;

public class ScreenBoundriesScriptMobile : MonoBehaviour
{
    [HideInInspector]
    public Vector3 screenPoint, offset;
    [HideInInspector]
    public float minX, maxX, minY, maxY;
    public float padding = 0.02f;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("Camera.main не найдена!");
            return;
        }

        // Используем правильную Z координату для UI Canvas
        float zDistance = Mathf.Abs(mainCam.transform.position.z);

        Vector3 lowerLeft = mainCam.ScreenToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 upperRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));

        float widthReduction = (upperRight.x - lowerLeft.x) * padding;
        float heightReduction = (upperRight.y - lowerLeft.y) * padding;

        minX = lowerLeft.x + widthReduction;
        maxX = upperRight.x - widthReduction;
        minY = lowerLeft.y + heightReduction;
        maxY = upperRight.y - heightReduction;
    }

    public Vector2 GetClampedPosition(Vector3 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, minX, maxX),
            Mathf.Clamp(position.y, minY, maxY)
        );
    }
}
