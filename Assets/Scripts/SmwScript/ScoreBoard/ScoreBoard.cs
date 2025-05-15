using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreBoard : MonoBehaviour
{
    [Header("ÅØ½ºÆ®")]
    [SerializeField] private TextMeshProUGUI[] texts = new TextMeshProUGUI[4];

    int MaxBlueScore;
    int MaxRedScore;
    int MaxYellowScore;


    public void HpPoint(int HpPoint)
    {
        texts[0].text = $"{HpPoint}";
    }
    public void BlueScore(int score)
    {
        texts[1].text = $"{score} / {MaxBlueScore}";
        
    }

    public void RedScore(int score)
    {
        texts[2].text = $"{score} / {MaxRedScore}";
    }

    public void YellowScore(int score)
    {
        texts[3].text = $"{score} / {MaxYellowScore}";
    }

    public void ResetScore()
    {
        for (int i = 0; i > texts.Length; i++)
        {
            texts[i].text = $"0 / 0";
        }
        MaxBlueScore = 0;
        MaxRedScore = 0;
        MaxYellowScore = 0;
    }

}
