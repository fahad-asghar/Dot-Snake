using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    private SnakeProperties _snakeProperties;
    private SnakeBlink _snakeBlink;

    public List<RectTransform> segments;
    [HideInInspector] public List<SnakePath> snakePath = new List<SnakePath>();
 
    [Tooltip("The lower the more")] 
    public float moveSpeed;
    private float _movementCounter;

    [HideInInspector] public Direction direction;
    [HideInInspector] public int directionX;
    [HideInInspector] public int directionY;

    private bool _blockInput;

    private void Start()
    {
        _snakeProperties = GetComponent<SnakeProperties>();
        _snakeBlink = GetComponent<SnakeBlink>();

        PlayerInputSwipe.OnSwipe += OnSwipe;
        PlayerInputKeyboard.OnKeyPress += OnKeyPress;
        _snakeBlink.OnBlinkDone += OnBlinkDone;

        directionX = 0;
        directionY = 40;
    }

    private void OnDestroy()
    {
        PlayerInputSwipe.OnSwipe -= OnSwipe;
        PlayerInputKeyboard.OnKeyPress -= OnKeyPress;
        _snakeBlink.OnBlinkDone -= OnBlinkDone;
    } 

    private void OnSwipe(SwipeData swipeData)
    {
        if(GameController.instance.stopMovement)
            return;

        switch (swipeData.Direction)
        {
            case Direction.Up:
                Set_Up();
                break;

            case Direction.Down:
                Set_Down();
                break;

            case Direction.Left:
                Set_Left();
                break;

            case Direction.Right:
                Set_Right();
                break;
        }
    }
    
    private void OnKeyPress(Direction direction)
    {
        if(GameController.instance.stopMovement)
            return;

        switch (direction)
        {
            case Direction.Up:
                Set_Up();
                break;

            case Direction.Down:
                Set_Down();
                break;

            case Direction.Left:
                Set_Left();
                break;

            case Direction.Right:
                Set_Right();
                break;
        }
    }

    private void Set_Up()
    {
        if(directionY != -40 && !_blockInput)
        {
            direction = Direction.Up;
            directionX = 0;
            directionY = 40;

            _blockInput = true;

            _snakeProperties.RotateFace(117);
        }
    }

    private void Set_Down()
    {
        if(directionY != 40 && !_blockInput)
        {
            direction = Direction.Down;
            directionX = 0;
            directionY = -40;

            _blockInput = true;

            _snakeProperties.RotateFace(297);
        }
    }

    private void Set_Left()
    {
        if(directionX != 40 && !_blockInput)
        {
            direction = Direction.Left;
            directionX = -40;
            directionY = 0;

            _blockInput = true;

            _snakeProperties.RotateFace(207);
        }
    }

    private void Set_Right()
    {   
        if(directionX != -40 && !_blockInput)
        {
            direction = Direction.Right;
            directionX = 40;
            directionY = 0;
            
            _blockInput = true;

            _snakeProperties.RotateFace(27);
        }
    }


    private void Update()
    {
        if(GameController.instance.stopMovement)
            return;

        _movementCounter += Time.deltaTime;

        if(_movementCounter > moveSpeed)
        {           
            Move();
            _movementCounter = 0;
        }     
    }

    private void Move()
    {
        SoundManager.instance.playSound(SoundManager.instance.snakeMove, 0.1f);
        _blockInput = false;
        
        for(int i = segments.Count - 1; i > 0; i--)
            segments[i].anchoredPosition = segments[i - 1].anchoredPosition;

        segments[0].anchoredPosition = new Vector2(Mathf.Round(segments[0].anchoredPosition.x + directionX), Mathf.Round(segments[0].anchoredPosition.y + directionY));
        
        snakePath.Add(new SnakePath(){
            anchoredPosition = segments[0].anchoredPosition,
            directionX = directionX,
            directionY = directionY
        });

        _snakeProperties.FadeInFaceLayer();
    }

    private void OnBlinkDone() => StartMovement();
         
    public void StartMovement()
    {
        segments[0].GetComponent<Collider2D>().enabled = true;
        GameController.instance.stopMovement = false;
    }
}
