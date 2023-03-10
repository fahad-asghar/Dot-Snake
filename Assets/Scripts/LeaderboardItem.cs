using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardItem : MonoBehaviour
{
    public TextMeshProUGUI initialsText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void ChangeColorsToPlayerColors()
    {
        GetComponent<Image>().color = Color.white;
        transform.GetChild(0).GetComponent<Image>().color = Color.black;

        initialsText.color = Color.white;
        nameText.color = Color.black;
        scoreText.color = Color.black;
    }
}
