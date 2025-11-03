using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBoundriesScript screenBou;
    private int originalIndex;

    void Start()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();
        originalIndex = rectTra.GetSiblingIndex();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // На мобильном касание сразу срабатывает через EventData
        objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ObjectScript.drag = true;
        ObjectScript.lastDragged = eventData.pointerDrag;
        canvasGro.blocksRaycasts = false;
        canvasGro.alpha = 0.6f;

        int lastIndex = transform.parent.childCount - 1;
        int position = Mathf.Max(0, lastIndex - 1);
        transform.SetSiblingIndex(position);

        Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(
            eventData.position.x, eventData.position.y, screenBou.screenPoint.z));
        rectTra.position = cursorWorldPos;

        screenBou.screenPoint = Camera.main.WorldToScreenPoint(rectTra.localPosition);
        screenBou.offset = rectTra.localPosition -
            Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, screenBou.screenPoint.z));
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 curScreenPoint = new Vector3(eventData.position.x, eventData.position.y, screenBou.screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + screenBou.offset;
        rectTra.position = screenBou.GetClampedPosition(curPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ObjectScript.drag = false;
        canvasGro.blocksRaycasts = true;
        canvasGro.alpha = 1.0f;

        if (objectScr.rightPlace)
        {
            canvasGro.blocksRaycasts = false;
            ObjectScript.lastDragged = null;
        }

        objectScr.rightPlace = false;
        rectTra.SetSiblingIndex(originalIndex);
    }
}
