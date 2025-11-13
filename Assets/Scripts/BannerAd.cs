using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidBannerId = "Banner_Android";

    // public метод для вызова баннера извне
    public void ShowBanner()
    {
        StartCoroutine(WaitAndShowBanner());
    }

    private IEnumerator WaitAndShowBanner()
    {
        // ждем инициализации Unity Ads
        while (!Advertisement.isInitialized)
            yield return null;

        // загружаем баннер
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(_androidBannerId, new BannerLoadOptions
        {
            loadCallback = () =>
            {
                Advertisement.Banner.Show(_androidBannerId);
                Debug.Log("Banner shown successfully!");
            },
            errorCallback = (message) =>
            {
                Debug.LogWarning("Failed to load banner: " + message);
            }
        });
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
}
