using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    BorpamonParty playerParty;
    Borpamon wildBorpamon;
    public void StartBattle(BorpamonParty playerParty, Borpamon wildBorpamon)
    {
        this.playerParty = playerParty;
        this.wildBorpamon = wildBorpamon;
        StartCoroutine(SetupBattle());
    }

   
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyBorpamon());
        enemyUnit.Setup(wildBorpamon);
        playerHud.SetData(playerUnit.Borpamon);
        enemyHud.SetData(enemyUnit.Borpamon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Borpamon.Moves);

        yield return
            StartCoroutine( dialogBox.TypeDialog($"A wild {enemyUnit.Borpamon.Base.Name} appeared!") );
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    { 
        state = BattleState.PlayerAction;
        dialogBox.SetDialog($"Choose an action");

        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        partyScreen.SetPartyData(playerParty.Borpamons);
        partyScreen.gameObject.SetActive(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Borpamon.Moves[currentMove];
        --move.Pp;

        yield return dialogBox.TypeDialog($"{playerUnit.Borpamon.Base.Name} used {move.Base.Name}!");
        yield return new WaitForSeconds(1f);

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        DamageDetails damageDetails = enemyUnit.Borpamon.TakeDamage(move, playerUnit.Borpamon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            currentMove = 0; //Prevents user from accessing a move that doesnt exist
            yield return dialogBox.TypeDialog($"Enemy {enemyUnit.Borpamon.Base.Name} Fainted.");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Borpamon.GetRandomMove();
        --move.Pp;

        yield return dialogBox.TypeDialog($"{enemyUnit.Borpamon.Base.Name} used {move.Base.Name}!");
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();

        DamageDetails damageDetails = playerUnit.Borpamon.TakeDamage(move, enemyUnit.Borpamon);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Borpamon.Base.Name} Fainted.");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            var nextBorpamon = playerParty.GetHealthyBorpamon();
            if (nextBorpamon != null)  // if there's a living pokemon in the party
            {
                playerUnit.Setup(nextBorpamon);
                playerHud.SetData(nextBorpamon);

                dialogBox.SetMoveNames(playerUnit.Borpamon.Moves);

                yield return dialogBox.TypeDialog($"Go {nextBorpamon.Base.Name}!");
                yield return new WaitForSeconds(1f);

                PlayerAction();
            }
            else
            {
                OnBattleOver(false);
            }
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A critical hit!");
            yield return new WaitForSeconds(1f);
        }
        
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's super effective!");
            yield return new WaitForSeconds(1f);
        } 
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("It's not very effective.");
            yield return new WaitForSeconds(1f);
        }
    }
    void HandleActionSelection()
    {   
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3); //Keeps the values within the 4

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                PlayerMove();

            }
            else if (currentAction == 1)
            {
                //Bag

            }
            else if (currentAction == 2)
            {
                //Borpamon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Run

            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Borpamon.Moves.Count - 1); 
        //Keeps the values within the the am of moves

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Borpamon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }
    public void HandleUpdate()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }
    

}
