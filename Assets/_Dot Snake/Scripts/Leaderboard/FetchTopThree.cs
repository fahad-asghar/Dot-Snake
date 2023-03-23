using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class FetchTopThree : MonoBehaviour
{
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Transform _topPositionsPanel;

    private FetchLeaderboard _fetchLeaderboard;

    private void Start() => _fetchLeaderboard = GetComponent<FetchLeaderboard>();

    public void Fetch()
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
            _topPositionsPanel.GetChild(1).GetComponent<LeaderboardItem>().indexText.text = resultCallback.Leaderboard[0].DisplayName[0].ToString();
            _topPositionsPanel.GetChild(1).GetComponent<LeaderboardItem>().nameText1.text = "1. " + resultCallback.Leaderboard[0].DisplayName.ToString();
            _topPositionsPanel.GetChild(1).GetComponent<LeaderboardItem>().scoreText.text = resultCallback.Leaderboard[0].StatValue.ToString();
            
            //Populate Second Position Data
            _topPositionsPanel.GetChild(0).GetComponent<LeaderboardItem>().indexText.text = resultCallback.Leaderboard[1].DisplayName[0].ToString();
            _topPositionsPanel.GetChild(0).GetComponent<LeaderboardItem>().nameText1.text = "2. " + resultCallback.Leaderboard[1].DisplayName.ToString();
            _topPositionsPanel.GetChild(0).GetComponent<LeaderboardItem>().scoreText.text = resultCallback.Leaderboard[1].StatValue.ToString();
            
            //Populate Third Position Data
            _topPositionsPanel.GetChild(2).GetComponent<LeaderboardItem>().indexText.text = resultCallback.Leaderboard[2].DisplayName[0].ToString();
            _topPositionsPanel.GetChild(2).GetComponent<LeaderboardItem>().nameText1.text = "3. " + resultCallback.Leaderboard[2].DisplayName.ToString();
            _topPositionsPanel.GetChild(2).GetComponent<LeaderboardItem>().scoreText.text = resultCallback.Leaderboard[2].StatValue.ToString();

            _fetchLeaderboard.GetPlayerPosition();
        },

        Error => {
            _leaderboardPanel.SetActive(false);

            LoadingPanelController.instance.CloseLoadingPanel();
            PushNotificationController.instance.SendPushNotification("Failed to fetch leaderboard. Please try again!");
        });
    }
}
