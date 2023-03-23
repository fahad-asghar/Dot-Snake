using UnityEngine;
using DG.Tweening;

public class HealthPoint : MonoBehaviour
{
    private Animator _animator;
    private Deactivate _deactivate;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _deactivate = GetComponent<Deactivate>();

        _animator.enabled = false;

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.4f).OnComplete(delegate(){
            _animator.enabled = true;
            InvokeRepeating("PlayBounce", 2f, 2f);
            Invoke("Deactivate", 10);
        });
    }

    private void PlayBounce() => _animator.Play("Bounce", -1, 0);

    private void Deactivate() => _deactivate.DeactivateObject();

    private void OnDestroy() => CancelInvoke();
}
