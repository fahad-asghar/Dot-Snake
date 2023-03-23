using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GridBlinkController : MonoBehaviour
{
    [SerializeField] private SnakeRegisterImpact snakeRegisterImpact;
    [SerializeField] private Image grid;
    [SerializeField] private float blinkSpeed;

    void Start()
    {
        snakeRegisterImpact.OnImpact += OnImpact;    
    }

    private void OnImpact(string tag, GameObject impactObject)
    {
        switch (tag)
        {
            case "Food":
                Blink(impactObject.GetComponent<Image>().color);
                break;

            case "SpecialFood":
                Blink(impactObject.GetComponent<Image>().color);
                break;      
        }
    }

    private void Blink(Color color)
    {
        grid.DOColor(color, blinkSpeed).OnComplete(delegate(){
            grid.DOColor(Color.white, blinkSpeed);
        });
    }
}
