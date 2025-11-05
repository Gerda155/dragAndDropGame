using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class flyingObjectsScript : MonoBehaviour
{
    [HideInInspector] public float speed = 1f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    public float fadeDuration = 1.5f;

    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploding = false;
    private Image image;
    private Color originalColor;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        originalColor = image.color;

        objectScript = Object.FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        // Двигаем объект с волной
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        // Уничтожение при выходе за экран
        if (speed > 0 && transform.position.x < (screenBoundriesScript.minX + 80) && !isFadingOut)
        {
            StartToDestroy(Color.cyan);
        }
        if (speed < 0 && transform.position.x > (screenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            StartToDestroy(Color.cyan);
        }

        // Получаем позицию ввода
        if (!TryGetInputPosition(out Vector2 inputPos)) return;

        // Если бомба и курсор на ней
        if (CompareTag("bomb") && !isExploding &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPos, Camera.main))
        {
            TriggerExplosion();
        }

        // Если перетаскивание
        if (ObjectScript.drag && !isFadingOut &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPos, Camera.main))
        {
            if (ObjectScript.lastDragged != null)
            {
                if (!CompareTag("bomb"))
                {
                    // Проигрыш при попадании на объект
                    objectScript.VehicleDestroyed(ObjectScript.lastDragged);
                }

                StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f));
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }

            if (CompareTag("bomb"))
                StartToDestroy(Color.red);
            else
                StartToDestroy(Color.cyan);
        }
    }

    bool TryGetInputPosition(out Vector2 position)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        position = Input.mousePosition;
        return true;
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
        else
        {
            position = Vector2.zero;
            return false;
        }
#endif
    }

    public void TriggerExplosion()
    {
        isExploding = true;
        if (objectScript != null && objectScript.effects != null && objectScript.audioCli.Length > 6)
            objectScript.effects.PlayOneShot(objectScript.audioCli[7], 1f);

        if (TryGetComponent<Animator>(out Animator animator))
            animator.SetBool("explotion", true);

        image.color = Color.red;
        StartCoroutine(RecoverColor(0.4f));
        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExplode());
    }

    IEnumerator WaitBeforeExplode()
    {
        float radius = 0f;
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
            radius = circleCollider.radius * transform.lossyScale.x;

        ExploadAndDestroyNearbyObjects(radius);
        yield return new WaitForSeconds(1f);
        ExploadAndDestroyNearbyObjects(radius);

        Destroy(gameObject);
    }

    void ExploadAndDestroyNearbyObjects(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit != null && hit.gameObject != gameObject)
            {
                flyingObjectsScript obj = hit.GetComponent<flyingObjectsScript>();
                if (obj != null && !obj.isExploding)
                {
                    obj.StartToDestroy(Color.cyan);
                }
            }
        }
    }

    public void StartToDestroy(Color c)
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        Vector3 startScale = target.transform.localScale;
        Quaternion startRot = target.transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0f, 360f, t / duration);
            target.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        Destroy(target);
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        image.color = originalColor;
    }

    IEnumerator Vibrate()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
        Vector2 originalPos = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elapsed = 0f;
        float intensity = 5f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = originalPos + Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = originalPos;
    }
}
