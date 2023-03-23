using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).OnComplete(delegate(){
            
            _animator.enabled = true;   
            PlayBounce();
            InvokeRepeating("PlayBounce", 5, 5);
        });
    }

    public void SetColor(Color color) => GetComponent<Image>().color = color;

    private void PlayBounce() => _animator.Play("Bounce", -1, 0);

    private void OnDestroy() => CancelInvoke();
}
