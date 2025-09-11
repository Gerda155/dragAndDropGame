using UnityEngine;

public class ParticleChecker : MonoBehaviour
{
    public ParticleSystem particleSystemToCheck;

    void Start()
    {
        if (particleSystemToCheck == null)
        {
            Debug.LogError("Particle System не назначена!");
            return;
        }

        CheckLayer();
        CheckRenderer();
        CheckPlayStatus();
    }

    void CheckLayer()
    {
        int layer = particleSystemToCheck.gameObject.layer;
        Camera mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("Main Camera не найдена!");
            return;
        }

        if ((mainCam.cullingMask & (1 << layer)) == 0)
            Debug.LogWarning("Слой партиклов НЕ включен в Culling Mask камеры!");
        else
            Debug.Log("Слой партиклов OK");
    }

    void CheckRenderer()
    {
        var renderer = particleSystemToCheck.GetComponent<ParticleSystemRenderer>();
        if (renderer == null)
        {
            Debug.LogError("ParticleSystemRenderer не найден!");
            return;
        }

        if (renderer.material == null)
            Debug.LogWarning("У партиклов нет материала!");
        else
            Debug.Log("Материал партиклов OK");

        if (renderer.sortingLayerName == "")
            Debug.LogWarning("Sorting Layer пустой, может быть скрыт за другими объектами");
        else
            Debug.Log($"Sorting Layer: {renderer.sortingLayerName}, Order: {renderer.sortingOrder}");
    }

    void CheckPlayStatus()
    {
        if (!particleSystemToCheck.isPlaying && !particleSystemToCheck.main.playOnAwake)
            Debug.LogWarning("Партиклы не играют и Play On Awake выключен! Нужно вызвать Play() вручную.");
        else
            Debug.Log("Play On Awake / текущий статус OK");
    }
}