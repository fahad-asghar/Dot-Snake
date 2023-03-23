using UnityEngine;
using DG.Tweening;

public class SnakeEyes : MonoBehaviour
{
    void Start()
    {
        MoveEyesRandomly();
    }

    private void MoveEyesRandomly()
    {
        transform.DOLocalMove(new Vector2(Random.Range(-138, -4), Random.Range(-107, 5)), 0.15f, false).SetDelay(Random.Range(0.3f, 1f)).OnComplete(delegate(){
            MoveEyesRandomly();
        });
    }
}
