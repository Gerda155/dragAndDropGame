using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;
    private bool firstAdShown = false;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;

    private BannerAd bannerAd;

    public static AdManager Instance { get; private set; }

    private bool firstSceneLoad = false;

    private void Awake()
    {
        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInitializer>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        adsInitializer.OnAdsInitialized += HandleAdsInitialized;
    }

    private void HandleAdsInitialized()
    {
        // Показываем баннер
        bannerAd = FindFirstObjectByType<BannerAd>();
        if (bannerAd != null)
            bannerAd.ShowBanner();

        // Загружаем Interstitial
        if (!turnOffInterstitialAd && interstitialAd != null)
        {
            interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
            interstitialAd.LoadAd();
        }

        // Загружаем Rewarded
        if (!turnOffRewardedAds && rewardedAds != null)
            rewardedAds.LoadAd();
    }

    private void HandleInterstitialReady()
    {
        if (!firstAdShown)
        {
            Debug.Log("Showing first time interstitial ad automatically!");
            interstitialAd.ShowAd();
            firstAdShown = true;
        }
        else
        {
            Debug.Log("Next interstitial ad is ready for manual show!");
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!firstSceneLoad)
        {
            firstSceneLoad = true;
            Debug.Log("First scene loaded, skip interstitial.");
            return;
        }

        Debug.Log("Scene loaded!");

        // Подключаем кнопки Interstitial
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        Button interstitialButton = GameObject.FindGameObjectWithTag("inter")?.GetComponent<Button>();
        if (interstitialAd != null && interstitialButton != null)
            interstitialAd.SetButton(interstitialButton);

        // Подключаем кнопки Rewarded
        if (rewardedAds == null)
            rewardedAds = FindFirstObjectByType<RewardedAds>();

        Button rewardedButton = GameObject.FindGameObjectWithTag("reward")?.GetComponent<Button>();
        if (rewardedAds != null && rewardedButton != null)
            rewardedAds.SetButton(rewardedButton);

        // Показ Interstitial после загрузки
        if (!turnOffInterstitialAd && interstitialAd != null)
        {
            interstitialAd.OnInterstitialAdReady += ShowInterstitialAfterLoad;
            interstitialAd.LoadAd();
        }

        // Подгружаем Rewarded на новой сцене
        if (!turnOffRewardedAds && rewardedAds != null)
            rewardedAds.LoadAd();
    }

    private void ShowInterstitialAfterLoad()
    {
        if (interstitialAd != null && interstitialAd.isReady)
            interstitialAd.ShowAd();

        interstitialAd.OnInterstitialAdReady -= ShowInterstitialAfterLoad;
    }
}
