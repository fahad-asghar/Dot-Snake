using UnityEngine;
using DG.Tweening;

public class Obstacle : MonoBehaviour
{
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f); 
    }
}
