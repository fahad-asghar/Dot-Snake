using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LeaderboardItem : MonoBehaviour
{
    public TextMeshProUGUI indexText;
    public TextMeshProUGUI nameText1;
    public TextMeshProUGUI nameText2;
    public TextMeshProUGUI scoreText;

    public void ChangeColorsToPlayerColors()
    {
        GetComponent<Image>().color = Color.white;
        transform.GetChild(0).GetComponent<Image>().color = Color.black;

        indexText.color = Color.white;
        nameText1.color = Color.black;
        nameText2.color = Color.black;
        scoreText.color = Color.black;
    }

    public void AnimateNameText()
    {
        nameText1.DOFade(0, 0.5f);
        nameText2.DOFade(1, 0.5f).OnComplete(delegate(){

            nameText1.DOFade(1, 0.5f).SetDelay(4);
            nameText2.DOFade(0, 0.5f).SetDelay(4).OnComplete(delegate(){
                Invoke("AnimateNameText", 5);    
            });     
        });
    }

    public void StopTextAnimation()
    {
        DOTween.Kill(nameText1);
        DOTween.Kill(nameText2);
    }
}
