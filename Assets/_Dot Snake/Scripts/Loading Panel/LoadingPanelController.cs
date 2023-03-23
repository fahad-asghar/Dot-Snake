using UnityEngine;
using TMPro;
using DG.Tweening;

public class LoadingPanelController : MonoBehaviour
{
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private TextMeshProUGUI _loadingPanelText;

    public static LoadingPanelController instance;

    private void Awake() => instance = this;

    public void OpenLoadingPanel(string text)
    {
        _loadingPanelText.text = text;
        
        _loadingPanel.SetActive(true); 
        _loadingPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(delegate(){       
                
        }); 
    }

    public void CloseLoadingPanel()
    {
        _loadingPanel.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(delegate(){
            _loadingPanel.SetActive(false);       
        });
    }
}
