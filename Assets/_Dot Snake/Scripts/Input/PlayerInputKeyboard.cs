using UnityEngine;

public class PlayerInputKeyboard : MonoBehaviour
{
    public delegate void Action(Direction direction);
    public static event Action OnKeyPress;   

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            OnKeyPress?.Invoke(Direction.Up);

        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            OnKeyPress?.Invoke(Direction.Down);

        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            OnKeyPress?.Invoke(Direction.Left);

        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            OnKeyPress?.Invoke(Direction.Right);
    }
}
