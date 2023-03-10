using System.Collections.Generic;
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
    [SerializeField] private int lives;
    [SerializeField] public bool stopMovement;
    [SerializeField] private int playerBestScore;
    [SerializeField] public float score;
    [SerializeField] private SnakeController snakeController;


    [Space][Space]
    [Header("OBJECTS SPAWN VARIABLES")]
    [Space][Space]
    [SerializeField] private Transform grid;
    
    [Space][Space]
    [Header("OBSTACLES")]
    [Space]
    [SerializeField] private List<GameObject> obstaclePrefab;
    [SerializeField] private Transform obstacleParent;
    public List<GameObject> activeObstacle = new List<GameObject>();

    [Space][Space]
    [Header("HEALTH")]
    [Space]
    [SerializeField] private GameObject healthPointPrefab;
    [SerializeField] private Transform healthParent;
    public GameObject activeHealthPoint;

    [Space][Space]
    [Header("FOOD")]
    [Space]
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject specialFoodPrefab;
    [SerializeField] private Transform foodParent;
    public GameObject activeFood;
    public int foodCounter;

    private bool isFetchDataFailed = true;

    private void Awake()
    {
        instance = this;
        DOTween.SetTweensCapacity(1000, 50);
    }

    private void Start()
    {
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
            snakeController.Blink();
    }

    public void TutorialCheckButton_OnClick()
    {
        SoundManager.instance.playSound(SoundManager.instance.buttonClick1);
        tutorialPopUp.GetComponent<Animator>().enabled = false;
        tutorialPopUp.GetComponent<CanvasGroup>().interactable = false;
        tutorialPopUp.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(delegate(){
            tutorialPopUp.SetActive(false);
            PlayerPrefs.SetInt("TutorialShown", 1);
            snakeController.Blink();
        });
    }

    public void DeductLife(int value)
    {
        lives = lives - value;

        switch (lives)
        {
            case 2:
            healthPoint1.transform.DOScale(new Vector2(1.2f, 1.2f), 0.1f).OnComplete(delegate(){
                healthPoint1.transform.DOScale(new Vector2(0.8f, 0.8f), 0.2f).OnComplete(delegate(){
                    healthPoint1.transform.DOScale(new Vector2(1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint1.DOFade(0.3f, 0.3f);
                    });
                });
            });
            snakeController.ChangeColors();  
            break;

            case 1:
            healthPoint2.transform.DOScale(new Vector2(1.2f, 1.2f), 0.1f).OnComplete(delegate(){
                healthPoint2.transform.DOScale(new Vector2(0.8f, 0.8f), 0.2f).OnComplete(delegate(){
                    healthPoint2.transform.DOScale(new Vector2(1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint2.DOFade(0.3f, 0.3f);
                    });
                });
            });
            snakeController.ChangeColors();
            break;

            case 0:
            healthPoint3.transform.DOScale(new Vector2(1.2f, 1.2f), 0.1f).OnComplete(delegate(){
                healthPoint3.transform.DOScale(new Vector2(0.8f, 0.8f), 0.2f).OnComplete(delegate(){
                    healthPoint3.transform.DOScale(new Vector2(1f, 1f), 0.1f).OnComplete(delegate(){
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
            healthPoint3.transform.DOScale(new Vector2(0.8f, 0.8f), 0.1f).OnComplete(delegate(){
                healthPoint3.transform.DOScale(new Vector2(1.2f, 1.2f), 0.2f).OnComplete(delegate(){
                    healthPoint3.transform.DOScale(new Vector2(1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint3.DOFade(1, 0.3f);
                    });
                });
            });
            break; 
            
            case 2:
            healthPoint2.transform.DOScale(new Vector2(0.8f, 0.8f), 0.1f).OnComplete(delegate(){
                healthPoint2.transform.DOScale(new Vector2(1.2f, 1.2f), 0.2f).OnComplete(delegate(){
                    healthPoint2.transform.DOScale(new Vector2(1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint2.DOFade(1, 0.3f);
                    });
                });
            });
            break;

            case 3:
            healthPoint1.transform.DOScale(new Vector2(0.8f, 0.8f), 0.1f).OnComplete(delegate(){
                healthPoint1.transform.DOScale(new Vector2(1.2f, 1.2f), 0.2f).OnComplete(delegate(){
                    healthPoint1.transform.DOScale(new Vector2(1f, 1f), 0.1f).OnComplete(delegate(){
                        healthPoint1.DOFade(1, 0.3f);
                    });
                });
            });
            break;       
        }
    }

    private void RewardedAdCheck()
    {
        if(rewardedAdsPerSession > 0 && rewardedAdManager.rewardedAd != null)
        {
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
            rewardedAdPopUp.SetActive(false);
            AddLife(1);    
            snakeController.ChangeColors();   
        });
    }

    private void GameOver()
    {
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
        LeaderboardGameplay.instance.UpdatePlayerName(nameText.text);
    }

    private void SendScoreToLeaderboard()
    {
        OpenLoadingPopUp("Updating score...");
        LeaderboardGameplay.instance.SendScoreToLeaderboard((int)score);
    }

    public void UpdateScore(int value, int factor)
    {
        DOTween.To(() => score, SetScore, score + (value * factor), 0.3f).SetEase(Ease.Linear);     
    }

    private void SetScore(float value)
    {
        score = value;
        scoreText.text = (int)score + "";
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

    #region OBJECTS SPAWNER

    public void CheckObstacleOdds()
    {
        int odds = Random.Range(1, 101);

        if(odds > 50)
        {
            int random = Random.Range(1, 4);

            for(int i = 0; i < random; i++)
                GameController.instance.SpawnObstacle();
        }         
    }

    private void SpawnObstacle()
    {
        GameObject obstacle = Instantiate(obstaclePrefab[Random.Range(0, obstaclePrefab.Count)]);
        obstacle.GetComponent<RectTransform>().SetParent(obstacleParent);
        obstacle.transform.localScale = Vector3.one;
        SetObstaclePosition(obstacle);
    }

    private void SetObstaclePosition(GameObject obstacle)
    {
        if(obstacle.name.Equals("Obstacle-Horizontal-3(Clone)"))
        {
            int positionIndex1 = Random.Range(0, grid.childCount);
            int positionIndex2 = positionIndex1 + 1;
            int positionIndex3 = positionIndex2 + 1;

            if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1)
            {
                SetObstaclePosition(obstacle);
                return;
            }

            string name1 = grid.GetChild(positionIndex1).name;
            string [] row1 = name1.Split("_");

            string name2 = grid.GetChild(positionIndex2).name;
            string [] row2 = name2.Split("_");

            string name3 = grid.GetChild(positionIndex3).name;
            string [] row3 = name3.Split("_");

            if(row2[0] != row1[0] || row3[0] != row1[0])
            {
                SetObstaclePosition(obstacle);
                return;
            }

            bool overlap1 = grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap2 = grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap3 = grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap();

            if(!overlap1 && !overlap2 && !overlap3)
            {
                obstacle.GetComponent<RectTransform>().position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;
                activeObstacle.Add(obstacle);
            }
            else
                SetObstaclePosition(obstacle);
        }

        else if(obstacle.name.Equals("Obstacle-Horizontal-4(Clone)"))
        {
            int positionIndex1 = Random.Range(0, grid.childCount);
            int positionIndex2 = positionIndex1 + 1;
            int positionIndex3 = positionIndex2 + 1;
            int positionIndex4 = positionIndex3 + 1;

            if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1 || positionIndex4 > grid.childCount - 1)
            {
                SetObstaclePosition(obstacle);
                return;
            }

            string name1 = grid.GetChild(positionIndex1).name;
            string [] row1 = name1.Split("_");

            string name2 = grid.GetChild(positionIndex2).name;
            string [] row2 = name2.Split("_");

            string name3 = grid.GetChild(positionIndex3).name;
            string [] row3 = name3.Split("_");

            string name4 = grid.GetChild(positionIndex4).name;
            string [] row4 = name4.Split("_");

            if(row2[0] != row1[0] || row3[0] != row1[0] || row4[0] != row1[0])
            {
                SetObstaclePosition(obstacle);
                return;
            }

            bool overlap1 = grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap2 = grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap3 = grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap4 = grid.GetChild(positionIndex4).GetComponent<ObjectOverlapStatus>().CheckOverlap();

            if(!overlap1 && !overlap2 && !overlap3 && !overlap4)
            {
                obstacle.GetComponent<RectTransform>().position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;
                activeObstacle.Add(obstacle);
            }
            else
                SetObstaclePosition(obstacle);
        }

        else if(obstacle.name.Equals("Obstacle-Vertical-3(Clone)"))
        {
            int positionIndex1 = Random.Range(0, grid.childCount);
            int positionIndex2 = positionIndex1 + 23;
            int positionIndex3 = positionIndex2 + 23;

            if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1)
            {
                SetObstaclePosition(obstacle);
                return;
            }

            string name1 = grid.GetChild(positionIndex1).name;
            string [] coloumn1 = name1.Split("_");

            string name2 = grid.GetChild(positionIndex2).name;
            string [] coloumn2 = name2.Split("_");

            string name3 = grid.GetChild(positionIndex3).name;
            string [] coloumn3 = name3.Split("_");

            if(coloumn2[1] != coloumn1[1] || coloumn3[1] != coloumn1[1])
            {
                SetObstaclePosition(obstacle);
                return;
            }

            bool overlap1 = grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap2 = grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap3 = grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap();

            if(!overlap1 && !overlap2 && !overlap3)
            {
                obstacle.GetComponent<RectTransform>().position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;
                activeObstacle.Add(obstacle);
            }
            else
                SetObstaclePosition(obstacle);
        }

        else if(obstacle.name.Equals("Obstacle-Vertical-4(Clone)"))
        {
            int positionIndex1 = Random.Range(0, grid.childCount);
            int positionIndex2 = positionIndex1 + 23;
            int positionIndex3 = positionIndex2 + 23;
            int positionIndex4 = positionIndex3 + 23;

            if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1 || positionIndex4 > grid.childCount - 1)
            {
                SetObstaclePosition(obstacle);
                return;
            }

            string name1 = grid.GetChild(positionIndex1).name;
            string [] coloumn1 = name1.Split("_");

            string name2 = grid.GetChild(positionIndex2).name;
            string [] coloumn2 = name2.Split("_");

            string name3 = grid.GetChild(positionIndex3).name;
            string [] coloumn3 = name3.Split("_");

            string name4 = grid.GetChild(positionIndex4).name;
            string [] coloumn4 = name4.Split("_");

            if(coloumn2[1] != coloumn1[1] || coloumn3[1] != coloumn1[1] || coloumn4[1] != coloumn1[1])
            {
                SetObstaclePosition(obstacle);
                return;
            }

            bool overlap1 = grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap2 = grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap3 = grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap();
            bool overlap4 = grid.GetChild(positionIndex4).GetComponent<ObjectOverlapStatus>().CheckOverlap();

            if(!overlap1 && !overlap2 && !overlap3 && !overlap4)
            {
                obstacle.GetComponent<RectTransform>().position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;
                activeObstacle.Add(obstacle);
            }
            else
                SetObstaclePosition(obstacle);
        }
    }

    public void ClearObstacles()
    {
        for(int i = 0; i < activeObstacle.Count; i++)
            activeObstacle[i].GetComponent<ObjectProperties>().Hide();

        activeObstacle.Clear();
    }

    public void CheckHealthPointOdds()
    {
        if(lives >= 3 || activeHealthPoint != null)
            return;
        
        int odds = Random.Range(1, 101);

        if(odds > 50)
            SpawnHealthPoint();
    }

    public void SpawnHealthPoint()
    {
        GameObject health = Instantiate(healthPointPrefab);
        health.GetComponent<RectTransform>().SetParent(healthParent);
        health.transform.localScale = Vector3.one;
        SetHealthPointPosition(health);    
    }

    private void SetHealthPointPosition(GameObject health)
    {
        int positionIndex = Random.Range(0, grid.childCount);
        bool overlap = grid.GetChild(positionIndex).GetComponent<ObjectOverlapStatus>().CheckOverlap();

        if(!overlap)
        {
            health.GetComponent<RectTransform>().position = grid.GetChild(positionIndex).GetComponent<RectTransform>().position;
            activeHealthPoint = health;
        }
        else
            SetHealthPointPosition(health);
    }

    public void SpawnFood()
    {
        GameObject food = null;

        if(foodCounter > 9)
        {
            food = Instantiate(specialFoodPrefab);
            foodCounter = 0;
        }
        else
            food = Instantiate(foodPrefab);

        food.GetComponent<RectTransform>().SetParent(foodParent);
        food.transform.localScale = Vector3.one;
        SetFoodPosition(food);    

        foodCounter++;
    }

    private void SetFoodPosition(GameObject food)
    {
        int positionIndex = Random.Range(0, grid.childCount);
        bool overlap = grid.GetChild(positionIndex).GetComponent<ObjectOverlapStatus>().CheckOverlap();

        if(!overlap)
        {
            food.GetComponent<RectTransform>().position = grid.GetChild(positionIndex).GetComponent<RectTransform>().position;
            activeFood = food;
        }
        else
            SetFoodPosition(food);
    }    

    #endregion
}
