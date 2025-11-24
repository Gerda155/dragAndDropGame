using UnityEngine;

public class RulesUI : MonoBehaviour
{
    public GameObject rulesPanel;

    private bool isOpen = false;

    public void ToggleRules()
    {
        isOpen = !isOpen;

        if (rulesPanel != null)
            rulesPanel.SetActive(isOpen);
    }
}
