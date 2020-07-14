using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Managers;

public class TutorialComponent : MonoBehaviour
{
    public List<string> TutorialTexts;
    public List<Sprite> TutorialImages;

    public Image CurrentImage;
    public TextMeshProUGUI CurrentText;
    public GameManager Manager;
    private int currentSlide = 0;

    void Start()
    {
        CurrentImage.preserveAspect = true;
        SetTutorialImageAndText();
    }

    void SetTutorialImageAndText()
    {
        if (TutorialImages.Count == 0)
        {
            return;
        }
        CurrentImage.sprite = TutorialImages[currentSlide];
        CurrentText.text = TutorialTexts[currentSlide];
    }

    void OnCycleThroughTutorial(InputValue value)
    {
        var dir = value.Get<float>();

        if (dir > 0)
        {
            currentSlide++;

            if (currentSlide == TutorialTexts.Count)
            {
                Manager.StartGame();
                return;
            }

            SetTutorialImageAndText();
        }
        else if (dir < 0)
        {
            currentSlide--;

            if (currentSlide < 0)
            {
                currentSlide = 0;
            }

            SetTutorialImageAndText();
        }

    }
}
