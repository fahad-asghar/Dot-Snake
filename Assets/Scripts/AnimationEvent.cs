using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEvent : MonoBehaviour
{
    public void OnSplashAnimationCompleted(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PlaySound(AudioClip audioClip)
    {
        if(gameObject.name.Equals("SwipeAnimation2") || gameObject.name.Equals("SwipeAnimation3"))
            return;
            
        SoundManager.instance.playSound(audioClip);
    }
}
