using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LeaderboardAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private ScrollRect _leaderboardScroll;
    [SerializeField] private GameObject _topPositionsPanel;
    [SerializeField] private RectTransform _cloneLeaderboardItem;
    [SerializeField] private bool _triggerAutoScrollAnimation;

    private ResizeLeaderboard _resizeLeaderboard;
    private PlayerLeaderboardEngagement _playerLeaderboardEngagement;

    private float _scrollVerticalNormalizePosition;
    private bool _canExpand = false;

    private void Start()
    {
        _resizeLeaderboard = GetComponent<ResizeLeaderboard>();
        _playerLeaderboardEngagement = GetComponent<PlayerLeaderboardEngagement>();
    } 

    public void SnapScrollToPlayer(RectTransform player)
    {
        Canvas.ForceUpdateCanvases();
  
        float contentWidth = _leaderboardScroll.content.rect.width;

        _leaderboardScroll.content.anchorMin = new Vector2(0.5f, 0.5f);
        _leaderboardScroll.content.anchorMax = new Vector2(0.5f, 0.5f);
        _leaderboardScroll.content.sizeDelta = new Vector2(contentWidth, _leaderboardScroll.content.sizeDelta.y);

        _leaderboardScroll.content.anchoredPosition = (Vector2)_leaderboardScroll.transform.InverseTransformPoint(_leaderboardScroll.content.position) - (Vector2)_leaderboardScroll.transform.InverseTransformPoint(player.position);

        if(_leaderboardScroll.verticalNormalizedPosition < 0)
            _leaderboardScroll.verticalNormalizedPosition = 0;

        else if(_leaderboardScroll.verticalNormalizedPosition > 1)
            _leaderboardScroll.verticalNormalizedPosition = 1;

        if(!_triggerAutoScrollAnimation) 
        {  
            _playerLeaderboardEngagement.StartCoroutine(_playerLeaderboardEngagement.GetTotalPlayers(player));  
            player.GetComponent<LeaderboardItem>().ChangeColorsToPlayerColors();
            player.GetComponent<LeaderboardItem>().AnimateNameText();

            FadeInLeaderboard();
        }

        else
            AutoScrollAnimation(player);   
    }
    
    public void FadeOutLeaderboard()
    {
        _leaderboardPanel.GetComponent<CanvasGroup>().DOFade(0, 0);
        _leaderboardPanel.GetComponent<CanvasGroup>().interactable = false;
        _leaderboardPanel.SetActive(true);

        _canExpand = false;
    }

    public void FadeInLeaderboard()
    {
        _leaderboardPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(delegate(){
            _leaderboardPanel.GetComponent<CanvasGroup>().interactable = true;
            _scrollVerticalNormalizePosition = _leaderboardScroll.verticalNormalizedPosition;
            _canExpand = true;

            LoadingPanelController.instance.CloseLoadingPanel();
        }); 
    }

    private void AutoScrollAnimation(RectTransform player)
    {   
        _cloneLeaderboardItem.anchoredPosition = _leaderboardScroll.content.GetChild(2).GetComponent<RectTransform>().anchoredPosition;
        _cloneLeaderboardItem.DOMoveX(_leaderboardScroll.content.GetChild(2).GetComponent<RectTransform>().position.x, 0);
        _cloneLeaderboardItem.sizeDelta = _leaderboardScroll.content.GetChild(2).GetComponent<RectTransform>().sizeDelta;
        _cloneLeaderboardItem.gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();

        _leaderboardScroll.enabled = false;
        _scrollVerticalNormalizePosition = _leaderboardScroll.verticalNormalizedPosition;
        _leaderboardScroll.verticalNormalizedPosition = 0;

        _leaderboardPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(delegate(){
            LoadingPanelController.instance.CloseLoadingPanel();

            _cloneLeaderboardItem.DOAnchorPosY(_cloneLeaderboardItem.anchoredPosition.y - 50, 0.2f).OnComplete(delegate(){
                _cloneLeaderboardItem.DOAnchorPosY(_leaderboardScroll.content.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y, 0.8f);
            });
      
            _leaderboardScroll.DOVerticalNormalizedPos(-0.02f, 0.2f).OnComplete(delegate(){
                _leaderboardScroll.DOVerticalNormalizedPos(_scrollVerticalNormalizePosition, 1f).OnComplete(delegate(){
                    _cloneLeaderboardItem.DOMoveY(player.position.y, 0.3f).OnComplete(delegate(){
                    
                        _playerLeaderboardEngagement.StartCoroutine(_playerLeaderboardEngagement.GetTotalPlayers(player));
                        player.GetComponent<LeaderboardItem>().AnimateNameText();
                        player.GetComponent<LeaderboardItem>().ChangeColorsToPlayerColors();
                        _cloneLeaderboardItem.gameObject.SetActive(false);

                        _leaderboardPanel.GetComponent<CanvasGroup>().interactable = true;
                        _leaderboardScroll.enabled = true;
                        _canExpand = true;
                    });
                });
            }); 
        });
    }

    private void Update()
    {
        if(_canExpand)
        {
            if(_scrollVerticalNormalizePosition + 0.05f < _leaderboardScroll.verticalNormalizedPosition 
                || _scrollVerticalNormalizePosition - 0.05f > _leaderboardScroll.verticalNormalizedPosition)
            {
                _canExpand = false;
                
                _resizeLeaderboard.ResizeLeaderboardLayout(
                    _leaderboardScroll.GetComponent<ResizeReferences>().targetHeight, 
                    _leaderboardScroll.GetComponent<ResizeReferences>().targetAnchorPosY, 
                    _topPositionsPanel.GetComponent<ResizeReferences>().targetHeight, 
                    _topPositionsPanel.GetComponent<ResizeReferences>().targetAnchorPosY);
            }                       
        }
    }
}
