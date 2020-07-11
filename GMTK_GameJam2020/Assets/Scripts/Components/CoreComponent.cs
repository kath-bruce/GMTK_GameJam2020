using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class CoreComponent : MonoBehaviour
{
    public CoreBase CoreElement { get; set; }

    public void SetHighlight(bool highlight)
    {
        transform.GetChild(0).gameObject.SetActive(highlight);
    }
}
