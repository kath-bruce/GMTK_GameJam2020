using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeElapsedComponent : MonoBehaviour
{
    private TextMeshProUGUI timeText;
    public float TimeElapsed { get; private set; } = 0f;

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
    }

    void Update()
    {
        TimeElapsed += Time.deltaTime;
        timeText.text = TimeElapsed.ToString("#.00") + "s";
    }
}
