using System.Collections.Generic;
using UnityEngine;

public class PegPhysics : MonoBehaviour
{
    public Stack<DiskPhysics> disks = new Stack<DiskPhysics>();
    public Transform topPosition; // точка, откуда начинается верхний диск

    // Можно ли поставить диск на эту башню
    public bool CanPlaceDisk(DiskPhysics disk)
    {
        if (disks.Count == 0) return true;
        return disk.size < disks.Peek().size;
    }

    // Ставим диск на башню
    public void PlaceDisk(DiskPhysics disk)
    {
        disks.Push(disk);
        disk.currentPeg = this;
        disk.transform.position = topPosition.position + Vector3.up * disks.Count * 0.3f; // смещение
        disk.rb.linearVelocity = Vector2.zero; // останавливаем падение
        disk.rb.gravityScale = 0; // фиксируем диск
    }

    // Берём верхний диск
    public DiskPhysics TakeDisk()
    {
        if (disks.Count == 0) return null;
        return disks.Pop();
    }
}
