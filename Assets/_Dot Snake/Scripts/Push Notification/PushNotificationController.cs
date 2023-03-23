using UnityEngine;
using TMPro;

public class PushNotificationController : MonoBehaviour
{
    [SerializeField] private GameObject _notificationObject;
    [SerializeField] private TextMeshProUGUI _notificationText;
    
    public static PushNotificationController instance;

    private void Awake() => instance = this;

    public void SendPushNotification(string text)
    {
        SoundManager.instance.playSound(SoundManager.instance.notification);

        _notificationText.text = text;
        _notificationObject.GetComponent<Animator>().Play("NotificationAnimation", -1, 0);
    }
}
