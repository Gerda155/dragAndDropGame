using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HanoiTower : MonoBehaviour, IDropHandler
{
    public int towerIndex = 0;
    public float diskSpacing = 50f;
    public float baseYPosition = 0f;
    public Transform diskContainer;

    private List<HanoiDisk> disks = new List<HanoiDisk>();
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (diskContainer == null)
        {
            diskContainer = transform;
        }
    }

    public void AddDisk(HanoiDisk disk)
    {
        disks.Add(disk);
        disk.SetTower(this);
        disk.transform.SetParent(diskContainer);

        RectTransform diskRect = disk.GetComponent<RectTransform>();
        diskRect.localScale = Vector3.one;

        if (diskRect.pivot.x != 0.5f || diskRect.pivot.y != 0.5f)
        {
            diskRect.pivot = new Vector2(0.5f, 0.5f);
        }

        UpdateDiskPositions();
    }

    public void RemoveDisk(HanoiDisk disk)
    {
        if (disks.Contains(disk))
        {
            disks.Remove(disk);
            UpdateDiskPositions();
        }
    }

    public bool CanPlaceDisk(HanoiDisk disk)
    {
        if (disks.Count == 0)
        {
            return true;
        }

        HanoiDisk topDisk = disks[disks.Count - 1];
        return disk.diskSize < topDisk.diskSize;
    }

    public bool IsTopDisk(HanoiDisk disk)
    {
        if (disks.Count == 0)
            return false;

        return disks[disks.Count - 1] == disk;
    }

    public int GetDiskCount()
    {
        return disks.Count;
    }

    public HanoiDisk GetTopDisk()
    {
        if (disks.Count == 0)
            return null;

        return disks[disks.Count - 1];
    }

    private void UpdateDiskPositions()
    {
        for (int i = 0; i < disks.Count; i++)
        {
            RectTransform diskRect = disks[i].GetComponent<RectTransform>();
            float yPos = baseYPosition + (i * diskSpacing);
            diskRect.anchoredPosition = new Vector2(0, yPos);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
    }
}