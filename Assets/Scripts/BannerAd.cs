using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidBannerId = "Banner_Android";

    private void Start()
    {
        // Показываем баннер сразу, если Unity Ads уже инициализирован
        if (Advertisement.isInitialized)
        {
            ShowBanner();
        }
        else
        {
            // Ждём инициализации через AdManager
            AdsInitializer adsInitializer = FindFirstObjectByType<AdsInitializer>();
            if (adsInitializer != null)
                adsInitializer.OnAdsInitialized += ShowBanner;
        }
    }

    public void ShowBanner()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(_androidBannerId);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
}
