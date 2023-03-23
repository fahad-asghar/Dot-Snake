using UnityEngine;

public class SnakeSpeedController : MonoBehaviour
{
    private SnakeMovement snakeMovement;

    [SerializeField] private float speedIncreaseFactor;
    [SerializeField] private float speedDecreaseFactor;

    private void Start() => snakeMovement = GetComponent<SnakeMovement>();

    public void IncreaseSpeed()
    {
        if(snakeMovement.moveSpeed > 0.03f)
            snakeMovement.moveSpeed -= speedIncreaseFactor;
    }

    public void DecreaseSpeed()
    {
        snakeMovement.moveSpeed += speedDecreaseFactor;
    }

    public void SetSpeed(float speed)
    {
        snakeMovement.moveSpeed = speed;
    }
}
