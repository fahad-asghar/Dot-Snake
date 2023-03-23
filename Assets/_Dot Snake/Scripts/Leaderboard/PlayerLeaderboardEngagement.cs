using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLeaderboardEngagement : MonoBehaviour
{
    private string _url = "https://52c43.playfabapi.com/Server/GetPlayersInSegment?SegmentId=CD5C2E59D7ABAB20";

    public IEnumerator GetTotalPlayers(RectTransform player)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(_url, "");

        webRequest.SetRequestHeader("X-SecretKey", "SQ5M9T589ZM66IO31AOYTW9EM91Q9S1F4Y3UC7H33ICIUHJAQX");
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(webRequest.error);

        else
        {
            string text = webRequest.downloadHandler.text;
            APIFormat json = JsonUtility.FromJson<APIFormat>(text);

            UpdatePlayerText(player, json.data.ProfilesInSegment);
        }
    }

    private void UpdatePlayerText(RectTransform player, int totalPlayers)
    {
        if(player  == null)
            return;
            
        float percentage = (float.Parse(player.GetComponent<LeaderboardItem>().indexText.text) / (float)totalPlayers) * 100;
        
        if(percentage < 50)
            player.GetComponent<LeaderboardItem>().nameText1.text += " | <b><color=#D462F0>In top " + percentage.ToString(".") + "%</color></b>";
        else
            player.GetComponent<LeaderboardItem>().nameText1.text += " | <b><color=#D462F0>Below " + percentage.ToString(".") + "%</color></b>";

        player.GetComponent<LeaderboardItem>().nameText2.text += " | <b><color=#D462F0>Out of " + totalPlayers + "</color></b>";
    }   
}

[System.Serializable]
public class APIFormat
{
    public int code;
    public string status;
    public Data data;
}

[System.Serializable]
public class Data
{
    public int ProfilesInSegment;
}
