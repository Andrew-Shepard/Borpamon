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

    BattleState state;
    int currentAction;
    int currentMove;

    private IEnumerator coroutine;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Borpamon);
        enemyHud.SetData(enemyUnit.Borpamon);

        dialogBox.SetMoveNames(playerUnit.Borpamon.Moves);

        yield return
            StartCoroutine( dialogBox.TypeDialog($"A wild {enemyUnit.Borpamon.Borpamon_base.Name} appeared!") );
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        coroutine = dialogBox.TypeDialog($"Choose an action");

        state = BattleState.PlayerAction;
        StartCoroutine(coroutine);
        
        dialogBox.EnableActionSelector(true);
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

        yield return dialogBox.TypeDialog($"{playerUnit.Borpamon.Borpamon_base.Name} used {move.Base.Name}!");

        yield return new WaitForSeconds(1f);

        bool isFainted = enemyUnit.Borpamon.TakeDamage(move, playerUnit.Borpamon);
        yield return enemyHud.UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"Enemy {enemyUnit.Borpamon.Borpamon_base.Name} Fainted.");
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

        yield return dialogBox.TypeDialog($"{enemyUnit.Borpamon.Borpamon_base.Name} used {move.Base.Name}!");

        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Borpamon.TakeDamage(move, enemyUnit.Borpamon);
        yield return playerHud.UpdateHP();
        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Borpamon.Borpamon_base.Name} Fainted.");
        }
        else
        {
            PlayerAction();
        }
    }
    void HandleActionSelection()
    {   //1 is run, 0 is fight
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                currentAction--;
        }

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
                //Run

            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Borpamon.Moves.Count - 1)
                currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Borpamon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Borpamon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StopCoroutine(coroutine);
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
    private void Update()
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
