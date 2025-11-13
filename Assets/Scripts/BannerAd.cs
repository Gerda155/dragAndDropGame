using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidBannerId = "Banner_Android";

    private void Start()
    {
        StartCoroutine(WaitAndShowBanner());
    }

    private IEnumerator WaitAndShowBanner()
    {
        while (!Advertisement.isInitialized)
            yield return null;

        ShowBanner();
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
