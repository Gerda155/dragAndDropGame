using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Victory : MonoBehaviour
{
    public Image victoryWindow;

    private Sprite oneStarSprite;
    private Sprite twoStarsSprite;
    private Sprite threeStarsSprite;
    public TextMeshProUGUI movesText;

    private int completionMoves;
    private bool victoryShown = false;

    void Awake()
    {
        if (victoryWindow != null)
        {
            victoryWindow.gameObject.SetActive(false);
        }
    }

    public void ShowVictory(int totalMoves)
    {
        if (victoryShown)
            return;

        victoryShown = true;
        completionMoves = totalMoves;

        DisplayVictoryWindow();
    }

    void DisplayVictoryWindow()
    {
        if (victoryWindow != null)
        {

            Canvas victoryCanvas = victoryWindow.GetComponentInParent<Canvas>();
            if (victoryCanvas != null)
            {
                victoryCanvas.sortingOrder = 1000;
            }

            RectTransform rectTransform = victoryWindow.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.SetAsLastSibling();
            }

            victoryWindow.gameObject.SetActive(true);
        }

        if (movesText != null)
        {
            movesText.text = completionMoves.ToString();
        }
    }

    public int GetCompletionMoves()
    {
        return completionMoves;
    }
}
