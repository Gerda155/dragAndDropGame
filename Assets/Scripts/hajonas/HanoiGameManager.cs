using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HanoiGameManager : MonoBehaviour
{
    public HanoiTower[] towers;
    public HanoiDisk[] disks;
    public int totalDisks = 6;
    public int winningTowerIndex = 2;
    public TextMeshProUGUI counterText;
    public Victory victory;

    private int moveCount = 0;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        if (towers == null || towers.Length != 3)
        {
            Debug.LogError("HanoiGameManager needs exactly 3 towers!");
            return;
        }

        if (disks == null || disks.Length != totalDisks)
        {
            Debug.LogError("HanoiGameManager: disks array size doesn't match totalDisks!");
            return;
        }

        for (int i = 0; i < disks.Length; i++)
        {
            disks[i].diskSize = disks.Length - i;
            towers[0].AddDisk(disks[i]);
        }

        moveCount = 0;
        UpdateCounterDisplay();
    }

    public void OnDiskMoved()
    {
        moveCount++;
        UpdateCounterDisplay();
        CheckWinCondition();
    }

    private void UpdateCounterDisplay()
    {
        if (counterText != null)
        {
            counterText.text = moveCount.ToString();
        }
    }

    private void CheckWinCondition()
    {
        if (towers[winningTowerIndex].GetDiskCount() == totalDisks)
        {
            Debug.Log($"You Win! Moves: {moveCount}");
            OnGameWon();
        }
    }

    private void OnGameWon()
    {
        Debug.Log("Congratulations! You solved the Tower of Hanoi!");

        if (victory != null)
        {
            victory.ShowVictory(moveCount);
        }
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void ReduceMoves(int amount)
    {
        moveCount = Mathf.Max(0, moveCount - amount);
        UpdateCounterDisplay();
    }

}
