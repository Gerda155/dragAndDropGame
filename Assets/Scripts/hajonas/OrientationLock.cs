using UnityEngine;

public class OrientationLock : MonoBehaviour
{
    void Start()
    {
        // Разрешаем только горизонталь
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Блокируем автоповорот
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    private void OnDestroy()
    {
        // Возвращаем настройку по умолчанию, если нужно
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
    }
}
