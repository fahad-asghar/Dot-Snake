using UnityEngine;

public class SnakeRegisterImpact : MonoBehaviour
{
    public delegate void Action(string tag, GameObject impactObject);
    public event Action OnImpact;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        OnImpact?.Invoke(collider.gameObject.tag, collider.gameObject);
    }
}
