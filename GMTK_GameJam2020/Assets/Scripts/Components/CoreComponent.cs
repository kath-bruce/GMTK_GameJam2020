using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

public class CoreComponent : MonoBehaviour
{
    public CoreBase CoreElement { get; set; }

    public void SetHighlight(bool highlight)
    {
        transform.GetChild(0).gameObject.SetActive(highlight);
    }

    public void SetMenuHighlight(bool highlight)
    {
        GetComponent<Image>().enabled = highlight;
    }
}
