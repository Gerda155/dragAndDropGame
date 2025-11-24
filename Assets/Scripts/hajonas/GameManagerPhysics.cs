using UnityEngine;

public class GameManagerPhysics : MonoBehaviour
{
    public PegPhysics targetPeg;  // башня, где проверяем победу
    public int totalDisks;        // сколько всего дисков в игре

    private bool gameWon = false; // чтобы не проверять после победы

    void Update()
    {
        if (!gameWon)
            CheckWin();
    }

    void CheckWin()
    {
        // Проверяем, есть ли все диски на целевой башне
        if (targetPeg.disks.Count != totalDisks)
            return;

        // Проверяем порядок сверху вниз
        int previousSize = int.MaxValue; // больше любого диска
        foreach (var disk in targetPeg.disks)
        {
            if (disk.size > previousSize)
            {
                // порядок нарушен → нет победы
                return;
            }
            previousSize = disk.size;
        }

        // Если дошли сюда → победа!
        gameWon = true;
        Debug.Log("Победа! Все диски на целевой башне!");

        // Можно добавить UI или остановить игру:
        // Time.timeScale = 0; // останавливает физику
        // Или показать Canvas с надписью "Вы выиграли!"
    }
}
