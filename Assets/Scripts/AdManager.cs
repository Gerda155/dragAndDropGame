using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;

    [SerializeField] bool turnOffInterstitialAd = false;
    private bool firstAdShown = false;
    private bool firstSceneLoad = false;

    public static AdManager Instance { get; private set; }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void HandleAdsInitialized()
    {
        if (!turnOffInterstitialAd)
        {
            interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
            interstitialAd.LoadAd();
        }
    }

    private void HandleInterstitialReady()
    {
        // показываем при инициализации
        if (!firstAdShown)
        {
            interstitialAd.ShowAd();
            firstAdShown = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // при загрузке новой сцены — заново подцепляем кнопку и рекламу
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        Button interstitialButton = null;
        GameObject interButtonObj = GameObject.FindGameObjectWithTag("inter");
        if (interButtonObj != null)
            interstitialButton = interButtonObj.GetComponent<Button>();

        if (interstitialAd != null && interstitialButton != null)
            interstitialAd.SetButton(interstitialButton);

        // чтобы не показывало при самом первом запуске
        if (!firstSceneLoad)
        {
            firstSceneLoad = true;
            Debug.Log("First scene loaded — skip ad.");
            return;
        }

        // вот тут показываем рекламу каждый раз при переходе на новую сцену
        if (!turnOffInterstitialAd)
        {
            Debug.Log("New scene loaded — showing ad...");
            interstitialAd.LoadAd(); // подгружаем заново
            StartCoroutine(ShowAdWhenReady());
        }
    }

    private System.Collections.IEnumerator ShowAdWhenReady()
    {
        while (!interstitialAd.isReady)
            yield return null;

        interstitialAd.ShowAd();
    }
}
