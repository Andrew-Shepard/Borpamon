using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState gameState;

    private void Start()
    {
        playerController.OnEncountered += FreeRoamtoBattleAnimation; //player controller subscribes to the event on encountered then
        //starts the function startbattle
        battleSystem.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        //gameState = GameState.Battle; called in freeroamtobattleanimation
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<BorpamonParty>();
        //FindObjectOfType returns the object with map area and get component gets maparea
        var wildBorpamon = FindObjectOfType<MapArea>()
                                .GetComponent<MapArea>()
                                       .GetRandomWildBorpamon();

        battleSystem.StartBattle(playerParty,wildBorpamon);
    }

    void EndBattle(bool won)
    {
        gameState = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
    
    private void FreeRoamtoBattleAnimation()
    {
        var sequence = DOTween.Sequence();
        //Previous issue where OnComplete called startbattle twice, I think it was from the shake position reaching its
        //starting point twice during the animation. changing the fade to false seems to have fixed.
        gameState = GameState.Battle;

        
        sequence.Append(worldCamera.DOShakePosition(1f, new Vector3(.2f,0,0), 10, 10, false)).OnComplete(this.StartBattle);
    }

    private void Update() // controls the player inputs
    {
        if (gameState == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (gameState == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
