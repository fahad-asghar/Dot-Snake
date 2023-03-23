using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;

public class UpdatePlayerName : MonoBehaviour
{
    private UpdatePlayerScore _updatePlayerScore;

    private void Start() => _updatePlayerScore = GetComponent<UpdatePlayerScore>();

    public void UpdateName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, 
        resultCallback => {
            
            Debug.Log("Name updatad to - " + resultCallback.DisplayName);
            _updatePlayerScore.SendScoreToLeaderboard((int)GameController.instance.score);
        
        }, 

        Error => {

            LoadingPanelController.instance.CloseLoadingPanel();
            PushNotificationController.instance.SendPushNotification("Failed to update the name due to unknown reason, please try again");
        
        });
    }
}
