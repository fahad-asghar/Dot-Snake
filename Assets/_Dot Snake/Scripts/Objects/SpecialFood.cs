using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SpecialFood : MonoBehaviour
{    
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).OnComplete(delegate(){
            PlayRipple();
        });
    }

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
        transform.GetChild(0).GetComponent<Image>().color = color;
    }

    private void PlayRipple() => _animator.Play("Ripple", -1, 0);
}
