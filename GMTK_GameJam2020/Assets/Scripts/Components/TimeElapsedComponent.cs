using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Managers;

public class TimeElapsedComponent : MonoBehaviour
{
    private TextMeshProUGUI timeText;
    public float TimeElapsed { get; private set; } = 0f;
    GameManager gameManager;

    void Start()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var text in texts)
        {
            if (text.tag == "Time")
            {
                timeText = text;
                break;
            }
        }

        gameManager = FindObjectOfType<GameManager>();
        gameManager.TimeElapsed = this;
    }

    void Update()
    {
        if (gameManager.State == GameState.Playing)
        {
            TimeElapsed += Time.deltaTime;
            timeText.text = TimeElapsed.ToString("#0.00") + "s";
        }
    }
}
