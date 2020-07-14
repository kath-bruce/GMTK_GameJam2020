using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasswordComponent : MonoBehaviour
{
    public string Password;

    private TextMeshProUGUI currentPassword;
    private int revealedChars = 0;

    void Start()
    {
        currentPassword = GetComponent<TextMeshProUGUI>();
        currentPassword.text = new string('_', Password.Length);
        revealedChars = 0;
    }

    public void AddLetter()
    {
        currentPassword.text = currentPassword.text.Remove(revealedChars, 1).Insert(revealedChars, Password[revealedChars].ToString());
        revealedChars++;
    }

    public bool CompletedPassword()
    {
        return revealedChars == Password.Length;
    }

    public void Restart()
    {
        Start();
    }
}
