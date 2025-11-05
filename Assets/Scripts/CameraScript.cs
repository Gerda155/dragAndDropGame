using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    public float minZoom = 150f;
    private float maxZoom;
    public float puncZoomSpeed = 0.9f, mouseZoomSpeed = 150f;
    public float mouseFollowSpeed = 1f, touchPanSpeed = 1f;
    public ScreenBoundriesScript screenBoundries;
    public Camera cam;
    public float centerMoveDuration = 0.6f;

    [HideInInspector] public float startZoom;

    Vector2 lastTouchPos;
    int panFingerId = -1;
    bool isTouchPanning = false;

    float lastTapTime = 0f;
    public float doubleTapMaxDelay = 0.4f;
    public float doubleTapMaxDistance = 100f;

    private Coroutine centerCoroutine = null;

    private void Awake()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (screenBoundries == null) screenBoundries = FindObjectOfType<ScreenBoundriesScript>();
    }

    void Start()
    {
        startZoom = cam.orthographicSize;
        if (screenBoundries != null) screenBoundries.RecalculateBounds();
        transform.position = (screenBoundries != null)
            ? screenBoundries.GetClampedCameraPosition(transform.position)
            : transform.position;
    }

    void Update()
    {
        if (TransformationScript.isTransforming)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        DesktopFollowCursor();
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
            cam.orthographicSize -= scroll * mouseZoomSpeed;
#else
        HandleTouch();
#endif

        if (Input.touchCount == 2)
            HandlePinch();

        UpdateMaxZoom();

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        if (screenBoundries != null)
        {
            screenBoundries.RecalculateBounds();
            transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
        }
    }

    void DesktopFollowCursor()
    {
        Vector3 mouse = Input.mousePosition;
        if (mouse.x < 0 || mouse.x > Screen.width || mouse.y < 0 || mouse.y > Screen.height)
            return;

        Vector3 screenPoint = new Vector3(mouse.x, mouse.y, cam.nearClipPlane);
        Vector3 targetWorld = cam.ScreenToWorldPoint(screenPoint);
        Vector3 desired = new Vector3(targetWorld.x, targetWorld.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, mouseFollowSpeed * Time.deltaTime);
    }

    void HandleTouch()
    {
        if (Input.touchCount != 1) return;

        Touch t = Input.GetTouch(0);

        if (IsTouchingUIButton(t.position)) return;

        if (t.phase == TouchPhase.Began)
        {
            float dt = Time.time - lastTapTime;
            if (dt <= doubleTapMaxDelay && Vector2.Distance(t.position, lastTouchPos) <= doubleTapMaxDistance)
            {
                MoveToCenterSmooth(centerMoveDuration, true);
                lastTapTime = 0f;
            }
            else
            {
                lastTapTime = Time.time;
            }

            lastTouchPos = t.position;
            panFingerId = t.fingerId;
            isTouchPanning = true;
        }
        else if (t.phase == TouchPhase.Moved && isTouchPanning && t.fingerId == panFingerId)
        {
            Vector2 delta = t.position - lastTouchPos;
            transform.Translate(ScreenDeltaToWorldDelta(delta) * touchPanSpeed, Space.World);
            lastTouchPos = t.position;
        }
        else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            isTouchPanning = false;
            panFingerId = -1;
        }
    }

    bool IsTouchingUIButton(Vector2 touchPos)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = touchPos };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach (RaycastResult result in results)
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                return true;
        return false;
    }

    void HandlePinch()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
        float currDist = (t0.position - t1.position).magnitude;
        cam.orthographicSize -= (currDist - prevDist) * puncZoomSpeed;
    }

    Vector3 ScreenDeltaToWorldDelta(Vector2 delta)
    {
        float worldPerPixel = (cam.orthographicSize * 2f) / Screen.height;
        return new Vector3(delta.x * worldPerPixel, delta.y * worldPerPixel, 0f);
    }

    void UpdateMaxZoom()
    {
        if (screenBoundries == null || cam == null)
            return;

        Rect wb = screenBoundries.worldBounds;
        float maxZoomHeight = wb.height / 2f;
        float maxZoomWidth = (wb.height / 2f) / cam.aspect;
        maxZoom = Mathf.Min(maxZoomHeight, maxZoomWidth);
    }

    // плавное центрирование камеры
    public void MoveToCenterSmooth(float duration = 0.6f, bool resetZoomToStart = true)
    {
        if (centerCoroutine != null) StopCoroutine(centerCoroutine);
        centerCoroutine = StartCoroutine(MoveToCenterCoroutine(duration, resetZoomToStart));
    }

    private IEnumerator MoveToCenterCoroutine(float duration, bool resetZoom)
    {
        if (screenBoundries == null)
            yield break;

        Vector3 centerPos = new Vector3(
            (screenBoundries.minX + screenBoundries.maxX) * 0.5f,
            (screenBoundries.minY + screenBoundries.maxY) * 0.5f,
            transform.position.z);

        Vector3 targetPos = screenBoundries.GetClampedCameraPosition(centerPos);

        float elapsed = 0f;
        Vector3 initialPos = transform.position;
        float initialZoom = cam.orthographicSize;
        float targetZoom = resetZoom ? startZoom : initialZoom;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(initialPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(initialZoom, targetZoom, t);
            yield return null;
        }

        transform.position = targetPos;
        cam.orthographicSize = targetZoom;
        centerCoroutine = null;
    }

    // ⚡ метод для проигрыша — камера отдаляется и центрируется
    public void MoveToCenterAndZoomOut(float duration = 1.2f)
    {
        if (centerCoroutine != null) StopCoroutine(centerCoroutine);
        centerCoroutine = StartCoroutine(MoveToCenterAndZoomOutCoroutine(duration));
    }

    private IEnumerator MoveToCenterAndZoomOutCoroutine(float duration)
    {
        if (screenBoundries == null) yield break;

        Vector3 centerPos = new Vector3(
            (screenBoundries.minX + screenBoundries.maxX) * 0.5f,
            (screenBoundries.minY + screenBoundries.maxY) * 0.5f,
            transform.position.z);

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        float startZoom = cam.orthographicSize;
        float endZoom = maxZoom;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(startPos, centerPos, t);
            cam.orthographicSize = Mathf.Lerp(startZoom, endZoom, t);
            yield return null;
        }

        transform.position = centerPos;
        cam.orthographicSize = endZoom;
        centerCoroutine = null;
    }
}
