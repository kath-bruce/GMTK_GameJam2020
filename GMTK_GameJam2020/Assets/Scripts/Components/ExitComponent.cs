using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitComponent : MonoBehaviour
{
    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ClosePopUp()
    {
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
