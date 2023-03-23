using UnityEngine;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;

public class UpdatePlayerScore : MonoBehaviour
{
    private SetupLeaderboard _setupLeaderboard;

    private void Start() => _setupLeaderboard = GetComponent<SetupLeaderboard>();

    public void SendScoreToLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest(){
            Statistics = new List<StatisticUpdate>{ new StatisticUpdate(){
                    StatisticName = "Leaderboard",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,

        resultCallback => {

            Debug.Log("Score sent successfully");
            _setupLeaderboard.Invoke("Setup", 1);
        
        }, 

        Error => {

            LoadingPanelController.instance.CloseLoadingPanel();
            PushNotificationController.instance.SendPushNotification("Unable to update score due to poor network conditions, please try again");
        
        });
    }
}
