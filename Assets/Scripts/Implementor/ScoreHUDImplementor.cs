using System.Collections;
using System.Collections.Generic;
using Svelto.ECS.Hybrid;
using TMPro;
using UnityEngine;


public class ScoreHUDImplementor : MonoBehaviour, IImplementor, IScoreHUD
{
    public TextMeshProUGUI scoreText;
    
    private int score = 0;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }
}
