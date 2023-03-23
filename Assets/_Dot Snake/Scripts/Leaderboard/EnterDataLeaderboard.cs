using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class EnterDataLeaderboard : MonoBehaviour
{
    [SerializeField] private string name;
    [SerializeField] private int score;
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            UpdatePlayerName(name, score);
    }

    public void UpdatePlayerName(string name, int score)
    {
        var request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, 
        resultCallback => {

            Debug.Log("Name updatad to - " + resultCallback.DisplayName);
            SendScoreToLeaderboard(score);
        }, 

        error => {
            Debug.Log("Error while updating display name");
            Debug.Log(error.GenerateErrorReport());
        });
    }

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
        }, 

        errorCallback => {
            Debug.Log("Error sending score to leaderboard");
            Debug.Log(errorCallback.GenerateErrorReport());
        });
    }
}
