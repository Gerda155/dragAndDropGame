using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float maxZoom = 300f;
    public float minZoom = 150f;
    public float panSpeed = 6f;
    public float zoomStep = 50f;
    public float zoomSmooth = 5f;
    private Camera cam;
    private float startZoom;
    private float targetZoom;

    private float x, y;
    private Vector3 bottomLeft, topRight;
    private float cameraMaxX, cameraMinX, cameraMaxY, cameraMinY;

    void Start()
    {
        cam = GetComponent<Camera>();
        startZoom = cam.orthographicSize;
        targetZoom = startZoom;

        topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));
        cameraMaxX = topRight.x;
        cameraMinX = bottomLeft.x;
        cameraMaxY = topRight.y;
        cameraMinY = bottomLeft.y;
    }

    void Update()
    {
        x = Input.GetAxis("Mouse X") * panSpeed;
        y = Input.GetAxis("Mouse Y") * panSpeed;
        transform.Translate(x, y, 0);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            float oldSize = cam.orthographicSize;

            if (scroll > 0 && targetZoom > minZoom)
                targetZoom -= zoomStep;
            else if (scroll < 0)
            {
                if (targetZoom < startZoom)
                    targetZoom = Mathf.Min(targetZoom + zoomStep, startZoom);
                else if (targetZoom < maxZoom)
                    targetZoom += zoomStep;
            }

            cam.orthographicSize = targetZoom;

            Vector3 newMouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = mouseWorldPos - newMouseWorldPos;
            transform.position += new Vector3(offset.x, offset.y, 0);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSmooth);

        topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));

        if (topRight.x > cameraMaxX)
            transform.position = new Vector3(transform.position.x - (topRight.x - cameraMaxX), transform.position.y, transform.position.z);
        if (topRight.y > cameraMaxY)
            transform.position = new Vector3(transform.position.x, transform.position.y - (topRight.y - cameraMaxY), transform.position.z);
        if (bottomLeft.x < cameraMinX)
            transform.position = new Vector3(transform.position.x + (cameraMinX - bottomLeft.x), transform.position.y, transform.position.z);
        if (bottomLeft.y < cameraMinY)
            transform.position = new Vector3(transform.position.x, transform.position.y + (cameraMinY - bottomLeft.y), transform.position.z);
    }
}
