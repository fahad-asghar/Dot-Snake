using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class FetchLeaderboard : MonoBehaviour
{
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private GameObject _leaderboardItemPrefab;
    [SerializeField] private ScrollRect _leaderboardScroll;
    [SerializeField] private RectTransform _cloneLeaderboardItem;

    private LeaderboardAnimation _leaderboardAnimation;

    private void Start() => _leaderboardAnimation = GetComponent<LeaderboardAnimation>();

    public void GetPlayerPosition()
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

            _leaderboardPanel.SetActive(false);

            LoadingPanelController.instance.CloseLoadingPanel();
            PushNotificationController.instance.SendPushNotification("Failed to fetch leaderboard. Please try again!");
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
                GameObject leaderboardItem = Instantiate(_leaderboardItemPrefab, Vector3.zero, Quaternion.identity);

                leaderboardItem.GetComponent<LeaderboardItem>().indexText.text = item.Position + 1 + "";
                leaderboardItem.GetComponent<LeaderboardItem>().nameText1.text = item.DisplayName;
                leaderboardItem.GetComponent<LeaderboardItem>().nameText2.text = item.DisplayName;
                leaderboardItem.GetComponent<LeaderboardItem>().scoreText.text = item.StatValue.ToString();

                leaderboardItem.GetComponent<RectTransform>().SetParent(_leaderboardScroll.content);
                leaderboardItem.transform.localScale = Vector3.one;
            }

            _leaderboardAnimation.FadeInLeaderboard();
        }, 

        Error => {

            _leaderboardPanel.SetActive(false);
            
            LoadingPanelController.instance.CloseLoadingPanel();
            PushNotificationController.instance.SendPushNotification("Failed to fetch leaderboard. Please try again!");
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
                GameObject leaderboardItem = Instantiate(_leaderboardItemPrefab, Vector3.zero, Quaternion.identity);

                leaderboardItem.GetComponent<LeaderboardItem>().indexText.text = item.Position + 1 + "";
                leaderboardItem.GetComponent<LeaderboardItem>().nameText1.text = item.DisplayName;
                leaderboardItem.GetComponent<LeaderboardItem>().nameText2.text = item.DisplayName;
                leaderboardItem.GetComponent<LeaderboardItem>().scoreText.text = item.StatValue.ToString();
                
                leaderboardItem.GetComponent<RectTransform>().SetParent(_leaderboardScroll.content);
                leaderboardItem.transform.localScale = Vector3.one;

                if(item.PlayFabId.Equals(MainMenuController.playfabId))
                {
                    player = leaderboardItem.GetComponent<RectTransform>();

                    _cloneLeaderboardItem.GetComponent<LeaderboardItem>().indexText.text = item.Position + 1 + "";
                    _cloneLeaderboardItem.GetComponent<LeaderboardItem>().nameText1.text = item.DisplayName;
                    _cloneLeaderboardItem.GetComponent<LeaderboardItem>().nameText2.text = item.DisplayName;
                    _cloneLeaderboardItem.GetComponent<LeaderboardItem>().scoreText.text = item.StatValue.ToString();
                }
            }

            _leaderboardAnimation.SnapScrollToPlayer(player);
        },

        Error => {

            _leaderboardPanel.SetActive(false);
            
            LoadingPanelController.instance.CloseLoadingPanel();
            PushNotificationController.instance.SendPushNotification("Failed to fetch leaderboard. Please try again!");
        });
    }
}
