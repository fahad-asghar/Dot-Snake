using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.UI;
using DG.Tweening;

public class LeaderboardGameplay : MonoBehaviour
{
    public static LeaderboardGameplay instance;

    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform topPositionsPanel;
    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private RectTransform cloneScoreEntry;

    private RectTransform playerEntry;

    private float verticalNormalizedPosition;
    private bool canExpand = false;

    private void Awake()
    {
        instance = this;
    }

    public void UpdatePlayerName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, 
        resultCallback => {
            Debug.Log("Name updatad to - " + resultCallback.DisplayName);
            SendScoreToLeaderboard((int)GameController.instance.score);
        }, 
        Error => {
            GameController.instance.CloseLoadingPopUp();
            GameController.instance.PopUpNotification("Failed to update the name due to unknown reason, please try again");
        });
    }

    public void SendScoreToLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest(){
            Statistics = new List<StatisticUpdate>{ new StatisticUpdate(){
                    StatisticName = "Leaderboard",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, 
        resultCallback => {
            Debug.Log("Score sent successfully");
            Invoke("GetTopThreeLeaderboard", 1);
        }, 
        Error => {
            GameController.instance.CloseLoadingPopUp();
            GameController.instance.PopUpNotification("Unable to update score due to poor network conditions, please try again");
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
        
            GetLeaderboardAroundPlayer(21);
        }, 
        Error => {
            GameController.instance.CloseLoadingPopUp();
            GameController.instance.PopUpNotification("Unable to fetch leaderboard due to poor network conditions, please try again");
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

            foreach(var item in resultCallback.Leaderboard)
            {
                GameObject leaderboardItem = Instantiate(scoreEntryPrefab, Vector3.zero, Quaternion.identity);

                leaderboardItem.GetComponent<LeaderboardItem>().initialsText.text = item.DisplayName[0].ToString();
                leaderboardItem.GetComponent<LeaderboardItem>().nameText.text = item.DisplayName;
                leaderboardItem.GetComponent<LeaderboardItem>().scoreText.text = item.StatValue.ToString();

                leaderboardItem.GetComponent<RectTransform>().SetParent(scroll.content);
                leaderboardItem.transform.localScale = Vector3.one;

                if(item.PlayFabId.Equals(MainMenuController.playfabId))
                {
                    playerEntry = leaderboardItem.GetComponent<RectTransform>();

                    cloneScoreEntry.GetComponent<LeaderboardItem>().initialsText.text = item.DisplayName[0].ToString();
                    cloneScoreEntry.GetComponent<LeaderboardItem>().nameText.text = item.DisplayName;
                    cloneScoreEntry.GetComponent<LeaderboardItem>().scoreText.text = item.StatValue.ToString();
                }
            }

            leaderboardPanel.GetComponent<CanvasGroup>().DOFade(0, 0).OnComplete(delegate(){
                leaderboardPanel.GetComponent<CanvasGroup>().interactable = false;
                leaderboardPanel.SetActive(true);
                SnapScrollToPlayer();
            });     
        },    
        Error => {
            GameController.instance.CloseLoadingPopUp();
            GameController.instance.PopUpNotification("Unable to fetch leaderboard due to poor network conditions, please try again");
        });
    }

    private void SnapScrollToPlayer()
    {
        Canvas.ForceUpdateCanvases();
  
        float contentWidth = scroll.content.rect.width;

        scroll.content.anchorMin = new Vector2(0.5f, 0.5f);
        scroll.content.anchorMax = new Vector2(0.5f, 0.5f);
        scroll.content.sizeDelta = new Vector2(contentWidth, scroll.content.sizeDelta.y);

        scroll.content.anchoredPosition = (Vector2)scroll.transform.InverseTransformPoint(scroll.content.position) - (Vector2)scroll.transform.InverseTransformPoint(playerEntry.position);

        if(scroll.verticalNormalizedPosition < 0)
            scroll.verticalNormalizedPosition = 0;

        else if(scroll.verticalNormalizedPosition > 1)
            scroll.verticalNormalizedPosition = 1;

        verticalNormalizedPosition = scroll.verticalNormalizedPosition;

        BuildUpLeaderboardAnimation();       
    }

    private void BuildUpLeaderboardAnimation()
    {
        cloneScoreEntry.anchoredPosition = scroll.content.GetChild(2).GetComponent<RectTransform>().anchoredPosition;
        cloneScoreEntry.DOMoveX(scroll.content.GetChild(2).GetComponent<RectTransform>().position.x, 0);
        cloneScoreEntry.sizeDelta = scroll.content.GetChild(2).GetComponent<RectTransform>().sizeDelta;
        
        Canvas.ForceUpdateCanvases();

        scroll.verticalNormalizedPosition = 0;
        scroll.enabled = false;

        leaderboardPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(delegate(){

            cloneScoreEntry.DOAnchorPosY(cloneScoreEntry.anchoredPosition.y - 50, 0.2f).OnComplete(delegate(){
                cloneScoreEntry.DOAnchorPosY(scroll.content.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y, 0.8f);
            });
      
            scroll.DOVerticalNormalizedPos(-0.02f, 0.2f).OnComplete(delegate(){
                scroll.DOVerticalNormalizedPos(verticalNormalizedPosition, 1f).OnComplete(delegate(){
                    cloneScoreEntry.DOMoveY(playerEntry.position.y, 0.3f).OnComplete(delegate(){
                              
                        playerEntry.GetComponent<LeaderboardItem>().ChangeColorsToPlayerColors();

                        cloneScoreEntry.GetComponent<CanvasGroup>().DOFade(0, 0f);
                        cloneScoreEntry.gameObject.SetActive(false);

                        leaderboardPanel.GetComponent<CanvasGroup>().interactable = true;
                        scroll.enabled = true;
                        canExpand = true;
                    });
                });
            }); 
        });
    }

    private void Update()
    {
        if(canExpand)
        {
            if(verticalNormalizedPosition + 0.05f < scroll.verticalNormalizedPosition || verticalNormalizedPosition - 0.05f > scroll.verticalNormalizedPosition) 
                ResizeLeaderboardLayout(scroll.GetComponent<ResizeReferences>().targetHeight, 
                    scroll.GetComponent<ResizeReferences>().targetAnchorPosY, 
                    topPositionsPanel.GetComponent<ResizeReferences>().targetHeight, 
                    topPositionsPanel.GetComponent<ResizeReferences>().targetAnchorPosY);
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
}
