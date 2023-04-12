using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [Space][Space][Header("ADs")]
    [SerializeField] private RewardedAdManager rewardedAdManager;
    private int rewardedAdsPerSession = 1;

    [Space][Space][Header("UI")]
    [SerializeField] private TextMeshProUGUI playerBestScoreText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image scoreIcon;
    [SerializeField] private Image healthPoint1;
    [SerializeField] private Image healthPoint2;
    [SerializeField] private Image healthPoint3;
    [SerializeField] private Transform highscoreGrid;
    [SerializeField] private Transform gameOverGrid;
    [SerializeField] private GameObject gameOverPopUp;
    [SerializeField] private Image gameOverPopUpCircle;
    [SerializeField] private GameObject gameOverCheckButton;
    [SerializeField] private GameObject gameOverReloadButton;
    [SerializeField] private GameObject gameOverCloseButton;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighscoreText;
    [SerializeField] private GameObject tutorialPopUp;
    [SerializeField] private Transform swipeAnimationObject;
    [SerializeField] private GameObject rewardedAdPopUp;
    [SerializeField] private GameObject namePopUp;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject loadingPopUp;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject notificationObject;
    [SerializeField] private TextMeshProUGUI notificationText;

    [SerializeField] private Image fader;

    [Space][Space][Header("GENERAL")]
    [SerializeField] public int lives;
    [SerializeField] public bool stopMovement;
    [SerializeField] private int playerBestScore = 0;
    [SerializeField] public float score;
    [SerializeField] private GameObject snake;
    [SerializeField] private GameObject _leaderboardController;

    private bool isFetchDataFailed = true;

    private void Awake()
    {
        instance = this;
        DOTween.SetTweensCapacity(2500, 50);
    }

    private void Start()
    { 
        FoodSpawn.instance.SpawnFood();
               
        fader.DOFade(1, 0).OnComplete(delegate(){
            fader.gameObject.SetActive(true);
            fader.DOFade(0, 1).OnComplete(delegate(){
                fader.gameObject.SetActive(false);
                TutorialCheck();
            });           
        });

        if(!PlayFabClientAPI.IsClientLoggedIn())
        {
            PopUpNotification("Unable to fetch data since the client is not connected to the server - score will not be recorded");
            return;
        }

        GetPlayerScoreFromServer();
    }

    private void GetPlayerScoreFromServer()
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "Leaderboard",
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, 
        resultCallback => {
            isFetchDataFailed = false;
            playerBestScore = resultCallback.Leaderboard[0].StatValue;
            playerBestScoreText.text = playerBestScore.ToString();                
        }, 
        
        errorCallback => {
            PopUpNotification("Unable to fetch data due to poor network conditions - Score will not be recorded");
        });
    }

    private void TutorialCheck()
    {
        if(PlayerPrefs.GetInt("TutorialShown") == 0)
            tutorialPopUp.SetActive(true);

        else
        {
            PauseController.instance.canPause = true;
            snake.GetComponent<SnakeBlink>().Blink();
        }
    }

    public void TutorialCheckButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
        tutorialPopUp.GetComponent<Animator>().enabled = false;
        tutorialPopUp.GetComponent<CanvasGroup>().interactable = false;
        tutorialPopUp.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(delegate(){
            
            tutorialPopUp.SetActive(false);
            PlayerPrefs.SetInt("TutorialShown", 1);
            
            PauseController.instance.canPause = true;
            snake.GetComponent<SnakeBlink>().Blink();
        });
    }

    public void DeductLife(int value)
    {
        lives = lives - value;

        switch (lives)
        {
            case 2:
            healthPoint1.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(delegate(){
                healthPoint1.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).OnComplete(delegate(){
                    healthPoint1.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint1.DOFade(0.3f, 0.3f);
                    });
                });
            });
            snake.GetComponent<SnakeImpactColorChange>().OnImpact_ColorEffect();
            break;

            case 1:
            healthPoint2.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(delegate(){
                healthPoint2.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).OnComplete(delegate(){
                    healthPoint2.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint2.DOFade(0.3f, 0.3f);
                    });
                });
            });
            snake.GetComponent<SnakeImpactColorChange>().OnImpact_ColorEffect();
            break;

            case 0:
            healthPoint3.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(delegate(){
                healthPoint3.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).OnComplete(delegate(){
                    healthPoint3.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint3.DOFade(0.3f, 0.3f);
                    });
                });
            });
            RewardedAdCheck();
            break;
        }
    }

    public void AddLife(int value)
    {
        lives = lives + 1;

        switch (lives)
        {
            case 1:
            healthPoint3.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).OnComplete(delegate(){
                healthPoint3.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).OnComplete(delegate(){
                    healthPoint3.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint3.DOFade(1, 0.3f);
                    });
                });
            });
            break; 
            
            case 2:
            healthPoint2.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).OnComplete(delegate(){
                healthPoint2.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).OnComplete(delegate(){
                    healthPoint2.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint2.DOFade(1, 0.3f);
                    });
                });
            });
            break;

            case 3:
            healthPoint1.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).OnComplete(delegate(){
                healthPoint1.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).OnComplete(delegate(){
                    healthPoint1.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint1.DOFade(1, 0.3f);
                    });
                });
            });
            break;       
        }
    }

    private void RewardedAdCheck()
    {
        if(rewardedAdsPerSession > 0 
        && rewardedAdManager.rewardedAd != null 
        && rewardedAdManager.rewardedAd.CanShowAd())
        {
            PauseController.instance.canPause = false;

            rewardedAdsPerSession--;          
            Invoke("OpenRewardedAdPopUp", 1);
        }
        
        else
            GameOver();
    }

    private void OpenRewardedAdPopUp()
    {
        rewardedAdPopUp.SetActive(true);
    }

    public void RewardedAdPopUpCheckButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
        rewardedAdManager.ShowRewardedAd();
    }

    public void RewardedAdPopUpCloseButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick2);

        rewardedAdPopUp.GetComponent<Animator>().enabled = false;
        rewardedAdPopUp.GetComponent<CanvasGroup>().DOFade(0, 1).OnComplete(delegate(){
            rewardedAdPopUp.SetActive(false);
            GameOver();
        });
    }

    public void RewardUser(int value)
    {
        rewardedAdPopUp.GetComponent<Animator>().enabled = false;
        rewardedAdPopUp.GetComponent<CanvasGroup>().DOFade(0, 1).OnComplete(delegate(){
            PauseController.instance.canPause = true;
            rewardedAdPopUp.SetActive(false);
            snake.GetComponent<SnakeImpactColorChange>().OnImpact_ColorEffect();
            AddLife(1);       
        });
    }

    private void GameOver()
    {
        PauseController.instance.canPause = false;

        if(isFetchDataFailed)
        {
            SoundManager.instance.playSound(SoundManager.instance.lose[UnityEngine.Random.Range(0, SoundManager.instance.lose.Count)]);

            gameOverGrid.gameObject.SetActive(true);
            
            for(int i = 0; i < gameOverGrid.childCount; i++)
                gameOverGrid.GetChild(i).GetComponent<Image>().DOFade(1, 0.3f).SetDelay(Random.Range(0f, 2.0f)).SetEase(Ease.Linear);

            gameOverScoreText.text = (int)score + "";
            gameOverHighscoreText.text = "";

            gameOverReloadButton.SetActive(true);
            gameOverCloseButton.SetActive(true);
            gameOverPopUpCircle.color = new Color32(212, 98, 240, 255);
            
            Invoke("OpenGameOverPopUp", 3);
            return;
        }

        if(score <= playerBestScore)
        {
            SoundManager.instance.playSound(SoundManager.instance.lose[UnityEngine.Random.Range(0, SoundManager.instance.lose.Count)]);

            gameOverGrid.gameObject.SetActive(true);
            
            for(int i = 0; i < gameOverGrid.childCount; i++)
                gameOverGrid.GetChild(i).GetComponent<Image>().DOFade(1, 0.3f).SetDelay(Random.Range(0f, 2.0f)).SetEase(Ease.Linear);

            gameOverScoreText.text = (int)score + "";
            gameOverHighscoreText.text = "Best: " + playerBestScore;

            gameOverReloadButton.SetActive(true);
            gameOverCloseButton.SetActive(true);
            gameOverPopUpCircle.color = new Color32(212, 98, 240, 255);
            
            Invoke("OpenGameOverPopUp", 3);
        }

        else
        {
            SoundManager.instance.playSound(SoundManager.instance.win[UnityEngine.Random.Range(0, SoundManager.instance.win.Count)]);

            highscoreGrid.gameObject.SetActive(true);

            for(int i = 0; i < highscoreGrid.childCount; i++)
                highscoreGrid.GetChild(i).GetComponent<Image>().DOColor(new Color(scoreIcon.color.r, scoreIcon.color.g, scoreIcon.color.b, 0), 0f);
            
            for(int i = 0; i < highscoreGrid.childCount; i++)
                highscoreGrid.GetChild(i).GetComponent<Image>().DOFade(1, 0.3f).SetDelay(Random.Range(0f, 2.0f)).SetEase(Ease.Linear);

            gameOverScoreText.text = (int)score + "";
            gameOverHighscoreText.text = "New record!";

            gameOverCheckButton.SetActive(true);
            gameOverPopUpCircle.color = new Color32(248, 202, 10, 255);

            Invoke("OpenGameOverPopUp", 3);
        }
    }

    private void OpenGameOverPopUp()
    {
        gameOverPopUp.SetActive(true);   
    }

    public void GameOverPopUpCheckButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);

        if(playerBestScore == 0)
        {         
            OpenNamePopUp();
            return;
        }
        
        if (score > playerBestScore)
            SendScoreToLeaderboard();
    }

    private void OpenNamePopUp()
    {
        namePopUp.SetActive(true);
    }

    public void NamePopUpCheckButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);

        OpenLoadingPopUp("Saving name...");
        _leaderboardController.GetComponent<UpdatePlayerName>().UpdateName(nameText.text);
    }

    private void SendScoreToLeaderboard()
    {
        OpenLoadingPopUp("Updating score...");
        _leaderboardController.GetComponent<UpdatePlayerScore>().SendScoreToLeaderboard((int)score);
    }

    public void UpdateScore(int value, int factor)
    {
        DOTween.To(() => score, SetScore, score + (value * factor), 0.3f).SetEase(Ease.Linear);     
    }

    private void SetScore(float value)
    {
        score = value;
        scoreText.text = (int)score + "";

        if(score > playerBestScore)
            playerBestScoreText.text = (int)score + "";
    }

    public void SetScoreIconColor(Color color)
    {
        scoreIcon.DOColor(color, 0.5f);
    }

    private void OpenLoadingPopUp(string text)
    {
        loadingText.text = text;
        loadingPopUp.GetComponent<CanvasGroup>().DOFade(0, 0).OnComplete(delegate(){
            loadingPopUp.SetActive(true);
            loadingPopUp.GetComponent<CanvasGroup>().DOFade(1, 0.3f).OnComplete(delegate(){
                
            });
        });
    }

    public void CloseLoadingPopUp()
    {
        loadingPopUp.GetComponent<CanvasGroup>().DOFade(0, 0.2f).OnComplete(delegate(){
            loadingPopUp.SetActive(false);
        });
    }

    public void PopUpNotification(string text)
    {
        SoundManager.instance.playSound(SoundManager.instance.notification);
        notificationText.text = text;
        notificationObject.GetComponent<Animator>().Play("NotificationAnimation", -1, 0);
    }

    public void CloseButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick2);

        fader.DOFade(0, 0).OnComplete(delegate(){
            fader.gameObject.SetActive(true);
            fader.DOFade(1, 0.5f).OnComplete(delegate(){
                DOTween.KillAll();
                SceneManager.LoadScene("Main Menu");
            });
        });
    }

    public void ReloadButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
        fader.DOFade(0, 0).OnComplete(delegate(){
            fader.gameObject.SetActive(true);
            fader.DOFade(1, 0.5f).OnComplete(delegate(){
                DOTween.KillAll();
                SceneManager.LoadScene("Gameplay");
            });
        });
    }
}
