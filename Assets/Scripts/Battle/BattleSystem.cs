using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;
    

    BattleState state;
    int currentAction; //Run / Party / Bag / Fight
    int currentMove; // Mon moves
    int currentMember; // Party selection

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

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Borpamon.Moves);

        yield return
            StartCoroutine(dialogBox.TypeDialog($"A wild {enemyUnit.Borpamon.Base.Name} appeared!"));
        yield return new WaitForSeconds(1f);

        ActionSelection();
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog($"Choose an action");

        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Borpamons);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Borpamon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if(state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Borpamon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);
        
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        --move.Pp;

        yield return dialogBox.TypeDialog($"{sourceUnit.Borpamon.Base.Name} used {move.Base.Name}!");
        yield return new WaitForSeconds(1f);

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();

        var damageDetails = targetUnit.Borpamon.TakeDamage(move, sourceUnit.Borpamon);
        yield return targetUnit.Hud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            currentMove = 0; //Prevents user from accessing a move that doesnt exist
            yield return dialogBox.TypeDialog($"{targetUnit.Borpamon.Base.Name} Fainted.");
            targetUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);

            CheckForBattleOver(targetUnit);

        }
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextBorpamon = playerParty.GetHealthyBorpamon();
            if (nextBorpamon != null)  // if there's a living pokemon in the party
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
            BattleOver(true);
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
                MoveSelection();

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
            StartCoroutine(PlayerMove());
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        
        currentMove = Mathf.Clamp(currentMember, 0, playerParty.Borpamons.Count - 1);
        
        partyScreen.UpdateMemberSelection(currentMember);
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Borpamons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted Borpamon");
                return;
            }
            if (selectedMember == playerUnit.Borpamon)
            {
                partyScreen.SetMessageText("You cant switch with the same Borpamon");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchBorpamon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }
    
    IEnumerator SwitchBorpamon(Borpamon newBorpamon)
    {
        if (playerUnit.Borpamon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Borpamon.Base.Name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newBorpamon);
        dialogBox.SetMoveNames(playerUnit.Borpamon.Moves);

        yield return dialogBox.TypeDialog($"Go {newBorpamon.Base.Name}!");
        yield return new WaitForSeconds(1f);

        StartCoroutine(EnemyMove());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }
}
