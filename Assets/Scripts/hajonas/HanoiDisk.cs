using UnityEngine;
using UnityEngine.EventSystems;

public class HanoiDisk : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int diskSize = 1;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private HanoiTower currentTower;
    private Camera uiCamera;
    private Vector3 dragOffsetWorld;
    private bool isDraggingAllowed = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (canvas != null)
        {
            uiCamera = canvas.worldCamera;
        }
        else
        {
            Debug.LogError("Canvas not found for HanoiDisk");
        }
    }

    public void SetTower(HanoiTower tower)
    {
        currentTower = tower;
    }

    public HanoiTower GetTower()
    {
        return currentTower;
    }

    private bool ScreenPointToWorld(Vector2 screenPoint, out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;
        if (uiCamera == null) return false;

        float z = Mathf.Abs(uiCamera.transform.position.z - transform.position.z);
        Vector3 sp = new Vector3(screenPoint.x, screenPoint.y, z);
        worldPoint = uiCamera.ScreenToWorldPoint(sp);
        return true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraggingAllowed = false;

        if (currentTower != null && !currentTower.IsTopDisk(this))
        {
            return;
        }

        isDraggingAllowed = true;
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        Vector3 pointerWorld;
        if (ScreenPointToWorld(eventData.position, out pointerWorld))
        {
            dragOffsetWorld = transform.position - pointerWorld;
        }
        else
        {
            dragOffsetWorld = Vector3.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingAllowed)
            return;

        Vector3 pointerWorld;
        if (ScreenPointToWorld(eventData.position, out pointerWorld))
        {
            transform.position = pointerWorld + dragOffsetWorld;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingAllowed)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        HanoiTower targetTower = null;

        if (eventData.pointerEnter != null)
        {
            targetTower = eventData.pointerEnter.GetComponent<HanoiTower>();
            if (targetTower == null)
            {
                targetTower = eventData.pointerEnter.GetComponentInParent<HanoiTower>();
            }
        }

        if (targetTower != null && targetTower.CanPlaceDisk(this))
        {
            HanoiTower previousTower = currentTower;

            if (currentTower != null)
            {
                currentTower.RemoveDisk(this);
            }

            targetTower.AddDisk(this);

            if (previousTower != targetTower)
            {
                HanoiGameManager gameManager = Object.FindFirstObjectByType<HanoiGameManager>();
                if (gameManager != null)
                {
                    gameManager.OnDiskMoved();
                }
            }
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }

        isDraggingAllowed = false;
    }
}
