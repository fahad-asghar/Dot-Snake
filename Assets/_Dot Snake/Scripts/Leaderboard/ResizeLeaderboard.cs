using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResizeLeaderboard : MonoBehaviour
{
    [SerializeField] private ScrollRect _leaderboardScroll;
    [SerializeField] private RectTransform _topPositionsPanel;

    public void ResizeLeaderboardLayout(float scrollHeight, float scrollPosY, float topPanelHeight, float topPanelPosY)
    {
        _leaderboardScroll.GetComponent<RectTransform>().DOSizeDelta(new Vector2(_leaderboardScroll.GetComponent<RectTransform>().sizeDelta.x, scrollHeight), 0.2f);
        _leaderboardScroll.GetComponent<RectTransform>().DOAnchorPosY(scrollPosY, 0.2f);

        _topPositionsPanel.DOSizeDelta(new Vector2(_topPositionsPanel.sizeDelta.x, topPanelHeight), 0.2f);
        _topPositionsPanel.DOAnchorPosY(topPanelPosY, 0.2f);       
    }
}
