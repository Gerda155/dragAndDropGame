using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
    private string _adUnitId;

    [SerializeField] private Button _rewardedAdButton;
    public FlayingObjectManager flayingObjectManager;

    private bool adLoaded = false;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (flayingObjectManager == null)
            flayingObjectManager = FindFirstObjectByType<FlayingObjectManager>();
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

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowAd);
        _rewardedAdButton = button;
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

    // ---------------- IUnityAdsLoadListener ----------------

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

    // ---------------- IUnityAdsShowListener ----------------

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Rewarded ad started");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Rewarded ad clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == _adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Rewarded ad completed!");

            // динамически ищем FlayingObjectManager, если его ещё нет
            if (flayingObjectManager == null)
                flayingObjectManager = FindFirstObjectByType<FlayingObjectManager>();

            if (flayingObjectManager != null)
                flayingObjectManager.DestroyAllFlyingObjects();

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
