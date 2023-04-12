using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using PlayFab.ClientModels;
using PlayFab;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Image fader;
    [SerializeField] private Animator snake;

    [SerializeField] Button soundOn;
    [SerializeField] Button soundOff;

    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject leaderboardController; 

    [SerializeField] private BannerAdManager _bannerAdManagerPrefab;
    private GameObject _bannerObject;

    public static string playfabId;

    private void Awake()
    {
        if(!PlayFabClientAPI.IsClientLoggedIn())
            PlayfabLogin();
    }

    private void PlayfabLogin()
    {
        var request = new LoginWithCustomIDRequest()
        {
            /*CustomId = System.Guid.NewGuid().ToString()*/
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, 
        resultCallback =>{

            Debug.Log("Login Successfull " +  resultCallback.PlayFabId);
            playfabId = resultCallback.PlayFabId;
            
            PushNotificationController.instance.SendPushNotification("Connected to server successfully!");    
        },

        Error => {
            PushNotificationController.instance.SendPushNotification("Unable to connect to the server. Make sure your internet is connected and restart the game.");
        });
    }

    private void Start()
    {
        if(PlayerPrefs.GetInt("Mute") == 1)
        {
            soundOff.gameObject.SetActive(true);
            soundOn.gameObject.SetActive(false);
        }
        else
        {
            soundOn.gameObject.SetActive(true);
            soundOff.gameObject.SetActive(false);        
        }

        snake.gameObject.SetActive(false);

        fader.DOFade(1, 0).OnComplete(delegate(){
            fader.gameObject.SetActive(true);
            fader.DOFade(0, 1).OnComplete(delegate(){
                fader.gameObject.SetActive(false);

                snake.gameObject.SetActive(true);
                snake.Play("MainMenuAnimation");
            });
        });
    }
    
    public void PlayButton_OnClick(string sceneName)
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
        fader.DOFade(0, 0).OnComplete(delegate(){
            fader.gameObject.SetActive(true);
            fader.DOFade(1, 1).OnComplete(delegate(){
                SceneManager.LoadScene(sceneName);
            });
        });
    }

    public void SoundOnButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick2);

        soundOn.gameObject.SetActive(false);
        soundOff.gameObject.SetActive(true);
        SoundManager.instance.Mute();
    }

    public void SoundOffButton_OnClick()
    {
        soundOff.gameObject.SetActive(false);
        soundOn.gameObject.SetActive(true);
        SoundManager.instance.UnMute();  

        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
    }

    public void LeaderboardButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
        _bannerObject = Instantiate(_bannerAdManagerPrefab.gameObject, Vector3.zero, Quaternion.identity);
        leaderboardController.GetComponent<SetupLeaderboard>().Setup();
    }

    public void LeaderboardCloseButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick2);
        Destroy(_bannerObject);

        leaderboardPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(delegate(){
            leaderboardPanel.SetActive(false);
            leaderboardController.GetComponent<SetupLeaderboard>().ClearLeaderboard();
        });        
    }
}
