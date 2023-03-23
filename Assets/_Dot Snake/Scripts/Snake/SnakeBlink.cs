using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SnakeBlink : MonoBehaviour
{
    [SerializeField] private Color blinkColor;
    [SerializeField] private Color defaultColor;

    private SnakeMovement _snakeMovement;

    public event Action OnBlinkDone;
    
    private void Start() => _snakeMovement = GetComponent<SnakeMovement>();

    public void Blink()
    {
        for(int i = 0; i < _snakeMovement.segments.Count; i++)
        {
            _snakeMovement.segments[i].GetComponent<Image>().DOColor(blinkColor, 0.4f);

            for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(blinkColor, 0.4f);      
        }

        for(int i = 0; i < _snakeMovement.segments.Count; i++)
        {
            _snakeMovement.segments[i].GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(0.4f);

            for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(0.4f);      
        }

        for(int i = 0; i < _snakeMovement.segments.Count; i++)
        {
            _snakeMovement.segments[i].GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(0.8f);

            for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(0.8f);      
        }

        for(int i = 0; i < _snakeMovement.segments.Count; i++)
        {
            _snakeMovement.segments[i].GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(1.2f);

            for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(1.2f);      
        }

        for(int i = 0; i < _snakeMovement.segments.Count; i++)
        {
            _snakeMovement.segments[i].GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(1.6f);

            for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(1.6f);      
        }

        for(int i = 0; i < _snakeMovement.segments.Count; i++)
        {
            _snakeMovement.segments[i].GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(2f);

            for(int j = 0; j < _snakeMovement.segments[i].childCount; j++)
                _snakeMovement.segments[i].GetChild(j).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(2f);      
        }

        _snakeMovement.segments[0].GetComponent<Image>().DOFade(0, 0f).SetDelay(2.4f).OnComplete(delegate(){
            OnBlinkDone?.Invoke();
        });

        for(float i = 0; i < 2f; i = i + 0.8f)
            Invoke("PlayBlinkSound", i);
    }

    private void PlayBlinkSound() => SoundManager.instance.playSound(SoundManager.instance.snakeBlink, 0.2f);
}
