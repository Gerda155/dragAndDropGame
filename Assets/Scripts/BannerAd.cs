using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class BannerAd : MonoBehaviour, IUnityAdsInitializationListener
{
    [Header("Unity Ads Settings")]
    [SerializeField] string _androidGameId = "5979317"; // например 6123456
    [SerializeField] string _bannerAdUnitId = "Banner_Android";
    [SerializeField] bool _testMode = true;

    private string _gameId;

    void Start()
    {
        InitializeAds();
    }

    void InitializeAds()
    {
        _gameId = _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads инициализированы успешно.");
        StartCoroutine(LoadAndShowBanner());
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Ошибка инициализации Unity Ads: {error} — {message}");
    }

    IEnumerator LoadAndShowBanner()
    {
        // Подожди, пока система полностью активна
        yield return new WaitForSeconds(1f);

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Debug.Log("Загрузка баннера...");

        // Загружаем баннер
        Advertisement.Banner.Load(_bannerAdUnitId, new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        });
    }

    void OnBannerLoaded()
    {
        Debug.Log("Баннер загружен, показываю...");
        Advertisement.Banner.Show(_bannerAdUnitId);
    }

    void OnBannerError(string message)
    {
        Debug.LogError($"Ошибка при загрузке баннера: {message}");
    }

    public void ShowBanner()
    {
        StartCoroutine(LoadAndShowBanner());
    }

}
