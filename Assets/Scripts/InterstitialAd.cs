using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    private string _adUnitId;

    public event Action OnInterstitialAdReady;
    public bool isReady = false;
    [SerializeField] private Button _interstitialAdButton;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;
        SceneManager.sceneLoaded += OnSceneLoaded; // подписка на смену сцены
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (_interstitialAdButton != null)
        {
            _interstitialAdButton.interactable = isReady;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Показываем рекламу при переходе на сцены, кроме первой
        if (scene.buildIndex != 0)
        {
            ShowInterstitial();
        }
    }

    public void OnInterstitialAdButtonClicked()
    {
        ShowInterstitial();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Unity Ads не инициализирован!");
            return;
        }

        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if (isReady)
        {
            Advertisement.Show(_adUnitId, this);
            isReady = false;
        }
        else
        {
            Debug.LogWarning("Interstitial ad не готов!");
            LoadAd();
        }
    }

    public void ShowInterstitial()
    {
        ShowAd();
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId != _adUnitId) return;

        Debug.Log("Interstitial ad загружен!");
        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = true;

        isReady = true;
        OnInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Ошибка загрузки рекламы: {message}");
        LoadAd();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Реклама кликнута!");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId != _adUnitId) return;

        Debug.Log("Interstitial ad завершена!");

        // Только для сцен кроме первой
        if (SceneManager.GetActiveScene().buildIndex != 0 && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            StartCoroutine(SlowDownTimeTemporarily(30f));
        }

        LoadAd();
    }

    private IEnumerator SlowDownTimeTemporarily(float seconds)
    {
        // Ждем кадр, чтобы реклама точно закрылась
        yield return new WaitForEndOfFrame();

        Time.timeScale = 0.4f;
        Debug.Log($"Время замедлено до 0.4x на {seconds} секунд");
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1f;
        Debug.Log("Время восстановлено");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Ошибка показа рекламы: {message}");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Реклама показана!");
        // НЕ трогаем Time.timeScale — Unity Ads сама замораживает игру на телефоне
    }

    public void SetButton(Button button)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnInterstitialAdButtonClicked);
        _interstitialAdButton = button;
        _interstitialAdButton.interactable = false;
    }
}
