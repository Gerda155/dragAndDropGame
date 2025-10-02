using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class flyingObjectsScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float fadeDuration = 1.5f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExplosding = false;
    private Image image;
    private Color originalColor;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        originalColor = image.color;
        objectScript = FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        // <-
        if (speed > 0 && transform.position.x < (screenBoundriesScript.minX + 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutDestroy());
            isFadingOut = true;
        }

        // ->
        if (speed < 0 && transform.position.x > (screenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutDestroy());
            isFadingOut = true;
        }

        if (CompareTag("bomb") && !isExplosding && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("The cursor collided with a bomb! (without car)");
            TriggerExplosion();
        }

        if (ObjectScript.drag && !isFadingOut && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("The cursor collided with a flying object");

            if(ObjectScript.lastDragged != null)
            {
                StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f));
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }

            StartToDestroy();
        }
    }

    public void TriggerExplosion()
    {
        isExplosding = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[7], 5f);

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }

        image.color = Color.red;
        StartCoroutine(RecoverColor(0.4f));

        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExpload());
    }

    IEnumerator WaitBeforeExpload()
    {
        float radius = 0f;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D circleCollider))
        {
            radius = circleCollider.radius * transform.lossyScale.x;
        }

        ExploadAndDestroy(radius);
        yield return new WaitForSeconds(1f);
        ExploadAndDestroy(radius);
        Destroy(gameObject);
    }

    void ExploadAndDestroy(float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider != null && hitCollider.gameObject != gameObject)
            {
                flyingObjectsScript obj = hitCollider.gameObject.GetComponent<flyingObjectsScript>();

                if(obj != null && !obj.isExplosding)
                {
                    obj.StartToDestroy();
                }
            }
        }
    }

    public void StartToDestroy()
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutDestroy());
            isFadingOut = true;

            image.color = Color.red;
            StartCoroutine(RecoverColor(0.0f));

            objectScript.effects.PlayOneShot(objectScript.audioCli[6]);

            StartCoroutine(Vibrate());
        }
    }

    IEnumerator Vibrate()
    {
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elpased = 0f;
        float intensity = 5f;

        while (elpased < duration)
        {
            rectTransform.anchoredPosition = originalPosition + Random.insideUnitCircle * intensity;
            elpased += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutDestroy()
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;
        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        Vector3 originalScale = target.transform.localScale;
        Quaternion originalRotation = target.transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t/duration);
            float angle = Mathf.Lerp(0f, 360f, t / duration);
            target.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }
        Destroy(target);
    }

    IEnumerator RecoverColor(float s)
    {
        yield return new WaitForSeconds(s);
        image.color = originalColor;
    }
}
