using System;
using System.Collections;
using System.Collections.Generic;
using Svelto.ECS.Hybrid;
using TMPro;
using UnityEngine;

public class GameHUDImplementor : MonoBehaviour, IImplementor, IGameStateListener, IScoreHUD
{
    public GameObject IntroUI, GameplayUI, GameOverUI;

    private EnumGameState _state;
    public EnumGameState State {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            UpdateStateUI();
        }
    }

    public TextMeshProUGUI scoreText, gameOverScoreText;
    
    private int score = 0;
    
    void UpdateStateUI()
    {
        IntroUI.SetActive(_state == EnumGameState.INTRO);
        GameOverUI.SetActive(_state == EnumGameState.GAMEOVER);
        GameplayUI.SetActive(_state == EnumGameState.PLAY);
    }

    public int Score {
        get
        {
            return score;
        }
        set
        {
            score = value;
            gameOverScoreText.text = score.ToString();
            scoreText.text = score.ToString();
        } }
}
