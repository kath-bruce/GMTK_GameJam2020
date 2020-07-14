using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class LoseComponent : MonoBehaviour
{
    public GameManager GameManager;

    public void Restart()
    {
        GameManager.Restart();

        this.gameObject.SetActive(false);
    }
}
