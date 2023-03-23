using UnityEngine;
using DG.Tweening;

public class Deactivate : MonoBehaviour
{
    private Collider2D _collider;
    private Animator _animator;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    public void DeactivateObject()
    {  
        _collider.enabled = false;
        _animator.enabled = false;

        for(int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<Collider2D>().enabled = false;

        transform.DOScale(Vector3.zero, 0.3f).OnComplete(delegate(){
            Destroy(gameObject);
        });
    }
}
