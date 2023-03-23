using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class SnakeImpactColorChange : MonoBehaviour
{
    [SerializeField] private Color impactColor;
    [SerializeField] private Color defaultColor;

    [Space][Space][Space]
    [SerializeField] private Image gridFrame;
    
    private SnakeMovement _snakeMovement;

    public event Action OnImpact_ColorEffectDone;

    private void Start() => _snakeMovement = GetComponent<SnakeMovement>();

    public void OnImpact_ColorEffect()
    {
        for(int i = 0; i < _snakeMovement.segments.Count; i++)
        {
            _snakeMovement.segments[i].GetComponent<Image>().DOColor(impactColor, 0.4f);

            for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(impactColor, 0.4f);      
        }

        gridFrame.DOColor(impactColor, 0.5f).OnComplete(delegate(){
            
            for(int i = 0; i < _snakeMovement.segments.Count; i++)
            {
                _snakeMovement.segments[i].GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(1f);

                for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                    _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(1);      
            }

            gridFrame.DOColor(defaultColor, 0.5f).SetDelay(1f).OnComplete(delegate(){
                _snakeMovement.segments[0].GetComponent<Image>().DOFade(0, 0f).SetDelay(1f);
                OnImpact_ColorEffectDone?.Invoke();
            });     
        });
    }
}
