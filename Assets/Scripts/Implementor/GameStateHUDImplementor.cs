using System;
using System.Collections;
using System.Collections.Generic;
using Svelto.ECS.Hybrid;
using UnityEngine;

public class GameStateHUDImplementor : MonoBehaviour, IImplementor, IGameStateListener
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

    void UpdateStateUI()
    {
        IntroUI.SetActive(_state == EnumGameState.INTRO);
        GameOverUI.SetActive(_state == EnumGameState.GAMEOVER);
        GameplayUI.SetActive(_state == EnumGameState.PLAY);
    }
}
