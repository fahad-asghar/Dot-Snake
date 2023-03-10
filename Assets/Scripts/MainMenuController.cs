using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Image fader;
    [SerializeField] private Animator snake;

    [SerializeField] Button soundOn;
    [SerializeField] Button soundOff;

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingPanelText;

    [SerializeField] private GameObject notificationObject;
    [SerializeField] private TextMeshProUGUI notificationText;

    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform topPositionsPanel; 
    [SerializeField] private GameObject leaderboardItemPrefab;

    private float scrollVerticalNormalizePosition;
    private bool canExpand = false;

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
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, 
        resultCallback =>{

            Debug.Log("Login Successfull " +  resultCallback.PlayFabId);
            playfabId = resultCallback.PlayFabId;
            PopUpNotification("Connected to server successfully!");    
        },
        Error => {
            PopUpNotification("Unable to connect to the server. Make sure your internet is connected and restart the game.");
        });
    }

    private void Start()
    {
        if(PlayerPrefs.GetInt("Mute") == 1)
        {
            soundOff.gameObject.SetActive(true);
            soundOn.gameObject.SetActive(false);
            GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Stop();
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
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Stop();
    }

    public void SoundOffButton_OnClick()
    {
        soundOff.gameObject.SetActive(false);
        soundOn.gameObject.SetActive(true);
        SoundManager.instance.UnMute();  

        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Play();
    }

    public void LeaderboardButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);

        if(!PlayFabClientAPI.IsClientLoggedIn())
        {
            PopUpNotification("Unable to fetch the leaderboard since the client is not connected to the server");
            return;
        }

        ResizeLeaderboardLayout(scroll.GetComponent<ResizeReferences>().originalHeight, 
            scroll.GetComponent<ResizeReferences>().originalAnchorPosY, 
            topPositionsPanel.GetComponent<ResizeReferences>().originalHeight, 
            topPositionsPanel.GetComponent<ResizeReferences>().originalAnchorPosY);

        //Destroy all the leaderboard DYNAMIC Content
        for(int i = 0; i < scroll.content.childCount; i++)
            Destroy(scroll.content.GetChild(i).gameObject);
        
        leaderboardPanel.GetComponent<CanvasGroup>().DOFade(0, 0);
        leaderboardPanel.GetComponent<CanvasGroup>().interactable = false;
        leaderboardPanel.SetActive(true);
        
        OpenLoadingPanel("Fetching leaderboard...");
        GetTopThreeLeaderboard();     
    }

    public void LeaderboardCloseButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick2);
        leaderboardPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(delegate(){
            leaderboardPanel.SetActive(false);
        });        
    }

    private void GetTopThreeLeaderboard()
    {
        var request = new GetLeaderboardRequest()
        {
            StatisticName = "Leaderboard",
            StartPosition = 0,
            MaxResultsCount = 3
        };

        PlayFabClientAPI.GetLeaderboard(request, 
        resultCallback => {
            //Atleast three entries should already be there in leaderboard for this to work properly.
            //Populate First Position Data
            topPositionsPanel.GetChild(1).GetComponent<LeaderboardItem>().initialsText.text = resultCallback.Leaderboard[0].DisplayName[0].ToString();
            topPositionsPanel.GetChild(1).GetComponent<LeaderboardItem>().nameText.text = "1. " + resultCallback.Leaderboard[0].DisplayName.ToString();
            topPositionsPanel.GetChild(1).GetComponent<LeaderboardItem>().scoreText.text = resultCallback.Leaderboard[0].StatValue.ToString();
            //Populate Second Position Data
            topPositionsPanel.GetChild(0).GetComponent<LeaderboardItem>().initialsText.text = resultCallback.Leaderboard[1].DisplayName[0].ToString();
            topPositionsPanel.GetChild(0).GetComponent<LeaderboardItem>().nameText.text = "2. " + resultCallback.Leaderboard[1].DisplayName.ToString();
            topPositionsPanel.GetChild(0).GetComponent<LeaderboardItem>().scoreText.text = resultCallback.Leaderboard[1].StatValue.ToString();
            //Populate Third Position Data
            topPositionsPanel.GetChild(2).GetComponent<LeaderboardItem>().initialsText.text = resultCallback.Leaderboard[2].DisplayName[0].ToString();
            topPositionsPanel.GetChild(2).GetComponent<LeaderboardItem>().nameText.text = "3. " + resultCallback.Leaderboard[2].DisplayName.ToString();
            topPositionsPanel.GetChild(2).GetComponent<LeaderboardItem>().scoreText.text = resultCallback.Leaderboard[2].StatValue.ToString();

            GetPlayerPosition();
        }, 
        Error => {
            leaderboardPanel.SetActive(false);
            CloseLoadingPanel();
            PopUpNotification("Failed to fetch leaderboard. Please try again!");
        });
    }

    private void GetPlayerPosition()
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "Leaderboard",
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, 
        resultCallback => {

            if(resultCallback.Leaderboard[0].StatValue == 0)
                GetLeaderboard(20);
            else
                GetLeaderboardAroundPlayer(21);         
        }, 
        Error => {
            leaderboardPanel.SetActive(false);
            CloseLoadingPanel();
            PopUpNotification("Failed to fetch leaderboard. Please try again!");
        });
    }

    private void GetLeaderboard(int totalEntries)
    {
        var request = new GetLeaderboardRequest()
        {
            StatisticName = "Leaderboard",
            StartPosition = 0,
            MaxResultsCount = totalEntries
        };

        PlayFabClientAPI.GetLeaderboard(request, 
        resultCallback => {
            foreach(var item in resultCallback.Leaderboard)
            {
                GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, Vector3.zero, Quaternion.identity);

                leaderboardItem.GetComponent<LeaderboardItem>().initialsText.text = item.DisplayName[0].ToString();
                leaderboardItem.GetComponent<LeaderboardItem>().nameText.text = item.DisplayName;
                leaderboardItem.GetComponent<LeaderboardItem>().scoreText.text = item.StatValue.ToString();

                leaderboardItem.GetComponent<RectTransform>().SetParent(scroll.content);
                leaderboardItem.transform.localScale = Vector3.one;
            }  
            FadeInLeaderboard();
        }, 
        Error => {
            leaderboardPanel.SetActive(false);
            CloseLoadingPanel();
            PopUpNotification("Failed to fetch leaderboard. Please try again!");
        });
    }

    private void GetLeaderboardAroundPlayer(int totalEntries)
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "Leaderboard",
            MaxResultsCount = totalEntries
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, 
        resultCallback => {
            
            RectTransform player = null;

            foreach(var item in resultCallback.Leaderboard)
            {
                GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, Vector3.zero, Quaternion.identity);

                leaderboardItem.GetComponent<LeaderboardItem>().initialsText.text = item.DisplayName[0].ToString();
                leaderboardItem.GetComponent<LeaderboardItem>().nameText.text = item.DisplayName;
                leaderboardItem.GetComponent<LeaderboardItem>().scoreText.text = item.StatValue.ToString();
                
                leaderboardItem.GetComponent<RectTransform>().SetParent(scroll.content);
                leaderboardItem.transform.localScale = Vector3.one;

                if(item.PlayFabId.Equals(playfabId))
                {
                    player = leaderboardItem.GetComponent<RectTransform>();
                    player.GetComponent<LeaderboardItem>().ChangeColorsToPlayerColors();
                }
            }
            SnapScrollToPlayer(player);
        },  
        Error => {
            leaderboardPanel.SetActive(false);
            CloseLoadingPanel();
            PopUpNotification("Failed to fetch leaderboard. Please try again!");
        });
    }

    public void SnapScrollToPlayer(RectTransform player)
    {
        Canvas.ForceUpdateCanvases();
  
        float contentWidth = scroll.content.rect.width;

        scroll.content.anchorMin = new Vector2(0.5f, 0.5f);
        scroll.content.anchorMax = new Vector2(0.5f, 0.5f);
        scroll.content.sizeDelta = new Vector2(contentWidth, scroll.content.sizeDelta.y);

        scroll.content.anchoredPosition = (Vector2)scroll.transform.InverseTransformPoint(scroll.content.position) - (Vector2)scroll.transform.InverseTransformPoint(player.position);

        if(scroll.verticalNormalizedPosition < 0)
            scroll.verticalNormalizedPosition = 0;

        else if(scroll.verticalNormalizedPosition > 1)
            scroll.verticalNormalizedPosition = 1;

        FadeInLeaderboard();
    }

    private void FadeInLeaderboard()
    {
        leaderboardPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(delegate(){
            CloseLoadingPanel();
            leaderboardPanel.GetComponent<CanvasGroup>().interactable = true;
            scrollVerticalNormalizePosition = scroll.verticalNormalizedPosition;
            canExpand = true;
        });        
    }

    private void Update()
    {
        if(canExpand)
        {
            if(scrollVerticalNormalizePosition + 0.05f < scroll.verticalNormalizedPosition 
                || scrollVerticalNormalizePosition - 0.05f > scroll.verticalNormalizedPosition)
            {
                ResizeLeaderboardLayout(scroll.GetComponent<ResizeReferences>().targetHeight, 
                    scroll.GetComponent<ResizeReferences>().targetAnchorPosY, 
                    topPositionsPanel.GetComponent<ResizeReferences>().targetHeight, 
                    topPositionsPanel.GetComponent<ResizeReferences>().targetAnchorPosY);
            }                       
        }
    }

    public void ResizeLeaderboardLayout(float scrollHeight, float scrollPosY, float topPanelHeight, float topPanelPosY)
    {
        scroll.GetComponent<RectTransform>().DOSizeDelta(new Vector2(scroll.GetComponent<RectTransform>().sizeDelta.x, scrollHeight), 0.2f);
        scroll.GetComponent<RectTransform>().DOAnchorPosY(scrollPosY, 0.2f);

        topPositionsPanel.DOSizeDelta(new Vector2(topPositionsPanel.sizeDelta.x, topPanelHeight), 0.2f);
        topPositionsPanel.DOAnchorPosY(topPanelPosY, 0.2f);

        canExpand = false;         
    }

    private void OpenLoadingPanel(string text)
    {
        loadingPanelText.text = text;
        loadingPanel.GetComponent<CanvasGroup>().DOFade(0, 0).OnComplete(delegate(){       
            loadingPanel.SetActive(true);       
            loadingPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }); 
    }

    private void CloseLoadingPanel()
    {
        loadingPanel.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(delegate(){
            loadingPanel.SetActive(false);       
        });
    }

    private void PopUpNotification(string text)
    {
        SoundManager.instance.playSound(SoundManager.instance.notification);
        notificationText.text = text;
        notificationObject.GetComponent<Animator>().Play("NotificationAnimation", -1, 0);
    }
}
