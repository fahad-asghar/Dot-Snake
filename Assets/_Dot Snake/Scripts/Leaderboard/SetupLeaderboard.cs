using UnityEngine;
using UnityEngine.UI;
using PlayFab;

public class SetupLeaderboard : MonoBehaviour
{
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private ScrollRect _leaderboardScroll;
    [SerializeField] private RectTransform _topPositionsPanel;

    private ResizeLeaderboard _resizeLeaderboard;
    private FetchTopThree _fetchTopThree;
    private LeaderboardAnimation _leaderboardAnimation;

    private void Start()
    {
        _resizeLeaderboard = GetComponent<ResizeLeaderboard>();
        _fetchTopThree = GetComponent<FetchTopThree>();
        _leaderboardAnimation = GetComponent<LeaderboardAnimation>();
    }

    public void Setup()
    {
        if(!PlayFabClientAPI.IsClientLoggedIn())
        {
            PushNotificationController.instance.SendPushNotification("Unable to fetch the leaderboard since the client is not connected to the server");
            return;
        }

        LoadingPanelController.instance.OpenLoadingPanel("Fetching leaderboard..");

        _resizeLeaderboard.ResizeLeaderboardLayout(
            _leaderboardScroll.GetComponent<ResizeReferences>().originalHeight,
            _leaderboardScroll.GetComponent<ResizeReferences>().originalAnchorPosY,
            _topPositionsPanel.GetComponent<ResizeReferences>().originalHeight,
             _topPositionsPanel.GetComponent<ResizeReferences>().originalAnchorPosY
        );
        
        _leaderboardAnimation.FadeOutLeaderboard();
        _fetchTopThree.Fetch();     
    }

    public void ClearLeaderboard()
    {
        for(int i = 0; i < _leaderboardScroll.content.childCount; i++)
        {
            _leaderboardScroll.content.GetChild(i).GetComponent<LeaderboardItem>().StopTextAnimation();
            Destroy(_leaderboardScroll.content.GetChild(i).gameObject);
        }
    }
}
