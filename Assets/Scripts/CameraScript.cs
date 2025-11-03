using UnityEngine;

public class CameraScriptMobile : MonoBehaviour
{
    public float maxZoom = 300f;
    public float minZoom = 150f;
    public float panSpeed = 0.1f;
    public float zoomSpeed = 0.5f;
    public float zoomSmooth = 5f;

    private Camera cam;
    private float targetZoom;

    private Vector2 lastPanPosition;
    private int panFingerId; // для отслеживания первого пальца
    private bool isPanning;
    private float lastPinchDistance;

    private Vector3 bottomLeft, topRight;
    private float cameraMaxX, cameraMinX, cameraMaxY, cameraMinY;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;

        UpdateCameraBounds();
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
                isPanning = true;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning && touch.fingerId == panFingerId)
            {
                Vector2 delta = touch.position - lastPanPosition;
                Vector3 move = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0);
                transform.Translate(move * Time.deltaTime);

                lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            float prevDistance = Vector2.Distance(touch0Prev, touch1Prev);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            float deltaDistance = prevDistance - currentDistance;

            targetZoom += deltaDistance * zoomSpeed * Time.deltaTime;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSmooth);

        // Ограничение камеры по краям
        UpdateCameraBounds();
        ClampCameraPosition();
    }

    void UpdateCameraBounds()
    {
        topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));
        cameraMaxX = topRight.x;
        cameraMinX = bottomLeft.x;
        cameraMaxY = topRight.y;
        cameraMinY = bottomLeft.y;
    }

    void ClampCameraPosition()
    {
        Vector3 pos = transform.position;
        if (topRight.x > cameraMaxX)
            pos.x -= (topRight.x - cameraMaxX);
        if (topRight.y > cameraMaxY)
            pos.y -= (topRight.y - cameraMaxY);
        if (bottomLeft.x < cameraMinX)
            pos.x += (cameraMinX - bottomLeft.x);
        if (bottomLeft.y < cameraMinY)
            pos.y += (cameraMinY - bottomLeft.y);

        transform.position = pos;
    }
}
