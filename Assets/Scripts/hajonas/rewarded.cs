using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class rewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
    private string _adUnitId;

    [SerializeField] private Button _rewardedAdButton;

    public HanoiGameManager hanoiManager;

    private bool adLoaded = false;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (hanoiManager == null)
            hanoiManager = FindFirstObjectByType<HanoiGameManager>();

        // Если кнопка назначена в инспекторе, сразу привязываем
        if (_rewardedAdButton != null)
            SetButton(_rewardedAdButton);
    }

    private void Start()
    {
        LoadAd();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Unity Ads not initialized yet.");
            return;
        }

        Advertisement.Load(_adUnitId, this);
    }

    public void SetButton(Button button)
    {
        if (button == null) return;

        _rewardedAdButton = button;
        _rewardedAdButton.onClick.RemoveAllListeners();
        _rewardedAdButton.onClick.AddListener(ShowAd);
        _rewardedAdButton.interactable = adLoaded;
    }

    public void ShowAd()
    {
        if (!Advertisement.isInitialized || !adLoaded)
        {
            Debug.Log("Rewarded ad not ready!");
            return;
        }

        _rewardedAdButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
        adLoaded = false;
    }

    // ---------------- LOAD LISTENER ----------------

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId != _adUnitId) return;

        Debug.Log("Rewarded ad loaded!");
        adLoaded = true;

        if (_rewardedAdButton != null)
            _rewardedAdButton.interactable = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Failed to load rewarded ad: {message}");
        StartCoroutine(RetryLoad(5f));
    }

    private IEnumerator RetryLoad(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadAd();
    }

    // ---------------- SHOW LISTENER ----------------

    public void OnUnityAdsShowStart(string placementId) => Debug.Log("Rewarded ad started");
    public void OnUnityAdsShowClick(string placementId) => Debug.Log("Rewarded ad clicked");

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == _adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Rewarded ad completed!");

            if (hanoiManager == null)
                hanoiManager = FindFirstObjectByType<HanoiGameManager>();

            hanoiManager?.ReduceMoves(10);

            if (_rewardedAdButton != null)
                _rewardedAdButton.interactable = false;

            StartCoroutine(RetryLoad(10f));
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Failed to show rewarded ad: {message}");
        StartCoroutine(RetryLoad(5f));
    }
}
