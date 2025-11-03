using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TransformationScript : MonoBehaviour
{
    public ObjectScript objScript;

    [Header("Rotation Buttons")]
    public Button rotateCWButton;
    public Button rotateCCWButton;

    [Header("Scale Buttons")]
    public Button scaleUpButton;
    public Button scaleDownButton;
    public Button scaleLeftButton;
    public Button scaleRightButton;

    [Header("Settings")]
    public float rotationSpeed = 90f; // градусов в секунду
    public float scaleStep = 0.01f;
    public float minScale = 0.5f;
    public float maxScale = 1.3f;

    // флаги для удержания кнопки
    private bool rotateCW, rotateCCW, scaleUp, scaleDown, scaleLeft, scaleRight;

    void Start()
    {
        // подписываем нажатия и отпускания кнопок
        AddButtonEvents(rotateCWButton, () => rotateCW = true, () => rotateCW = false);
        AddButtonEvents(rotateCCWButton, () => rotateCCW = true, () => rotateCCW = false);

        AddButtonEvents(scaleUpButton, () => scaleUp = true, () => scaleUp = false);
        AddButtonEvents(scaleDownButton, () => scaleDown = true, () => scaleDown = false);
        AddButtonEvents(scaleLeftButton, () => scaleLeft = true, () => scaleLeft = false);
        AddButtonEvents(scaleRightButton, () => scaleRight = true, () => scaleRight = false);
    }

    void Update()
    {
        if (ObjectScript.lastDragged == null) return;

        RectTransform obj = ObjectScript.lastDragged.GetComponent<RectTransform>();

        // Вращение
        if (rotateCW) obj.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        if (rotateCCW) obj.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Масштабирование
        if (scaleUp && obj.localScale.y < maxScale) obj.localScale = new Vector3(obj.localScale.x, obj.localScale.y + scaleStep, 1f);
        if (scaleDown && obj.localScale.y > minScale) obj.localScale = new Vector3(obj.localScale.x, obj.localScale.y - scaleStep, 1f);

        if (scaleLeft && obj.localScale.x > minScale) obj.localScale = new Vector3(obj.localScale.x - scaleStep, obj.localScale.y, 1f);
        if (scaleRight && obj.localScale.x < maxScale) obj.localScale = new Vector3(obj.localScale.x + scaleStep, obj.localScale.y, 1f);
    }

    // Метод для удобного добавления событий кнопок
    private void AddButtonEvents(Button button, UnityEngine.Events.UnityAction onPress, UnityEngine.Events.UnityAction onRelease)
    {
        EventTriggerListener listener = button.gameObject.GetComponent<EventTriggerListener>();
        if (listener == null)
        {
            listener = button.gameObject.AddComponent<EventTriggerListener>();
        }
        listener.onPointerDown = onPress;
        listener.onPointerUp = onRelease;
    }

    // Вспомогательный класс для обработки удержания кнопок
    private class EventTriggerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEngine.Events.UnityAction onPointerDown;
        public UnityEngine.Events.UnityAction onPointerUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke();
        }
    }
}
