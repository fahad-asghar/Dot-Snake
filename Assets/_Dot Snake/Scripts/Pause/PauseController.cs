using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePopUp;
    public bool canPause = false;

    public static PauseController instance;

    private void Awake() => instance = this;

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            if(canPause)
            {
                Time.timeScale = 0; 
                pausePopUp.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if(Time.timeScale != 0)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1;
            pausePopUp.SetActive(false);
        }
    }
}
