using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class WinComponent : MonoBehaviour
{
    public GameManager GameManager;

    public void SetWinText(string wintext)
    {
        //???????????????
    }

    public void Restart()
    {
        //???????????????

        this.gameObject.SetActive(false);
    }
}
