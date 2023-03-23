using UnityEngine;

public class PlayerInputSwipe : MonoBehaviour
{
    [SerializeField] private bool detectSwipeOnlyAfterRelease = false;
    [SerializeField] private float minDistanceForSwipe = 5f;
    
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    public delegate void Action(SwipeData data);
    public static event Action OnSwipe;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fingerUpPosition = Input.mousePosition;
            fingerDownPosition = Input.mousePosition;
        }

        if (!detectSwipeOnlyAfterRelease && Input.GetMouseButton(0))
        {
            fingerDownPosition = Input.mousePosition;
            CheckSwipe();
        }

        if (Input.GetMouseButtonUp(0))
        {
            fingerDownPosition = Input.mousePosition;
            CheckSwipe();
        }
    }

    private void CheckSwipe()
    {
        float deltaX = fingerDownPosition.x - fingerUpPosition.x;
        float deltaY = fingerDownPosition.y - fingerUpPosition.y;

        if (Mathf.Abs(deltaX) > minDistanceForSwipe || Mathf.Abs(deltaY) > minDistanceForSwipe)
        {
            Direction direction = GetSwipeDirection(deltaX, deltaY);
            SwipeData swipeData = new SwipeData(fingerDownPosition, fingerUpPosition, direction);
            OnSwipe?.Invoke(swipeData);
        }

        fingerUpPosition = fingerDownPosition;
    }

    private Direction GetSwipeDirection(float deltaX, float deltaY)
    {
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            return (deltaX > 0) ? Direction.Right : Direction.Left;

        else
            return (deltaY > 0) ? Direction.Up : Direction.Down;
    }
}

public struct SwipeData
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public Direction Direction;

    public SwipeData(Vector2 start, Vector2 end, Direction dir)
    {
        StartPosition = start;
        EndPosition = end;
        Direction = dir;
    }
}

