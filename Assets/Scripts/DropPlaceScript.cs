using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) // Убрали проверку мыши
        {
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                // Расчёт углов и размеров
                placeZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
                vehicleZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
                rotDiff = Mathf.Abs(placeZRot - vehicleZRot);

                placeSiz = GetComponent<RectTransform>().localScale;
                vehicleSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
                ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);

                // Проверка на совпадение с небольшим допуском
                if ((rotDiff <= 5 || rotDiff >= 355) &&
                    (xSizeDiff <= 0.05f && ySizeDiff <= 0.05f))
                {
                    // Закрепляем объект
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                        GetComponent<RectTransform>().anchoredPosition;
                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation =
                        GetComponent<RectTransform>().localRotation;
                    eventData.pointerDrag.GetComponent<RectTransform>().localScale =
                        GetComponent<RectTransform>().localScale;

                    objScript.rightPlace = true;

                    for (int i = 0; i < objScript.vehicles.Length; i++)
                    {
                        if (objScript.vehicles[i].CompareTag(eventData.pointerDrag.tag))
                        {
                            objScript.onRightPlaces[i] = true;
                            break;
                        }
                    }

                    objScript.CheckWin();

                    // Звук
                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage": objScript.effects.PlayOneShot(objScript.audioCli[1]); break;
                        case "Medicine": objScript.effects.PlayOneShot(objScript.audioCli[2]); break;
                        case "Fire": objScript.effects.PlayOneShot(objScript.audioCli[3]); break;
                        case "cement": objScript.effects.PlayOneShot(objScript.audioCli[4]); break;
                        case "buss": objScript.effects.PlayOneShot(objScript.audioCli[5]); break;
                        case "b2": objScript.effects.PlayOneShot(objScript.audioCli[13]); break;
                        case "tractor5": objScript.effects.PlayOneShot(objScript.audioCli[14]); break;
                        case "exalator": objScript.effects.PlayOneShot(objScript.audioCli[8]); break;
                        case "police": objScript.effects.PlayOneShot(objScript.audioCli[9]); break;
                        case "e46": objScript.effects.PlayOneShot(objScript.audioCli[10]); break;
                        case "e61": objScript.effects.PlayOneShot(objScript.audioCli[11]); break;
                        case "tractor": objScript.effects.PlayOneShot(objScript.audioCli[12]); break;
                        default: Debug.Log("Unknown tag detected"); break;
                    }
                }
            }
            else
            {
                // Обратное перемещение объекта на старт
                objScript.rightPlace = false;
                objScript.effects.PlayOneShot(objScript.audioCli[15]);

                for (int i = 0; i < objScript.vehicles.Length; i++)
                {
                    if (objScript.vehicles[i].CompareTag(eventData.pointerDrag.tag))
                    {
                        eventData.pointerDrag.GetComponent<RectTransform>().localPosition =
                            objScript.startCoordinates[i];
                        break;
                    }
                }
            }
        }
    }
}
